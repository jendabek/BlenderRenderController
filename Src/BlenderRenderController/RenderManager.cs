using BRClib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static BRClib.CommandARGS;

namespace BlenderRenderController
{
    /// <summary>
    /// Manages the render of a list of <see cref="Chunk"/>s.
    /// </summary>
    /// <remarks>
    /// After calling <see cref="Start"/>, changing any property
    /// will not affect the in progress render, you must <seealso cref="Abort"/>
    /// or wait for it to finish, then the next call to <see cref="Start"/> will
    /// use the new values
    /// </remarks>
    public class RenderManager
    {
        // holds info about the current render process, so changes
        // to RenderManager's properties won't affect a inProgress render
        class CurrentSettings
        {
            public string ChunksPath, BlendPath, BaseFileName;
            public int Max;
            //public List<Chunk> Chunks;
            public IReadOnlyList<Chunk> Chunks;

            public CurrentSettings(RenderManager parent)
            {
                Chunks = parent.ChunkList;
                ChunksPath = parent.ChunksFolderPath;
                BlendPath = parent.BlendFilePath;
                BaseFileName = parent.BaseFileName;
                Max = parent.MaxConcurrency;
            }
        }
        CurrentSettings _rcs;

        private ConcurrentBag<Process> procBag;
        private ConcurrentHashSet<int> framesRendered;
        private int chunksToDo, chunksInProgress,
                    initalChunkCount, currentIndex;

        private Timer timer;

        // Progress stuff
        bool canReportProgress;
        IProgress<RenderProgressInfo> progress;

        AppSettings appSettings = AppSettings.Current;

        public string ChunksFolderPath { get; set; }
        public string BlendFilePath { get; set; }
        public string BaseFileName { get; set; }
        public int MaxConcurrency { get; set; }

        public int NumberOfFramesRendered => framesRendered.Count;

        public List<Chunk> ChunkList { get; set; }
        public bool InProgress { get => _rcs != null; }

        /// <summary>
        /// Raised when all chunks finish rendering, 'e' is
        /// the total number of frames rendered
        /// </summary>
        public event EventHandler<int> Finished;


        public RenderManager()
        {
            timer = new Timer { Interval = 75, AutoReset = true };
            timer.Elapsed += TryQueueRenderProcess;
            MaxConcurrency = 2;
        }
        public RenderManager(IEnumerable<Chunk> chunks) : this()
        {
            ChunkList = new List<Chunk>(chunks);
        }
        // for testing
        public RenderManager(IEnumerable<Chunk> chunks, AppSettings settings)
            : this(chunks)
        {
            this.appSettings = settings;
        }

        public RenderManager(ProjectSettings project)
            : this(project.ChunkList)
        {
            Setup(project);
        }

        /// <summary>
        /// Setup <see cref="RenderManager"/> using a <see cref="ProjectSettings"/> object
        /// </summary>
        /// <param name="project"></param>
        public void Setup(ProjectSettings project)
        {
            ChunkList = new List<Chunk>(project.ChunkList);
            MaxConcurrency = project.ProcessesCount;
            BlendFilePath = project.BlendPath;
            BaseFileName = project.BlendData.ProjectName;
            ChunksFolderPath = Path.Combine(project.BlendData.OutputPath, "chunks");
        }

        /// <summary>
        /// Starts rendering 
        /// </summary>
        /// <param name="prog">Progress provider</param>
        public void Start(IProgress<RenderProgressInfo> prog)
        {
            // do not start if its already in progress
            if (InProgress)
            {
                throw new InvalidOperationException("A render is already in progress");
            }

            CheckForValidProperties();

            progress = prog;
            canReportProgress = progress != null;

            procBag = new ConcurrentBag<Process>();
            framesRendered = new ConcurrentHashSet<int>();
            currentIndex = 0;
            chunksInProgress = 0;
            chunksToDo = ChunkList.Count;
            initalChunkCount = ChunkList.Count;

            _rcs = new CurrentSettings(this);

            timer.Start();
        }

        /// <summary>
        /// Starts rendering
        /// </summary>
        public void Start()
        {
            Start(null);
        }

