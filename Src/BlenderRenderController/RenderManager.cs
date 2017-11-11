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
    /// Properties values must remain the same once <see cref="Start"/> is called,
    /// attempting to changing any property while <see cref="InProgress"/> == true
    /// will throw an Exception, you must <seealso cref="Abort"/>
    /// or wait for the process to finish before changing any values
    /// </remarks>
    public class RenderManager
    {
        // State trackers
        private ConcurrentBag<Process> procBag;
        private ConcurrentHashSet<int> framesRendered;
        private int chunksToDo, chunksInProgress,
                    initalChunkCount, currentIndex;

        private Timer timer;

        // Progress stuff
        bool canReportProgress;
        IProgress<RenderProgressInfo> progress;

        AppSettings appSettings = AppSettings.Current;

        private string _chunkPath, _blendPath, _fileName;
        private int _maxC;
        private IReadOnlyList<Chunk> _chunkList;

        public string ChunksFolderPath
        {
            get => _chunkPath;
            set => SetValue(ref _chunkPath, value);
        }
        public string BlendFilePath
        {
            get => _blendPath;
            set => SetValue(ref _blendPath, value);
        }
        public string BaseFileName
        {
            get => _fileName;
            set => SetValue(ref _fileName, value);
        }
        public int MaxConcurrency
        {
            get => _maxC;
            set => SetValue(ref _maxC, value);
        }
        public IReadOnlyList<Chunk> ChunkList
        {
            get => _chunkList;
            set => SetValue(ref _chunkList, value);
        }

        public int NumberOfFramesRendered => framesRendered.Count;

        public bool InProgress { get => timer.Enabled; }

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
            if (InProgress)
            {
                timer.Stop();
                DisposeProcesses();
            }
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
                                            BlendFilePath,
                                            Path.Combine(ChunksFolderPath, BaseFileName + "-#"),
                                            appSettings.Renderer,
                                            chunk.Start,
                                            chunk.End),
            };
            renderCom.StartInfo = info;
            renderCom.EnableRaisingEvents = true;
            renderCom.OutputDataReceived += RenderCom_OutputDataReceived;
            renderCom.Exited += RenderCom_Exited;

            return renderCom;
        }

        // read blender's output to see what frames are beeing rendered
        private void RenderCom_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.IndexOf("Fra:", StringComparison.InvariantCulture) == 0)
                {
                    var line = e.Data.Split(' ')[0].Replace("Fra:", "");
                    int frameBeingRendered = int.Parse(line);
                    framesRendered.Add(frameBeingRendered);
                }
            }
        }

        // decrement counts when a process exits
        private void RenderCom_Exited(object sender, EventArgs e)
        {
            chunksToDo--;
            chunksInProgress--;
        }

        private void TryQueueRenderProcess(object sender, ElapsedEventArgs e)
        {
            // start new render procs only within the concurrency limit and until the
            // end of ChunkList
            if (currentIndex < initalChunkCount && chunksInProgress < MaxConcurrency)
            {
                var currentChunk = ChunkList[currentIndex];
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
                Debug.Assert(NumberOfFramesRendered == ChunkList.TotalLength(),
                            "Frames counted don't match the ChunkList lenght");

                timer.Stop();
                OnFinished();
            }
        }

        private void OnFinished()
        {
            Finished?.Invoke(this, NumberOfFramesRendered);
            DisposeProcesses();
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
                catch (Exception ex)
                {
                    // Processes may be in an invalid state, just swallow the errors 
                    // since we're diposing them anyway
                    Trace.WriteLine("Error while killing process\n\n" + ex.Message, nameof(RenderManager));
                }
                finally
                {
                    p.Dispose();
                }
            }
        }

        // Property values must only change when there isn't a render in progress
        void SetValue<T>(ref T storage, T value)
        {
            if (!InProgress)
            {
                storage = value;
            }
            else
            {
                throw new InvalidOperationException("Cannot change property value while Render is in progress");
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
