using BRClib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using static BRClib.CommandARGS;

namespace BlenderRenderController
{

    public class RenderManager
    {
        private ConcurrentBag<Process> procBag;
        private ConcurrentHashSet<int> framesRendered;
        private int chunksToDo, chunksInProgress,
                    initalChunkCount, currentIndex;

        private Timer timer;

        bool canReportProgress;
        IProgress<RenderProgressInfo> progress;
        RenderProgressInfo lastReport;

        AppSettings settings = AppSettings.Current;

        public string ChunksFolderPath { get; set; }
        public string BlendFilePath { get; set; }
        public string BaseFileName { get; set; }
        public int MaxConcurrency { get; set; }
        public int NumberOfFramesRendered { get => framesRendered.Count; }
        public List<Chunk> ChunkList { get; }


        public event EventHandler<int> Finished;


        public RenderManager(IEnumerable<Chunk> chunks)
        {
            ChunkList = new List<Chunk>(chunks);

            timer = new Timer { Interval = 75, AutoReset = true };
            timer.Elapsed += Timer_Elapsed;
        }
        public RenderManager(IEnumerable<Chunk> chunks, AppSettings settings)
            : this(chunks)
        {
            this.settings = settings;
        }
        public RenderManager(ProjectSettings project)
            : this(project.ChunkList)
        {
            MaxConcurrency = project.ProcessesCount;
            BlendFilePath = project.BlendPath;
        }

        public void Start(IProgress<RenderProgressInfo> prog)
        {
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

        private void CheckForValidProperties()
        {
            string[] mustHaveValues = { ChunksFolderPath, BlendFilePath, BaseFileName };

            if (mustHaveValues.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new Exception("Required info missing");
            }

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

        public void Start()
        {
            Start(null);
        }

        public void Abort()
        {
            timer.Stop();
            DisposeProcesses();
        }

        private Process RenderProcessFactory(Chunk chunk)
        {
            var renderCom = new Process();
            var info = new ProcessStartInfo
            {
                FileName = settings.BlenderProgram,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format(RenderComARGS,
                                            BlendFilePath,
                                            Path.Combine(ChunksFolderPath, BaseFileName + "-#"),
                                            settings.Renderer,
                                            chunk.Start,
                                            chunk.End),
            };
            renderCom.StartInfo = info;
            renderCom.EnableRaisingEvents = true;
            renderCom.OutputDataReceived += RenderCom_OutputDataReceived;
            renderCom.Exited += (s, e) =>
            {
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (currentIndex < ChunkList.Count)
            {
                var currentChunk = ChunkList[currentIndex];

                if (chunksInProgress < MaxConcurrency)
                {
                    var proc = RenderProcessFactory(currentChunk);
                    proc.Start();
                    proc.BeginOutputReadLine();
                    procBag.Add(proc);

                    chunksInProgress++;
                    currentIndex++;
                }
            }

            if (canReportProgress)
            {
                var progInfo = new RenderProgressInfo(framesRendered.Count, initalChunkCount - chunksToDo);
                if (lastReport.FramesRendered != progInfo.FramesRendered) progress.Report(progInfo);
                lastReport = progInfo;
            }

            if (chunksToDo == 0)
            {
                Debug.Assert(framesRendered.Count == ChunkList.TotalLength(), "Frames counted don't match the ChunkList lenght");

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
                if (!p.HasExited)
                {
                    p.Kill();
                }
                p.Dispose();
            }
        }
    }

    public struct RenderProgressInfo
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
