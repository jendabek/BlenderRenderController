// For Mono compatible Unix builds, uncomment
// the next line or compile with /d:UNIX
//#define UNIX
#if !WINDOWS && !UNIX
#define WINDOWS
#elif UNIX
#undef WINDOWS
#endif

using BlenderRenderController.Properties;
using BRClib;
#if WINDOWS
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Taskbar;
#endif
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace BlenderRenderController
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class BrcForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        const string timePassedFmt = "Time Elapsed: {0}";
        const string etaFmt = "ETR: {0}";

        private int _autoRefStart, _autoRefEnd;
        private AppSettings _appSettings;
        private ProjectSettings _project;
        private BlendData _blendData;
        private int rendersInProcess, processesCompletedCount, _currentChunkIndex;
        private List<Process> _processList;
        private List<int> framesRendered;
        private AppState appState;
        private DateTime startTime;
        private ETACalculator _etaCalc;
        private Process concatenateCom, mixdownCom;
        private SettingsForm _settingsForm;
        delegate void StatusUpdate(string msg, Control ctrl);


        public bool IsRendering { get; private set; }
        private string AssemblyVersion
        {
            get { return " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }


        public BrcForm()
        {
            InitializeComponent();

            _project = new ProjectSettings();
            _blendData = new BlendData();
            framesRendered = new List<int>();
            _appSettings = AppSettings.Current;
            mixdownCom = new Process();
            concatenateCom = new Process();
        }

        private void BrcForm_Load(object sender, EventArgs e)
        {
            //add version numbers to label
            verToolStripLbl.Text = AssemblyVersion;

            // save appSettings on exit
            AppDomain.CurrentDomain.ProcessExit += (ad, cd) => _appSettings.Save();

            // load recent blends from file
            UpdateRecentBlendsMenu();

            _appSettings.RecentBlends_Changed += AppSettings_RecentBlends_Changed;
            _project.ProcessesCount = Environment.ProcessorCount;

            // set source for project binding
            projectSettingsBindingSource.DataSource = _project;

            // Time duration format
            infoDuration.DataBindings["Text"].Format += (fs, fe) =>
            {
                if (!Convert.IsDBNull(fe.Value) && fe.Value != null)
                {
                    fe.Value = string.Format("{0:%h}h {0:%m}m {0:%s}s {0:%f}ms", (TimeSpan)fe.Value);
                }
            };


            // extra bindings
            totalStartNumericUpDown.DataBindings.Add("Enabled", startEndCustomRadio, "Checked");
            totalEndNumericUpDown.DataBindings.Add("Enabled", startEndCustomRadio, "Checked");

            chunkLengthNumericUpDown.DataBindings.Add("Enabled", renderOptionsCustomRadio, "Checked");
            processCountNumericUpDown.DataBindings.Add("Enabled", renderOptionsCustomRadio, "Checked");



#if UNIX
            forceUIUpdateToolStripMenuItem.Visible = true;
            // TODO: fix ui not updating w/ data binding changes

#endif
        }

        private void BrcForm_Shown(object sender, EventArgs e)
        {
            logger.Info("Program Started");

            if (!_appSettings.CheckCorrectConfig())
            {
                UpdateUI(AppState.NOT_CONFIGURED);

                _settingsForm = new SettingsForm();
                _settingsForm.FormClosed += SettingsForm_FormClosed;

                string errMsg = "One or more required program(s) were not found (Path invalid OR first time run), " +
                                "set the paths in the Settings window";
                string cap = "Setup required";
                string info = "Paths missing";
#if WINDOWS
                var td = new TaskDialog()
                {
                    Caption = cap,
                    InstructionText = info,
                    Text = errMsg,
                    Icon = TaskDialogStandardIcon.Warning,
                    StandardButtons = TaskDialogStandardButtons.Ok
                };

                var tdCmdLink = new TaskDialogCommandLink("BtnOpenSettings", "Goto Settings");
                tdCmdLink.Click += (tdS, tdE) =>
                {
                    _settingsForm.Show();
                    td.Close();
                };

                td.Controls.Add(tdCmdLink);
                td.Show();
#else
                var res = MessageBox.Show(errMsg, cap + " - " + info, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (res == DialogResult.OK)
                {
                    // fix width and show
                    _settingsForm.MaximumSize = _settingsForm.Size;
                    _settingsForm.Show();
                }
#endif
            }
            else
            {
                UpdateUI(AppState.AFTER_START);
            }

        }

        private void BrcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsRendering)
            {
                var result = MessageBox.Show(
                             "Closing now will cancel the rendering process. Close anyway?",
                             "Render in progress",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                    StopRender(false);
            }

            logger.Info("Program closing");
        }

        private void AppSettings_RecentBlends_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateRecentBlendsMenu();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // when closing the Settings window, check if valid
            // and update UI if needed
            if (_appSettings.CheckCorrectConfig())
            {
                // ignore if file is already loaded
                if (appState == AppState.READY_FOR_RENDER)
                    return;

                UpdateUI(AppState.AFTER_START);
            }
            else
                UpdateUI(AppState.NOT_CONFIGURED);

        }


        #region BlendFileInfo
        private void GetBlendInfo(string blendFile, string scriptName = Constants.PyGetInfo)
        {
            // call this if GetBlendInfo fails
            Action ReadFail = () =>
            {
                logger.Error(".blend was NOT loaded");
                UpdateUI(AppState.AFTER_START);
                Status("Error loading blend file");
            };

            logger.Info("Loading .blend");
            Status("Reading .blend file...");

            if (!File.Exists(blendFile))
            {
                MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // remove and update recent blends...
                ReadFail();
                return;
            }
            if (!Directory.Exists(_appSettings.ScriptsFolder))
            {
                // Error scriptsfolder not found
                MessageBox.Show("Scripts folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReadFail();
                return;
            }

            var getBlendInfoCom = new Process();
            var info = new ProcessStartInfo()
            {
                FileName = _appSettings.BlenderProgram,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format(CommandARGS.GetInfoComARGS,
                                            blendFile,
                                            Path.Combine(_appSettings.ScriptsFolder, scriptName))
            };

            getBlendInfoCom.StartInfo = info;

            try
            {
                getBlendInfoCom.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Trace(ex.StackTrace);
                return;
            }

            // Get values from streams
            var fullOutput = getBlendInfoCom.StandardOutput.ReadToEnd();
            var fullErrors = getBlendInfoCom.StandardError.ReadToEnd();
            var streamOutput = new List<string>(fullOutput.Split('\n'));
            var streamErrors = new List<string>(fullErrors.Split('\n'));

            // errors
            if (streamErrors.Count > 0)
            {
                logger.Debug("Blender output errors detected.");
                logger.Trace(fullErrors);

                // folder count exception
                if (streamErrors.Contains(Constants.PY_FolderCountError))
                {
                    var err = Ui.Dialogs.ShowErrorBox("There was an error parsing the output path", 
                        "Read error", 
                        "Script failed to parse the relative output path into an absolute path, try" +
                        "changing the path in your project\n\nError: " + Constants.PY_FolderCountError);

                    err.Show();
                    ReadFail();
                    return;
                }

            }

            if (streamOutput.Count == 0)
            {
                var detailsContent = "Error output: \n\n" + fullErrors;

                var err = Ui.Dialogs.ShowErrorBox("Could not open project, no information was received", 
                                                "Failed to read project", 
                                                "Error", 
                                                detailsContent);
                err.Show();
                ReadFail();
                return;
            }


            var blendData = Utilities.ParsePyOutput(streamOutput);

            if (blendData != null)
            {
                _blendData = blendData;
                _project.BlendPath = blendFile;

                // save copy of start and end frames values
                _autoRefStart = blendData.Start;
                _autoRefEnd = blendData.End;

                // refresh binding source for blend data
                blendDataBindingSource.DataSource = _blendData;
                blendDataBindingSource.ResetBindings(false);

                if (RenderFormats.IMAGES.Contains(blendData.RenderFormat))
                {
                    Helper.ShowErrors(MessageBoxIcon.Asterisk, AppErrorCode.RENDER_FORMAT_IS_IMAGE);
                }

                // output path w/o project name
                if (string.IsNullOrWhiteSpace(_blendData.OutputPath))
                {
                    // use .blend folder path if outputPath is unset, display a warning about it
                    Helper.ShowErrors(MessageBoxIcon.Information, AppErrorCode.BLEND_OUTPUT_INVALID);
                    _blendData.OutputPath = Path.GetDirectoryName(blendFile);
                }
                else
                    _blendData.OutputPath = Path.GetDirectoryName(_blendData.OutputPath);


                logger.Info(".blend loaded successfully");
            }
            else
            {
                //var detailContents = string.Format("# STD output:\n\n{0}\n\n# STD errors:\n\n{1}", fullOutput, fullErrors);
                var errorBox = Ui.Dialogs.ShowErrorBox("Failed to read blend file info.", 
                    "Read error", "Error output:\n\n" + fullErrors);

                errorBox.Show();
                ReadFail();

                return;
            }

            var chunks = Chunk.CalcChunks(blendData.Start, blendData.End, _project.ProcessesCount);
            UpdateCurrentChunks(chunks);
            _appSettings.AddRecentBlend(blendFile);
            Status("File loaded");
            UpdateUI(AppState.READY_FOR_RENDER);
        }


        private void blendBrowseBtn_Click(object sender, EventArgs e)
        {
            var result = openBlendDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var blend = openBlendDialog.FileName;
                GetBlendInfo(blend);
            }
        }
        
        private void reloadBlenderDataButton_Click(object sender, EventArgs e)
        {
            var blend = _project.BlendPath;
            if (!string.IsNullOrEmpty(blend))
            {
                GetBlendInfo(blend);
            }
        }

        private void showRecentBlendsBtn_Click(object sender, EventArgs e)
        {
            var recentBtn = sender as Button;
            recentBlendsMenu.Show(recentBtn, 0, recentBtn.Height);
        }
        #endregion

        #region RenderMethods
        private void RenderChunk(Chunk chunk)
        {
            var chunksFolder = Path.Combine(_blendData.OutputPath, Constants.ChunksSubfolder);

            var renderCom = new Process();
            var info = new ProcessStartInfo();
            info.FileName = _appSettings.BlenderProgram;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.Arguments = string.Format(CommandARGS.RenderComARGS,
                            _project.BlendPath,
                            Path.Combine(chunksFolder, _blendData.ProjectName + "-#"),
                            _appSettings.Renderer,
                            chunk.Start,
                            chunk.End);

            renderCom.StartInfo = info;
            renderCom.EnableRaisingEvents = true;
            renderCom.OutputDataReceived += OnRenderProcessDataRecived;
            renderCom.Exited += (pSender, pArgs) =>
            {
                // updates the counts
                _processList.Remove((Process)pSender);
                rendersInProcess--;
                processesCompletedCount++;
            };

            try
            {
                renderCom.Start();
                rendersInProcess++;
                _processList.Add(renderCom);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                logger.Error(ex.Message);
                StopRender(false);
                return;
            }

            renderCom.BeginOutputReadLine();
            renderCom.BeginErrorReadLine();
        }

        private void RenderAll()
        {
            // Calculate chunks
            if (renderOptionsCustomRadio.Checked)
            {
                var customChunks = Chunk.CalcChunksByLenght(_blendData.Start, _blendData.End, _project.ChunkLenght);
                UpdateCurrentChunks(customChunks);
            }
            else
            {
                var autoChunks = Chunk.CalcChunks(_blendData.Start, _blendData.End, _project.ProcessesCount);
                UpdateCurrentChunks(autoChunks);
            }

            startTime = DateTime.Now;
            framesRendered.Clear();
            Status("Starting render...");
            processesCompletedCount = 0;
            rendersInProcess = 0;
            _currentChunkIndex = 0;
            IsRendering = true;
            _processList = new List<Process>(_project.ProcessesCount);

            // render progress reset
            totalTimeLabel.Text = string.Format(timePassedFmt, Helper.SecondsToString(0, true));
            _etaCalc = new ETACalculator(5, 1);

#if WINDOWS
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            TaskbarManager.Instance.SetProgressValue(0, 100);
#endif

            processManager.Enabled = true;

            UpdateUI(AppState.RENDERING_ALL);
        }

        private void StopRender(bool wasComplete)
        {
            var msg = wasComplete ? "Render complete." : "Render cancelled.";
            Status(msg);

            foreach (var process in _processList.ToList())
            {
                try
                {
                    if (process != null && !process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Error("An error ocurred while killing processes: " + ex.Message);
                    logger.Trace(ex.StackTrace);
                    //Trace.WriteLine(ex);
                }
            }

            _processList.Clear();

            processManager.Enabled = false;
            IsRendering = false;
            renderProgressBar.Style = ProgressBarStyle.Blocks;
            renderProgressBar.Value = 0;
            renderProgressBar.Refresh();

            ETALabel.Text = string.Format(etaFmt, Helper.SecondsToString(0, true));
            totalTimeLabel.Text = string.Format(timePassedFmt, Helper.SecondsToString(0, true));

            Text = Constants.APP_TITLE;

#if WINDOWS
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
#endif

            UpdateUI(AppState.READY_FOR_RENDER);
        }

        private void renderAllButton_Click(object sender, EventArgs e)
        {
            if (IsRendering)
            {
                var result = MessageBox.Show("Are you sure you want to stop?",
                                                "",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Exclamation);

                if (result == DialogResult.No)
                    return;

                StopRender(false);
                logger.Warn("RENDER ABORTED");
            }
            else
            {
                var chunksFolder = Path.Combine(_blendData.OutputPath, Constants.ChunksSubfolder);

                if (Directory.Exists(chunksFolder) && Directory.GetFiles(chunksFolder).Length > 0)
                {
                    var dialogResult = MessageBox.Show("All previously rendered chunks will be deleted.\nDo you want to continue?",
                                            "Chunks folder not empty",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Exclamation);

                    if (dialogResult == DialogResult.No)
                        return;

                    try
                    {
                        Helper.ClearFolder(chunksFolder);
                    }
                    catch (IOException)
                    {
                        string msg = "Can't clear chunk folder, files are in use";
                        logger.Error(msg);
                        MessageBox.Show(msg);
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        logger.Trace(ex.StackTrace);
                        MessageBox.Show("An unexpected error ocurred, sorry.\n\n" + ex.Message);
                        return;
                    }

                }

                RenderAll();
                logger.Info("RENDER STARTED");
            }


        }

        /// <summary>
        /// <see cref="processManager"/> ticks calls this method and it controls what
        /// processes get started for rendering, when to call <see cref="AfterRenderBG"/>
        /// and updates the progress to the user
        /// </summary>
        private void TryQueueRenderProcess(object sender, EventArgs e)
        {
            try
            {
                Chunk currentChunk;
                if (_currentChunkIndex < _project.ChunkList.Count)
                {
                    currentChunk = _project.ChunkList[_currentChunkIndex];

                    if (_processList.Count < _processList.Capacity)
                    {
                        RenderChunk(currentChunk);
                        _currentChunkIndex++;
                    }
                }

                UpdateProgress();

                // do after render once all processes exits
                if (_processList.Count == 0)
                {
                    renderProgressBar.Style = ProgressBarStyle.Marquee;
#if WINDOWS
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
#endif
                    processManager.Enabled = false;
                    AfterRenderBG();
                }

            }
            catch (Exception)
            {
                // if anything goes wrong, make rendering stop
                StopRender(false);
                throw;
            }
        }

        private void OnRenderProcessDataRecived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.IndexOf("Fra:", StringComparison.InvariantCulture) == 0)
                {
                    int frameBeingRendered = int.Parse(e.Data.Split(' ')[0].Replace("Fra:", ""));
                    var frame = e.Data.Split(' ')[0].Replace("Fra:", "");
                    if (!framesRendered.Contains(frameBeingRendered))
                    {
                        framesRendered.Add(frameBeingRendered);
                    }

                }
            }
        }

        #endregion

        #region UpdateElements
        /// <summary>
        /// Updates the UI on the render process
        /// </summary>
        private void UpdateProgress()
        {

            int progressPercentage = (int)Math.Floor((framesRendered.Count / (decimal)_blendData.TotalFrames) * 100);
            var chunksTotalCount = Math.Ceiling((decimal)_blendData.TotalFrames / _project.ChunkLenght);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Completed {0} / {1}", processesCompletedCount, chunksTotalCount);
            sb.AppendFormat(" chunks, rendered {0} frames in {1} processes", framesRendered.Count, rendersInProcess);
            Status(sb.ToString());

#if WINDOWS
            // taskbar progress
            TaskbarManager.Instance.SetProgressValue(progressPercentage, 100);
#endif

            //progress bar
            if (progressPercentage > 100)
                throw new Exception("Progress is over 100%");

            else
                renderProgressBar.Value = progressPercentage;

            _etaCalc.Update(progressPercentage / 100f);

            if (_etaCalc.ETAIsAvailable)
            {
                var etr = string.Format(etaFmt, Helper.SecondsToString(_etaCalc.ETR.TotalSeconds, true));
                Status(etr, ETALabel);
            }

            //time elapsed display
            TimeSpan runTime = DateTime.Now - startTime;
            var tElapsed = string.Format(timePassedFmt, Helper.SecondsToString(runTime.TotalSeconds, true));
            Status(tElapsed, totalTimeLabel);

            //title progress
            var titleProg = progressPercentage.ToString() + "% rendered - " + Constants.APP_TITLE;
            Status(titleProg, this);
        }

        /// <summary>
        /// Updates the list of chunks that will be rendered
        /// </summary>
        /// <param name="newChunks"></param>
        private void UpdateCurrentChunks(params Chunk[] newChunks)
        {
            bool ignore = (newChunks.TotalLength() > _blendData.TotalFrames) 
                        || newChunks.SequenceEqual(_project.ChunkList);

            if (ignore)
                return;

            if (_project.ChunkList.Count > 0)
                _project.ChunkList.Clear();

            foreach (var chnk in newChunks)
            {
                _project.ChunkList.Add(chnk);
            }

            _project.ChunkLenght = (int)_project.ChunkList.First().Length;

            logger.Debug("ChunkLenght: " + _project.ChunkLenght);
            logger.Trace(string.Join(", " ,_project.ChunkList));

#if UNIX
            ForceBindedElementsUpdate(null, null);
#endif

        }

        private void UpdateRecentBlendsMenu()
        {
            // clear local blends
            recentBlendsMenu.Items.Clear();

            // make blends from recent list
            foreach (string item in _appSettings.RecentBlends)
            {
                var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(item), Resources.blend_icon);
                menuItem.ToolTipText = item;
                menuItem.Click += RecentBlendsItem_Click;
                recentBlendsMenu.Items.Add(menuItem);
            }
        }

        private void RecentBlendsItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem recentItem = (ToolStripMenuItem)sender;
            var blendPath = recentItem.ToolTipText;
            GetBlendInfo(blendPath);
        }

        /// <summary>
        /// Thread safe method to change UI text
        /// </summary>
        /// <param name="msg">new text</param>
        /// <param name="ctrl">Control to update, <seealso cref="statusLabel"/> by default</param>
        private void Status(string msg, Control ctrl = null)
        {
            if (ctrl == null)
                ctrl = statusLabel;

            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new StatusUpdate(Status), msg, statusLabel);
            }
            else
            {
                ctrl.Text = msg;
                statusLabel.Refresh();
            }
        }
        #endregion

        #region Mixdown
        private void MixdownAudio(string mixdownScript = Constants.PyMixdown)
        {
            Status("Rendering mixdown, it can take a while for larger projects...");
            logger.Info("Mixdown started");

            if (!File.Exists(_project.BlendPath))
                return;

            if (!Directory.Exists(_appSettings.ScriptsFolder))
            {
                // Error scriptsfolder not found
                string caption = "Error";
                string msg = "Scripts folder not found. Separate audio mixdown and automatic project info " +
                    "detection will not work, but you can still use the basic rendering functionality.";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!Directory.Exists(_blendData.OutputPath))
            {
                Directory.CreateDirectory(_blendData.OutputPath);
            }

#region Mixdown Commad
            //var mixdownCom = new Process();
            var info = new ProcessStartInfo()
            {
                FileName = _appSettings.BlenderProgram,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format(CommandARGS.MixdownComARGS,
                                           _project.BlendPath,
                                           _blendData.Start,
                                           _blendData.End,
                                           Path.Combine(_appSettings.ScriptsFolder, mixdownScript),
                                           _blendData.OutputPath)
            };
            mixdownCom.StartInfo = info;
            mixdownCom.EnableRaisingEvents = true;
#endregion

            Trace.WriteLine(mixdownCom.StartInfo.Arguments);

            try
            {
                mixdownCom.Start();
            }
            catch (Exception ex)
            {
                Status("Mixdown cancelled.");
                string errText = string.Format("An unexpected error occurred: '{0}'", ex.Message);
                string errInfo = "Mixdown failed";
#if WINDOWS
                var err = new TaskDialog()
                {
                    Text = errText,
                    InstructionText = errInfo,
                    DetailsExpanded = false,
                    DetailsExpandedText = "StackTrace:\n\n" + ex.StackTrace,
                    Icon = TaskDialogStandardIcon.Error
                }; 
#else
                MessageBox.Show(errText, errInfo, MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                return;
            }

            mixdownCom.WaitForExit();

            string message = "Mixdown complete";
            logger.Info(message);
            Status(message);
        }

        private async void mixDownButton_Click(object sender, EventArgs e)
        {
            renderProgressBar.Style = ProgressBarStyle.Marquee;
            await Task.Run(() => MixdownAudio());
            renderProgressBar.Style = ProgressBarStyle.Blocks;
        }
        #endregion

        #region Concatenate
        private void Concatenate()
        {
            var chunksFolder = Path.Combine(_blendData.OutputPath, Constants.ChunksSubfolder);

            if (!Directory.Exists(chunksFolder))
            {
                concatenatePartsButton.Enabled = false;
                return;
            }

            string chunksTxtPath = Path.Combine(chunksFolder, Constants.ChunksTxtFileName);
            string audioFileName = _blendData.ProjectName + "." + "ac3";
            string projPath = Path.Combine(_blendData.OutputPath, _blendData.ProjectName);
            string mixdownAudioPath = Path.Combine(_blendData.OutputPath, audioFileName);

            var fileListSorted = Utilities.GetChunkFiles(chunksFolder);

            if (fileListSorted.Count == 0)
            {
                MessageBox.Show("Failed to get chunk files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            var videoExtensionFound = Path.GetExtension(fileListSorted.FirstOrDefault());

            //write txt for FFmpeg concatenation
            using (StreamWriter partListWriter = new StreamWriter(chunksTxtPath))
            {
                foreach (var filePath in fileListSorted)
                {
                    partListWriter.WriteLine("file '{0}'", filePath);
                }
            }

            string comArgs;

            //mixdown audio NOT found
            if (!File.Exists(mixdownAudioPath))
            {
                Status("Joining chunks, please wait...");
                comArgs = string.Format(CommandARGS.ConcatenateOnly,
                            chunksTxtPath,
                            projPath,
                            videoExtensionFound);
            }
            //mixdown audio found
            else
            {
                Status("Joining chunks with mixdown audio...");
                comArgs = string.Format(CommandARGS.ConcatenateMixdown,
                            chunksTxtPath,
                            mixdownAudioPath,
                            projPath,
                            videoExtensionFound);
            }

            //var concatenateCom = new Process();
            var info = new ProcessStartInfo();
            info.FileName = _appSettings.FFmpegProgram;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.Arguments = comArgs;
            concatenateCom.StartInfo = info;
            concatenateCom.EnableRaisingEvents = true;
            concatenateCom.Exited += (pSender, pArgs) =>
            {
                logger.Info("FFmpeg exited");
            };

            Trace.WriteLine(concatenateCom.StartInfo.Arguments);

            try
            {
                logger.Info(statusLabel.Text);
                concatenateCom.Start();
            }
            catch (Exception ex)
            {
                //Trace.WriteLine(ex);
                logger.Error("Unexpected error at {0}. Msg: {1}", nameof(Concatenate), ex.Message);
                logger.Trace(ex.StackTrace);
                statusLabel.Text = "Joining cancelled.";
                return;
            }
            concatenateCom.WaitForExit();
            var msg = "Chunks Joined.";
            Status(msg);
            logger.Info(msg);
        }

        private async void concatenatePartsButton_Click(object sender, EventArgs e)
        {
            renderProgressBar.Style = ProgressBarStyle.Marquee;
            await Task.Run(() => Concatenate());
            renderProgressBar.Style = ProgressBarStyle.Blocks;
        }
        #endregion

        #region AfterRenderActions
        /// <summary>
        /// Checks if rendering process is completed and calls the after
        /// render actions in a background thread
        /// </summary>
        void AfterRenderBG()
        {
            bool wasComplete = (framesRendered.Count > Math.Round(Convert.ToDouble(_blendData.TotalFrames)) * 0.75);

            if (wasComplete)
            {
                afterRenderBGWorker.RunWorkerAsync();
            }
            else
                StopRender(false);

        }

        private void AfterRenderBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // does all after render actions in a bg thread
            if (_appSettings.AfterRender == AfterRenderAction.JOIN_MIXDOWN)
            {
                MixdownAudio();
                Concatenate();
            }
            else if (_appSettings.AfterRender == AfterRenderAction.JOIN)
                Concatenate();

        }

        private void AfterRenderBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopRender(true);

            var openOutputFolderQuestion =
                    MessageBox.Show("Open the folder with the video?",
                                    "Work complete!",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

            if (openOutputFolderQuestion == DialogResult.Yes)
                OpenOutputFolder();

            UpdateUI(appState);
        }
        #endregion


        private void Enter_GotoNext(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
                e.SuppressKeyPress = true; //disables sound
            }

        }

        private void openOutputFolderButton_Click(object sender, EventArgs e)
        {
            OpenOutputFolder();
        }

        private void OpenOutputFolder()
        {
            if (Directory.Exists(_blendData.OutputPath))
            {
                Process.Start(_blendData.OutputPath);
            }
            else
            {
                MessageBox.Show("Output folder does not exist.", "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }

        }

        private void AutoOptionsRadio_CheckedChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;

            if (radio.Name == renderOptionsAutoRadio.Name)
            {
                if (radio.Checked)
                {
                    _project.ProcessesCount = Environment.ProcessorCount;
                    // recalc auto chunks:
                    var currentStart = totalStartNumericUpDown.Value;
                    var currentEnd = totalEndNumericUpDown.Value;
                    var currentProcessors = (int)processCountNumericUpDown.Value;

                    var autoChunks = Chunk.CalcChunks(currentStart, currentEnd, currentProcessors);
                    UpdateCurrentChunks(autoChunks);
                }
            }
            else if (radio.Name == startEndBlendRadio.Name)
            {
                if (radio.Checked)
                {
                    // set to blend values
                    _blendData.Start = _autoRefStart;
                    _blendData.End = _autoRefEnd;

#if UNIX
                    ForceBindedElementsUpdate(null, null);
#endif
                }
            }
        }

        private void AfterRenderAction_Changed(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;

            if (radio.Checked)
            {
                if (radio.Name == afterRenderJoinMixdownRadio.Name)
                    _appSettings.AfterRender = AfterRenderAction.JOIN_MIXDOWN;

                else if (radio.Name == afterRenderJoinRadio.Name)
                    _appSettings.AfterRender = AfterRenderAction.JOIN;

                else if (radio.Name == afterRenderDoNothingRadio.Name)
                    _appSettings.AfterRender = AfterRenderAction.NOTHING;

            }
        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {

#if WINDOWS
            var folderPicker = new CommonOpenFileDialog
            {
                InitialDirectory = _blendData.OutputPath,
                IsFolderPicker = true,
                Title = "Select output location",

            };

            var result = folderPicker.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                _blendData.OutputPath = folderPicker.FileName;
                UpdateUI();
            }
#else
            var folderPicker = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = _blendData.OutputPath,
                ShowNewFolderButton = true
            };
            var result = folderPicker.ShowDialog();

            if (result == DialogResult.OK)
            {
                _blendData.OutputPath = folderPicker.SelectedPath;
                UpdateUI();

                blendDataBindingSource.ResetBindings(false);
            }
#endif
        }

        private void StartEndNumeric_Changed(object sender, EventArgs e)
        {
            if (renderOptionsAutoRadio.Checked)
            {
                var currentStart = totalStartNumericUpDown.Value;
                var currentEnd = totalEndNumericUpDown.Value;
                var currentProcessors = (int)processCountNumericUpDown.Value;

                if (currentEnd <= currentStart || currentProcessors == 0)
                    return;

                var autoChunks = Chunk.CalcChunks(currentStart, currentEnd, currentProcessors);
                UpdateCurrentChunks(autoChunks);
            }
        }

        private void RendererType_RadioChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;

            if (radio.Checked)
            {
                if (radio.Name == rendererRadioButtonBlender.Name)
                    _appSettings.Renderer = BlenderRenderes.BLENDER_RENDER;

                else
                    _appSettings.Renderer = BlenderRenderes.CYCLES;
            }
        }

        private void AuthorLink_Clicked(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            string link;

            switch (item.Name)
            {
                case nameof(isti115MenuItem):
                    link = "https://github.com/Isti115/BlenderRenderController";
                    break;
                case nameof(jendabekMenuItem):
                    link = "https://github.com/jendabek/BlenderRenderController";
                    break;
                case nameof(meTwentyFiveMenuItem):
                    link = "https://github.com/MeTwentyFive/BlenderRenderController";
                    break;
                case nameof(redRaptorMenuItem):
                    link = "https://github.com/RedRaptor93/BlenderRenderController";
                    break;
                default:
                    return;
            }

            Process.Start(link);
        }

        private void tipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolTipInfo.Active =
            toolTipWarn.Active =
            _appSettings.DisplayToolTips = tipsToolStripMenuItem.Checked;
        }

        private void donateButton_Click(object sender, EventArgs e)
        {
            string business = "jendabek@gmail.com";  // your paypal email
            string description = "Donation for Blender Render Controller";
            string country = "CZE";                  // AU, US, etc.
            string currency = "USD";                 // AUD, USD, etc.

            string url = "https://www.paypal.com/cgi-bin/webscr" +
                    "?cmd=" + "_donations" +
                    "&business=" + business +
                    "&lc=" + country +
                    "&item_name=" + description +
                    "&currency_code=" + currency +
                    "&bn=" + "PP%2dDonationsBF";

            Process.Start(url);
        }

        private void ForceBindedElementsUpdate(object sender, EventArgs e)
        {
            // Mono's WinForm doesn't update the UI elements properly, 
            // so do it manually
            blendDataBindingSource.ResetBindings(false);
            projectSettingsBindingSource.ResetBindings(false);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _settingsForm = new SettingsForm();
            _settingsForm.FormClosed += SettingsForm_FormClosed;
            _settingsForm.ShowDialog();
        }

        private void toolStripMenuItemBug_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/jendabek/BlenderRenderController/issues");
        }

        private void clearRecentProjectsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _appSettings.ClearRecentBlend();
        }

        /// <summary>
        /// Updates UI according to <see cref="AppState"/>
        /// </summary>
        /// <param name="newState">New state, leave empty to refresh using the current state</param>
        private void UpdateUI(AppState? newState = null)
        {
            appState = newState ?? appState;

            renderAllButton.Text = (appState == AppState.RENDERING_ALL)
                                    ? "Stop Render"
                                    : "Start Render";

            renderAllButton.Image = (appState == AppState.RENDERING_ALL)
                                    ? Resources.stop_icon_small
                                    : Resources.render_icon_small;

            // the world's longest switch block!
            // enabling / disabling UI according to current app state
            switch (appState)
            {
                case AppState.AFTER_START:
                    blendNameLabel.Visible = false;
                    renderAllButton.Enabled = false;
                    menuToolStrip.Enabled = true;
                    blendBrowseBtn.Enabled = true;
                    showRecentBlendsBtn.Enabled = true;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    openOutputFolderButton.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    ETALabel.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    startEndBlendRadio.Enabled = false;
                    startEndCustomRadio.Enabled = false;
                    renderOptionsCustomRadio.Enabled = false;
                    renderOptionsAutoRadio.Enabled = false;
                    afterRenderDoNothingRadio.Enabled = false;
                    afterRenderJoinMixdownRadio.Enabled = false;
                    afterRenderJoinRadio.Enabled = false;
                    renderInfoLabel.Enabled = false;
                    Status("Select a file");
                    break;
                case AppState.NOT_CONFIGURED:
                    renderAllButton.Enabled = false;
                    menuToolStrip.Enabled = true;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    blendBrowseBtn.Enabled = false;
                    showRecentBlendsBtn.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    openOutputFolderButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    ETALabel.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    startEndBlendRadio.Enabled = false;
                    startEndCustomRadio.Enabled = false;
                    renderOptionsCustomRadio.Enabled = false;
                    renderOptionsAutoRadio.Enabled = false;
                    afterRenderDoNothingRadio.Enabled = false;
                    afterRenderJoinMixdownRadio.Enabled = false;
                    afterRenderJoinRadio.Enabled = false;
                    renderInfoLabel.Enabled = false;
                    Status("Required program(s) not found, see Settings");
                    break;
                case AppState.READY_FOR_RENDER:
                    blendNameLabel.Visible = true;
                    renderAllButton.Enabled = true;
                    menuToolStrip.Enabled = true;
                    mixDownButton.Enabled = true;
                    totalStartNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    totalEndNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    chunkLengthNumericUpDown.Enabled = renderOptionsCustomRadio.Checked;
                    processCountNumericUpDown.Enabled = renderOptionsCustomRadio.Checked;
                    var chunksFolder = Path.Combine(_blendData.OutputPath, Constants.ChunksSubfolder);
                    concatenatePartsButton.Enabled = Directory.Exists(chunksFolder);
                    reloadBlenderDataButton.Enabled = true;
                    blendBrowseBtn.Enabled = true;
                    showRecentBlendsBtn.Enabled = true;
                    outputFolderBrowseButton.Enabled = true;
                    outputFolderTextBox.Enabled = true;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    ETALabel.Visible = true;
                    totalTimeLabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    startEndBlendRadio.Enabled = true;
                    startEndCustomRadio.Enabled = true;
                    renderOptionsCustomRadio.Enabled = true;
                    renderOptionsAutoRadio.Enabled = true;
                    afterRenderDoNothingRadio.Enabled = true;
                    afterRenderJoinMixdownRadio.Enabled = true;
                    afterRenderJoinRadio.Enabled = true;
                    renderInfoLabel.Enabled = true;
                    Status("Ready");
                    break;
                case AppState.RENDERING_ALL:
                    menuToolStrip.Enabled = false;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    blendBrowseBtn.Enabled = false;
                    showRecentBlendsBtn.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    ETALabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = false;
                    rendererRadioButtonCycles.Enabled = false;
                    renderOptionsCustomRadio.Enabled = false;
                    renderOptionsAutoRadio.Enabled = false;
                    startEndBlendRadio.Enabled = false;
                    startEndCustomRadio.Enabled = false;
                    afterRenderDoNothingRadio.Enabled = false;
                    afterRenderJoinMixdownRadio.Enabled = false;
                    afterRenderJoinRadio.Enabled = false;
                    break;
            }

        }

    }

}
