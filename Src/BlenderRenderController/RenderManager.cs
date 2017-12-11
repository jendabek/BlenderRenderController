using BlenderRenderController.Infra;
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
    /// <remarks>
    /// Properties values must remain the same once <see cref="Start"/> is called,
    /// attempting to call <see cref="Setup(ProjectSettings)"/> while 
    /// <see cref="InProgress"/> is true will throw an Exception, 
    /// you must <seealso cref="Abort"/> or wait for the process to finish before 
    /// changing any values
    /// </remarks>
    public class RenderManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // State trackers
        private ConcurrentBag<Process> procBag;
        private ConcurrentHashSet<int> framesRendered;
        private int chunksToDo, chunksInProgress,
                    initalChunkCount, currentIndex,
                    maxConcurrency;

        private Timer timer;

        // Progress stuff
        bool canReportProgress;
        IProgress<RenderProgressInfo> progress;
        int _reportCount;
        const int PROG_STACK_SIZE = 3;

        AppSettings appSettings = AppSettings.Current;
        ProjectSettings _proj;

        // after render
        Dictionary<string, ProcessResult> _afterRenderReport;
        Task<bool> _arState;
        const string MIX_KEY = "mixdown";
        const string CONCAT_KEY = "concat";
        CancellationTokenSource _arCts;


        public int NumberOfFramesRendered => framesRendered.Count;

        public bool InProgress { get => timer.Enabled; }

        public bool WasAborted { get; private set; }

        string ChunksFolderPath
        {
            get
            {
                if (_proj == null || string.IsNullOrWhiteSpace(_proj.BlendData.OutputPath)) return null;
                return Path.Combine(_proj.BlendData.OutputPath, Constants.ChunksSubfolder);
            }
        }

        //string OutputFileName
        //{
        //    get
        //    {
        //        if (ChunksFolderPath == null)
        //        {
        //            return null;
        //        }

        //        var videoExt = Path.GetExtension(Utilities.GetChunkFiles(ChunksFolderPath).FirstOrDefault());

        //        return _proj.BlendData.ProjectName + videoExt;
        //    }
        //}

        string MixdownFile
        {
            get
            {
                if (_proj == null || _proj.BlendData.ProjectName == null) return null;

                var mixdownFmt = _proj.BlendData.FFmpegAudioCodec;
                var projName = _proj.BlendData.ProjectName;

                switch (mixdownFmt)
                {
                    case "PCM":
                        return Path.ChangeExtension(projName, "wav");
                    case "VORBIS":
                        return Path.ChangeExtension(projName, "ogg");
                    case null:
                    case "NONE":
                        return Path.ChangeExtension(projName, "ac3");
                    default:
                        return Path.ChangeExtension(projName, mixdownFmt.ToLower());
                }
            }
        }

        public AfterRenderAction Action { get; set; } = AfterRenderAction.NOTHING;

        IReadOnlyList<Chunk> ChunkList;
        SimpleSyncObject _sync = new SimpleSyncObject();


        /// <summary>
        /// Raised when AfterRender actions finish
        /// </summary>
        public event EventHandler Finished;

        public event EventHandler<RenderProgressInfo> ProgressChanged;

        public event EventHandler<AfterRenderAction> AfterRenderStarted;

        
        public RenderManager()
        {
            timer = new Timer
            {
                Interval = 75,
                AutoReset = true,
                SynchronizingObject = _sync,
            };

            timer.Elapsed += TryQueueRenderProcess;
        }

        // for testing
        internal RenderManager(AppSettings settings) : this()
        {
            this.appSettings = settings;
        }

        public RenderManager(ProjectSettings project) : this()
        {
            Setup(project);
        }


        /// <summary>
        /// Setup <see cref="RenderManager"/> using a <see cref="ProjectSettings"/> object
        /// </summary>
        /// <param name="project"></param>
        public void Setup(ProjectSettings project)
        {
            if (InProgress)
            {
                Abort();
                throw new InvalidOperationException("Cannot change settings while a render is in progress!");
            }

            _proj = project;
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

            ResetFields();

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
            //string[] mustHaveValues = { ChunksFolderPath, _proj.BlendPath, OutputFileName };

            //if (mustHaveValues.Any(x => string.IsNullOrWhiteSpace(x)))
            //{
            //    throw new Exception("Required info missing");
            //}

            if (_proj == null)
            {
                throw new Exception("Invalid settings");
            }

            if (_proj.ChunkList.Count == 0)
            {
                throw new Exception("Chunk list is empty");
            }

            if (!File.Exists(_proj.BlendPath))
            {
                throw new FileNotFoundException("Could not find 'blend' file", _proj.BlendPath);
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

        void ResetFields()
        {
            ChunkList = _proj.ChunkList.ToList();
            procBag = new ConcurrentBag<Process>();
            framesRendered = new ConcurrentHashSet<int>();
            currentIndex = 0;
            chunksInProgress = 0;
            chunksToDo = ChunkList.Count;
            initalChunkCount = ChunkList.Count;
            _afterRenderReport = new Dictionary<string, ProcessResult>();
            _arCts = new CancellationTokenSource();
            WasAborted = false;
            _reportCount = 0;
            maxConcurrency = _proj.MaxConcurrency;
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
                                            _proj.BlendPath,
                                            Path.Combine(ChunksFolderPath, _proj.BlendData.ProjectName + "-#"),
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
                    framesRendered.Add(int.Parse(line));
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
                Debug.Assert(framesRendered.ToList().Count == ChunkList.TotalLength(),
                            "Frames counted don't match the ChunkList TotalLenght");

                OnChunksFinished(NumberOfFramesRendered);
            }
        }

        private void TryQueueRenderProcess(object sender, ElapsedEventArgs e)
        {
            // start new render procs only within the concurrency limit and until the
            // end of ChunkList
            if (currentIndex < initalChunkCount && chunksInProgress < maxConcurrency)
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

            ReportProgress(NumberOfFramesRendered, initalChunkCount - chunksToDo);
        }

        private void OnChunksFinished(int framesRendered)
        {
            DisposeProcesses();
            //ChunksFinished?.Raise(this, framesRendered);
            logger.Info("RENDER FINISHED");

            // Send a '100%' ProgressReport
            ReportProgress(NumberOfFramesRendered, 0);

            _arState = Task.Factory.StartNew(() =>
            {
                return AfterRenderProc(Action);
            }, 
            _arCts.Token);

            _arState.ContinueWith(t =>
            {
                OnAllFinished();
            },
            TaskContinuationOptions.ExecuteSynchronously);
        }

        void OnAllFinished()
        {
            Finished?.Raise(this, EventArgs.Empty);
        }


        void ReportProgress(int framesRendered, int chunksCompleted)
        {
            // Stagger report sending to save some heap allocation
            if (_reportCount++ % PROG_STACK_SIZE == 0)
            {
                var progressInfo = new RenderProgressInfo(framesRendered, chunksCompleted);

                ProgressChanged?.Raise(this, progressInfo);
                if (canReportProgress) progress.Report(progressInfo);
            }
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

        string GetRandSulfix(string baseName)
        {
            var tmp = Path.GetRandomFileName();
            return baseName + Path.ChangeExtension(tmp, "txt");
        }


        bool AfterRenderProc(AfterRenderAction action)
        {
            AfterRenderStarted?.Raise(this, action);

            if (action == AfterRenderAction.NOTHING)
            {
                return true;
            }

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

            var videoExt = Path.GetExtension(Utilities.GetChunkFiles(ChunksFolderPath).First());
            var projFinalPath = Path.Combine(_proj.BlendData.OutputPath, _proj.BlendData.ProjectName + videoExt);
            var chunksTxt = Path.Combine(ChunksFolderPath, Constants.ChunksTxtFileName);
            var mixdownPath = Path.Combine(_proj.BlendData.OutputPath, MixdownFile);


            Process mixdownProc = null, concatProc = null;
            string mixdownArgs = string.Format(MixdownComARGS,
                                                 _proj.BlendPath,
                                                 fullc.Start, fullc.End,
                                                 Path.Combine(appSettings.ScriptsFolder,
                                                              Constants.PyMixdown),
                                                 _proj.BlendData.OutputPath);
            string concatArgs;

            _afterRenderReport.Add(MIX_KEY, new ProcessResult());
            _afterRenderReport.Add(CONCAT_KEY, new ProcessResult());


            if (_arCts.IsCancellationRequested) return false;

            switch (action)
            {
                case AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN:

                    mixdownProc = ProcessFactory.MixdownProcess(AppSettings.Current.BlenderProgram, mixdownArgs);
                    RunProc(ref mixdownProc, MIX_KEY);

                    if (_arCts.IsCancellationRequested) return false;

                    // we're creating the args here because the mixdown file
                    // must exist for this to work
                    concatArgs = GetConcatenationArgs(chunksTxt, projFinalPath, _proj.BlendData.Duration, mixdownPath);
                    concatProc = ProcessFactory.ConcatProcess(AppSettings.Current.FFmpegProgram, concatArgs);

                    RunProc(ref concatProc, CONCAT_KEY);

                    break;
                case AfterRenderAction.JOIN:

                    concatArgs = GetConcatenationArgs(chunksTxt, projFinalPath, _proj.BlendData.Duration, mixdownPath);
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
                string arReportFile = Path.Combine(_proj.BlendData.OutputPath, GetRandSulfix("AfterRenderReport_"));

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
                                                        mixdownProc.ExitCode, 
                                                        _afterRenderReport[MIX_KEY].StdError, 
                                                        _afterRenderReport[MIX_KEY].StdOutput));
                        }

                        if (concatProc?.ExitCode != 0)
                        {
                            sw.Write("\n\n");
                            sw.Write("FFMpeg concat ");
                            sw.WriteLine(string.Format(Resources.BadProcResult_Report,
                                                        concatProc.ExitCode, 
                                                        _afterRenderReport[CONCAT_KEY].StdError, 
                                                        _afterRenderReport[CONCAT_KEY].StdOutput));
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
