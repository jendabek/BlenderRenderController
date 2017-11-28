using BRClib;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static BRClib.CommandARGS;
using Resources = BlenderRenderController.Properties.Resources;
using Timer = System.Timers.Timer;


namespace BlenderRenderController
{
    /// <summary>
    /// Manages the render process of a list of <see cref="Chunk"/>s.
    /// </summary>
    public class RenderManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        // after render
        Dictionary<string, ProcessResult> _afterRenderReport;
        Task<bool> _arState;
        const string MIX_KEY = "mixdown";
        const string CONCAT_KEY = "concat";
        CancellationTokenSource _arCts;


        public AfterRenderAction Action { get; set; } = AfterRenderAction.NOTHING;

        public TimeSpan Duration { get; set; }

        public string OutputPath { get; set; }

        public string ChunksFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OutputPath)) return null;
                return Path.Combine(OutputPath, Constants.ChunksSubfolder);
            }
        }

        public string BlendFilePath { get; set; }

        public string MixdownAudioFilePath { get; set; }

        public string OutputFileName { get; set; }

        public int MaxConcurrency { get; set; }

        public IReadOnlyList<Chunk> ChunkList { get; set; }

        public int NumberOfFramesRendered => framesRendered.Count;

        public bool InProgress { get => timer.Enabled; }

        public bool WasAborted { get; private set; }

        /// <summary>
        /// Raised when all chunks finish rendering, 'e' is
        /// the total number of frames rendered
        /// </summary>
        public event EventHandler<int> ChunksFinished;

        /// <summary>
        /// Raised when AfterRender actions finish
        /// </summary>
        public event EventHandler AllFinished;

        public event EventHandler<RenderProgressInfo> ProgressChanged;

        public event EventHandler<AfterRenderAction> AfterRenderStarted;


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
            OutputFileName = project.BlendData.ProjectName;
            OutputPath = project.BlendData.OutputPath;
            Duration = project.BlendData.Duration.Value;
        }

        /// <summary>
        /// Starts rendering 
        /// </summary>
        /// <param name="progress">Progress handler</param>
        public void StartAsync(IProgress<RenderProgressInfo> progress)
        {
            // do not start if its already in progress
            if (InProgress)
            {
                Abort();
                throw new InvalidOperationException("A render is already in progress");
            }

            CheckForValidProperties();

            this.progress = progress;
            canReportProgress = progress != null;

            procBag = new ConcurrentBag<Process>();
            framesRendered = new ConcurrentHashSet<int>();
            currentIndex = 0;
            chunksInProgress = 0;
            chunksToDo = ChunkList.Count;
            initalChunkCount = ChunkList.Count;
            _afterRenderReport = new Dictionary<string, ProcessResult>();
            _arCts = new CancellationTokenSource();
            WasAborted = false;

            logger.Info("RENDER STARTING");
            timer.Start();
        }

        /// <summary>
        /// Starts rendering 
        /// </summary>
        public void StartAsync() => StartAsync(null);

        /// <summary>
        /// Aborts the render process
        /// </summary>
        public void Abort()
        {
            if (InProgress)
            {
                timer.Stop();
                WasAborted = true;
                _arCts.Cancel();
                DisposeProcesses();
                logger.Warn("RENDER ABORTED");
            }
        }

        public bool GetAfterRenderResult()
        {
            return _arState.Result;
        }

        private void CheckForValidProperties()
        {
            string[] mustHaveValues = { ChunksFolderPath, BlendFilePath, OutputFileName };

            //mustHaveValues.Where(p => string.IsNullOrWhiteSpace(p))
            //              .Select();

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

        Process CreateRenderProcess(Chunk chunk)
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
                                            Path.Combine(ChunksFolderPath, OutputFileName + "-#"),
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


        bool CreateChunksTxtFile(string chunksFolder)
        {
            // TODO: Find a way to get the videos file ext
            // before rendering ends
            var fileListSorted = Utilities.GetChunkFiles(chunksFolder);

            if (fileListSorted.Count == 0)
            {
                return false;
            }

            string chunksTxtFile = Path.Combine(chunksFolder, Constants.ChunksTxtFileName);

            //write txt for FFmpeg concatenation
            using (StreamWriter partListWriter = new StreamWriter(chunksTxtFile))
            {
                foreach (var filePath in fileListSorted)
                {
                    partListWriter.WriteLine("file '{0}'", filePath);
                }
            }

            return true;
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

        // decrement counts when a process exits, stops the timer when the count 
        // reaches 0
        private void RenderCom_Exited(object sender, EventArgs e)
        {
            --chunksInProgress;
         
            logger.Trace("Render proc exited with code {0}", (sender as Process).ExitCode);

            // check if the overall render is done
            if (Interlocked.Decrement(ref chunksToDo) == 0)
            {
                timer.Stop();

                // all render processes are done at this point
                Debug.Assert(NumberOfFramesRendered == ChunkList.TotalLength(),
                            "Frames counted don't match the ChunkList TotalLenght");

                OnChunksFinished(NumberOfFramesRendered);
            }
        }

        private void TryQueueRenderProcess(object sender, ElapsedEventArgs e)
        {
            // start new render procs only within the concurrency limit and until the
            // end of ChunkList
            if (currentIndex < initalChunkCount && chunksInProgress < MaxConcurrency)
            {
                var currentChunk = ChunkList[currentIndex];
                var proc = CreateRenderProcess(currentChunk);
                proc.Start();
                proc.BeginOutputReadLine();
                procBag.Add(proc);

                chunksInProgress++;
                currentIndex++;

                logger.Trace("Started render n. {0}, frames: {1}", currentIndex, currentChunk);
            }

            ReportProgress(new RenderProgressInfo(NumberOfFramesRendered, initalChunkCount - chunksToDo));
        }

        private void OnChunksFinished(int framesRendered)
        {
            DisposeProcesses();
            ChunksFinished?.Raise(this, framesRendered);
            logger.Info("RENDER FINISHED");

            _arState = Task.Factory.StartNew(() =>
            {
                return AfterRenderProc(Action);
            }, 
            _arCts.Token);

            _arState.ContinueWith(t =>
            {
                AllFinished?.Raise(this, EventArgs.Empty);
            },
            TaskContinuationOptions.ExecuteSynchronously);
        }

        void ReportProgress(RenderProgressInfo progressInfo)
        {
            ProgressChanged?.Raise(this, progressInfo);
            if (canReportProgress) progress.Report(progressInfo);
        }

        private void DisposeProcesses()
        {
            var procList = procBag.ToList();

            foreach (var p in procList)
            {
                try
                {
                    p.Exited -= RenderCom_Exited;
                    p.OutputDataReceived -= RenderCom_OutputDataReceived;

                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
                catch (Exception ex)
                {
                    // Processes may be in an invalid state, just swallow the errors 
                    // since we're diposing them anyway
                    Debug.WriteLine(ex.ToString(), "RenderManager Proc dispose");
                }
                finally
                {
                    p.Dispose();
                }
            }

        }


        string GetMixdownFileName()
        {
            string[] outPathFiles = Directory.GetFiles(OutputPath);
            var exts = RenderFormats.AllowedAudioFileExts.Select(e => '.' + e);
            var fullOutPath = Path.Combine(OutputPath, OutputFileName);

            var res = from fFile in outPathFiles
                      from e in exts
                      let match = Path.ChangeExtension(fullOutPath, e)
                      where File.Exists(match) select match;

            return res.FirstOrDefault();
        }

        string GetRandSulfix(string baseName)
        {
            var tmp = Path.GetRandomFileName();
            return baseName + Path.ChangeExtension(tmp, "txt");
        }


        bool AfterRenderProc(AfterRenderAction action)
        {
            if (action == AfterRenderAction.NOTHING)
            {
                return true;
            }

            AfterRenderStarted.Raise(this, action);

            logger.Info("AfterRender started. Action: {0}", action);

            if ((action & AfterRenderAction.JOIN) != 0)
            {
                // create chunklist.txt
                if (!CreateChunksTxtFile(ChunksFolderPath))
                {
                    // did not create txtFile
                    throw new Exception("Failed to create chunklist.txt");
                }
            }

            // full range of frames
            var fullc = new Chunk(ChunkList.First().Start, ChunkList.Last().End);

            // get the video ext from the rendered chunks name
            var videoExt = Path.GetExtension(Utilities.GetChunkFiles(ChunksFolderPath).First());

            // fix OutputFileName if needed
            if (string.IsNullOrWhiteSpace(OutputFileName))
            {
                OutputFileName = "BrcOutput" + videoExt;
            }
            else if (!Path.HasExtension(OutputFileName))
            {
                OutputFileName += videoExt;
            }

            var projFinalPath = Path.Combine(OutputPath, OutputFileName);
            var chunksTxt = Path.Combine(ChunksFolderPath, Constants.ChunksTxtFileName);


            Process mixdownProc = null, concatProc = null;
            string mixdownArgs = string.Format(MixdownComARGS,
                                                 BlendFilePath,
                                                 fullc.Start, fullc.End,
                                                 Path.Combine(appSettings.ScriptsFolder,
                                                              Constants.PyMixdown),
                                                 OutputPath);

            _afterRenderReport.Add(MIX_KEY, new ProcessResult());
            _afterRenderReport.Add(CONCAT_KEY, new ProcessResult());

            var concatArgs = GetConcatenationArgs(chunksTxt, projFinalPath, Duration, MixdownAudioFilePath);

            if (_arCts.IsCancellationRequested) return false;

            switch (action)
            {
                case AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN:

                    mixdownProc = ProcessFactory.MixdownProcess(AppSettings.Current.BlenderProgram, mixdownArgs);
                    RunProc(ref mixdownProc, MIX_KEY);

                    if (_arCts.IsCancellationRequested) return false;

                    // we're calling this here because the mixdown file must exist for this
                    // check to work
                    if (string.IsNullOrWhiteSpace(MixdownAudioFilePath))
                    {
                        MixdownAudioFilePath = GetMixdownFileName();
                    }

                    concatArgs = GetConcatenationArgs(chunksTxt, projFinalPath, Duration, MixdownAudioFilePath);
                    concatProc = ProcessFactory.ConcatProcess(AppSettings.Current.FFmpegProgram, concatArgs);

                    RunProc(ref concatProc, CONCAT_KEY);

                    break;
                case AfterRenderAction.JOIN:

                    concatProc = ProcessFactory.ConcatProcess(AppSettings.Current.FFmpegProgram, concatArgs);
                    RunProc(ref concatProc, CONCAT_KEY);

                    break;
                case AfterRenderAction.MIXDOWN:

                    mixdownProc = ProcessFactory.MixdownProcess(AppSettings.Current.BlenderProgram, mixdownArgs);
                    RunProc(ref mixdownProc, MIX_KEY);

                    break;
                default:
                    break;
            }

            if (_arCts.IsCancellationRequested) return false;

            // check for bad exit codes
            Process[] processes = { mixdownProc, concatProc };
            var badProcResults = processes.Where(p => p != null && p.ExitCode != 0).ToArray();

            if (badProcResults.Length > 0)
            {
                // create a file report file
                string arReportFile = Path.Combine(OutputPath, GetRandSulfix("AfterRenderReport_"));

                using (var sw = File.AppendText(arReportFile))
                {

                    // do not write reports if exit code was caused by cancellation
                    if (!_arCts.IsCancellationRequested)
                    {
                        if (mixdownProc?.ExitCode != 0)
                        {
                            sw.Write("\n\n");
                            sw.Write("Mixdown ");
                            sw.WriteLine(string.Format(Resources.BadProcResult_Report,
                                mixdownProc.ExitCode, _afterRenderReport[MIX_KEY].StdError, _afterRenderReport[MIX_KEY].StdOutput));
                        }

                        if (concatProc?.ExitCode != 0)
                        {
                            sw.Write("\n\n");
                            sw.Write("FFMpeg concat ");
                            sw.WriteLine(string.Format(Resources.BadProcResult_Report,
                                concatProc.ExitCode, _afterRenderReport[CONCAT_KEY].StdError, _afterRenderReport[CONCAT_KEY].StdOutput));
                        } 
                    }

                }

                return false;
            }
            else
            {
                return !_arCts.IsCancellationRequested;
            }
        }


        void RunProc(ref Process proc, string key)
        {
            proc.Start();

            procBag.Add(proc);

            var readOutput = proc.StandardOutput.ReadToEndAsync();
            var readError = proc.StandardError.ReadToEndAsync();

            readOutput.ContinueWith(t => _afterRenderReport[key].StdOutput = t.Result);
            readError.ContinueWith(t => _afterRenderReport[key].StdError = t.Result);

            proc.WaitForExit();
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