        /// <summary>
        /// Aborts the render process
        /// </summary>
        public void Abort()
        {
            timer.Stop();
            DisposeProcesses();
            _rcs = null;
        }

        private void CheckForValidProperties()
        {
            string[] mustHaveValues = { ChunksFolderPath, BlendFilePath, BaseFileName };

            if (mustHaveValues.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new Exception("Required info missing");
            }

            if (ChunkList.Count == 0)
            {
                throw new Exception("Chunk list is empty");
            }

            //if (MaxConcurrency < 2)
            //{
            //    throw new Exception("Must have a MaxConcurrency value of 2 or more");
            //}

            if (!File.Exists(BlendFilePath))
            {
                throw new FileNotFoundException("Could not find 'blend' file", BlendFilePath);
            }

            if (!Directory.Exists(ChunksFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(ChunksFolderPath);
                }
                catch (Exception inner)
                {
                    throw new Exception("Could not create 'chunks' folder", inner);
                }
            }
        }

        private Process RenderProcessFactory(Chunk chunk)
        {
            var renderCom = new Process();
            var info = new ProcessStartInfo
            {
                FileName = appSettings.BlenderProgram,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format(RenderComARGS,
                                            _rcs.BlendPath,
                                            Path.Combine(_rcs.ChunksPath, _rcs.BaseFileName + "-#"),
                                            appSettings.Renderer,
                                            chunk.Start,
                                            chunk.End),
            };
            renderCom.StartInfo = info;
            renderCom.EnableRaisingEvents = true;
            renderCom.OutputDataReceived += RenderCom_OutputDataReceived;
            renderCom.Exited += (s, e) =>
            {
                // decrement counts when the process exits
                chunksToDo--;
                chunksInProgress--;
            };

            return renderCom;
        }


        private void RenderCom_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                // read blender's output to see what frames are beeing rendered
                if (e.Data.IndexOf("Fra:", StringComparison.InvariantCulture) == 0)
                {
                    var line = e.Data.Split(' ')[0].Replace("Fra:", "");
                    int frameBeingRendered = int.Parse(line);
                    framesRendered.Add(frameBeingRendered);
                }
            }
        }

        private void TryQueueRenderProcess(object sender, ElapsedEventArgs e)
        {
            // start new render procs only within the concurrency limit and until the
            // end of ChunkList
            if (currentIndex < initalChunkCount && chunksInProgress < _rcs.Max)
            {
                var currentChunk = _rcs.Chunks[currentIndex];
                var proc = RenderProcessFactory(currentChunk);
                proc.Start();
                proc.BeginOutputReadLine();
                procBag.Add(proc);

                chunksInProgress++;
                currentIndex++;
            }

            if (canReportProgress)
            {
                var progInfo = new RenderProgressInfo(NumberOfFramesRendered, initalChunkCount - chunksToDo);
                progress.Report(progInfo);
            }

            if (chunksToDo == 0)
            {
                // all render processes are done at this point
                Debug.Assert(NumberOfFramesRendered == _rcs.Chunks.TotalLength(), 
                            "Frames counted don't match the ChunkList lenght");

                timer.Stop();
                OnFinished();
            }
        }

        private void OnFinished()
        {
            Finished?.Invoke(this, NumberOfFramesRendered);
            DisposeProcesses();
            _rcs = null;
        }

        private void DisposeProcesses()
        {
            var procList = procBag.ToList();

            foreach (var p in procList)
            {
                try
                {
                    if (p != null && !p.HasExited)
                    {
                        p.Kill();
                    }
                }
                catch (InvalidOperationException ioex)
                {
                    // Processes may be in an invalid state, just swallow the errors 
                    // since we're diposing them anyway
                    Trace.WriteLine("Error while killing process\n\n" + ioex.Message, nameof(RenderManager));
                }
                finally
                {
                    p.Dispose();
                }
            }

        }
    }

    public class RenderProgressInfo
    {
        public int FramesRendered { get; }
        public int PartsCompleted { get; }

        public RenderProgressInfo(int framesRendered, int partsCompleted)
        {
            FramesRendered = framesRendered;
            PartsCompleted = partsCompleted;
        }
    }
}
