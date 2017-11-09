// For Mono compatible Unix builds compile with /d:UNIX
#if !WINDOWS && !UNIX
#error You must define a platform (WINDOWS or UNIX)
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
using System.Threading;
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

        const string TimePassedPrefix = "Time Elapsed: ",
                     ETR_Prefix = "ETR: ",
                     TimeFmt = @"hh\:mm\:ss";

        private AppSettings _appSettings;
        private ProjectSettings _project;
        private RenderManager renderManager;
        private Progress<RenderProgressInfo> renderProgress;
        private int _autoRefStart, _autoRefEnd;
        private AppState appState;
        private DateTime startTime;
        private ETACalculator _etaCalc;
        private SettingsForm _settingsForm;
        CancellationTokenSource afterRenderCancelSrc;

        public bool IsWorking { get; private set; }


        public BrcForm()
        {
            InitializeComponent();

            _project = new ProjectSettings();
            _project.BlendData = new BlendData();
            _appSettings = AppSettings.Current;
#if WINDOWS
            // set the form icon outside designer
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
#endif
        }

        private void BrcForm_Load(object sender, EventArgs e)
        {
            //add version numbers to label
            verToolStripLbl.Text = " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

            // save appSettings on exit
            AppDomain.CurrentDomain.ProcessExit += (ad, cd) => _appSettings.Save();

            // load recent blends from file
            UpdateRecentBlendsMenu();

            _appSettings.RecentBlends_Changed += AppSettings_RecentBlends_Changed;

            processCountNumericUpDown.Maximum = 
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

            infoActiveScene.TextChanged += (s, args) => toolTipInfo.SetToolTip(infoActiveScene, infoActiveScene.Text);
#if UNIX
            forceUIUpdateToolStripMenuItem.Visible = true;
            totalStartNumericUpDown.EnabledChanged += NumericUpDown_EnableChanged;
            totalEndNumericUpDown.EnabledChanged += NumericUpDown_EnableChanged;
            chunkLengthNumericUpDown.EnabledChanged += NumericUpDown_EnableChanged;
            processCountNumericUpDown.EnabledChanged += NumericUpDown_EnableChanged;
#endif
        }

#if UNIX
        private void NumericUpDown_EnableChanged(object sender, EventArgs e)
        {
            // Work around Mono not gray-ing out if disabled
            var numeric = sender as NumericUpDown;
            numeric.BackColor = (numeric.Enabled)
                                ? System.Drawing.Color.White
                                : System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
            //numeric.Invalidate();
        }
#endif

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
            if (IsWorking)
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
                {
                    StopWork(false);
                }
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
        private async void GetBlendInfo(string blendFile)
        {

            if (!File.Exists(blendFile))
            {
                MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

            renderProgressBar.Style = ProgressBarStyle.Marquee;
            logger.Info("Loading .blend");
            Status("Reading .blend file...");

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
                                            Path.Combine(_appSettings.ScriptsFolder, Constants.PyGetInfo))
            };

            getBlendInfoCom.StartInfo = info;

            // exec process asynchronously
            var (exitCode, stdOutput, stdErrors) = await getBlendInfoCom.StartAsyncGetOutput();

            // errors
            if (stdErrors.Length > 0)
            {
                logger.Debug("Blender output errors detected.");
                logger.Trace(stdErrors);

                // detect folder count exception
                if (stdErrors.Contains(Constants.PY_FolderCountError))
                {
                    var err = Ui.Dialogs.ShowErrorBox("There was an error parsing the output path", 
                        "Read error", 
                        "Script failed to parse the relative output path into an absolute path, try " +
                        "changing the path in your project\n\nError: " + Constants.PY_FolderCountError);

                    err.Show();
                    ReadFail();
                    return;
                }

            }

            if (stdOutput.Length == 0)
            {
                var detailsContent = "Error output: \n\n" + stdErrors;

                var err = Ui.Dialogs.ShowErrorBox("Could not open project, no information was received", 
                                                "Failed to read project", 
                                                "Error", 
                                                detailsContent);
                err.Show();
                ReadFail();
                return;
            }


            var blendData = Utilities.ParsePyOutput(stdOutput);

            if (blendData != null)
            {
                _project.BlendData = blendData;
                _project.BlendPath = blendFile;

                // save copy of start and end frames values
                _autoRefStart = blendData.Start;
                _autoRefEnd = blendData.End;

                // refresh binding source for blend data
                blendDataBindingSource.DataSource = _project.BlendData;
                blendDataBindingSource.ResetBindings(false);

                if (RenderFormats.IMAGES.Contains(blendData.FileFormat))
                {
                    Helper.ShowErrors(MessageBoxIcon.Asterisk, AppErrorCode.RENDER_FORMAT_IS_IMAGE);
                }

                // output path w/o project name
                if (string.IsNullOrWhiteSpace(_project.BlendData.OutputPath))
                {
                    // use .blend folder path if outputPath is unset, display a warning about it
                    Helper.ShowErrors(MessageBoxIcon.Information, AppErrorCode.BLEND_OUTPUT_INVALID);
                    _project.BlendData.OutputPath = Path.GetDirectoryName(blendFile);
                }
                else
                    _project.BlendData.OutputPath = Path.GetDirectoryName(_project.BlendData.OutputPath);


                logger.Info(".blend loaded successfully");
            }
            else
            {
                //var detailContents = string.Format("# STD output:\n\n{0}\n\n# STD errors:\n\n{1}", fullOutput, fullErrors);
                var errorBox = Ui.Dialogs.ShowErrorBox("Failed to read blend file info.", 
                    "Read error", "Error output:\n\n" + stdErrors);

                errorBox.Show();
                ReadFail();
                return;
            }

            var chunks = Chunk.CalcChunks(blendData.Start, blendData.End, _project.ProcessesCount);
            UpdateCurrentChunks(chunks);
            _appSettings.AddRecentBlend(blendFile);
            UpdateUI(AppState.READY_FOR_RENDER);
            renderProgressBar.Style = ProgressBarStyle.Blocks;
            // ---

            // call this if GetBlendInfo fails
            void ReadFail()
            {
                logger.Error(".blend was NOT loaded");
                UpdateUI(AppState.AFTER_START, "Error loading blend file");
                renderProgressBar.Style = ProgressBarStyle.Blocks;
            };
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

        private void RenderAll()
        {
            // Calculate chunks
            bool customLen = renderOptionsCustomRadio.Checked;
            Chunk[] chunks = customLen ? Chunk.CalcChunksByLenght(_project.BlendData.Start, _project.BlendData.End, _project.ChunkLenght)
                                       : Chunk.CalcChunks(_project.BlendData.Start, _project.BlendData.End, _project.ProcessesCount);

            UpdateCurrentChunks(chunks);

            startTime = DateTime.Now;

            renderManager = new RenderManager(_project);
            renderManager.Finished += (s,e) => this.InvokeAction(RenderManager_Finished, e);

            renderProgress = new Progress<RenderProgressInfo>();
            renderProgress.ProgressChanged += (s,e) => this.InvokeAction(UpdateProgress, e);

            IsWorking = true;

            // render progress reset
            totalTimeLabel.Text = TimePassedPrefix + TimeSpan.Zero.ToString(TimeFmt);
            _etaCalc = new ETACalculator(5, 1);

#if WINDOWS
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            TaskbarManager.Instance.SetProgressValue(0, 100);
#endif

            UpdateUI(AppState.RENDERING_ALL, "Starting render...");
            renderManager.Start(renderProgress);
        }

        private async void RenderManager_Finished(int e)
        {
            renderProgressBar.Style = ProgressBarStyle.Marquee;
            //afterRenderCancelSrc = new CancellationTokenSource();
            ResetCTS();

#if WINDOWS
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
#endif
            await AfterRender(_appSettings.AfterRender, afterRenderCancelSrc.Token);

            if (!afterRenderCancelSrc.Token.IsCancellationRequested)
            {
                WorkDone();
            }
        }


        private void StopWork(bool wasComplete)
        {
            if (!wasComplete)
            {
                if (renderManager != null && renderManager.InProgress)
                {
                    renderManager.Abort();
                }

                if (afterRenderCancelSrc != null)
                    afterRenderCancelSrc.Cancel();
            }


            IsWorking = false;
            renderProgressBar.Value = 0;
            renderProgressBar.Style = ProgressBarStyle.Blocks;
            renderProgressBar.Refresh();

            ETALabel.Text = ETR_Prefix + TimeSpan.Zero.ToString(TimeFmt);
            totalTimeLabel.Text = TimePassedPrefix + TimeSpan.Zero.ToString(TimeFmt);

            Text = Constants.APP_TITLE;

#if WINDOWS
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
#endif

            UpdateUI(AppState.READY_FOR_RENDER);
        }

        private void renderAllButton_Click(object sender, EventArgs e)
        {
            if (IsWorking)
            {
                var result = MessageBox.Show("Are you sure you want to stop?",
                                                "",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Exclamation);

                if (result == DialogResult.No)
                    return;

                // cancel
                StopWork(false);
                logger.Warn("RENDER ABORTED");
            }
            else
            {
                var chunksFolder = Path.Combine(_project.BlendData.OutputPath, Constants.ChunksSubfolder);

                if (Directory.Exists(chunksFolder) && Directory.GetFiles(chunksFolder).Length > 0)
                {
                    var dialogResult = MessageBox.Show("All previously rendered chunks will be deleted.\nDo you want to continue?",
                                            "Chunks folder not empty",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Exclamation);

                    if (dialogResult == DialogResult.No)
                        return;

                    var tryToClear = Helper.ClearFolder(chunksFolder);

                    if (!tryToClear) return;
                }

                RenderAll();
                logger.Info("RENDER STARTED");
            }
        }

        #endregion

        #region UpdateElements
        /// <summary>
        /// Updates the UI on the render process
        /// </summary>
        private void UpdateProgress(RenderProgressInfo info)
        {
            int progressPercentage = (int)Math.Floor((info.FramesRendered / (decimal)_project.BlendData.TotalFrames) * 100);

            var statusReport = $"Completed {info.PartsCompleted} / {_project.ChunkList.Count} chunks, {info.FramesRendered} frames rendered";

            Status(statusReport);

            // progress bar
#if WINDOWS
            TaskbarManager.Instance.SetProgressValue(progressPercentage, 100);
#endif
            renderProgressBar.Value = progressPercentage;

            _etaCalc.Update(progressPercentage / 100f);

            if (_etaCalc.ETAIsAvailable)
            {
                var etr = ETR_Prefix + _etaCalc.ETR.ToString(TimeFmt);
                Status(etr, ETALabel);
            }

            //time elapsed display
            TimeSpan runTime = DateTime.Now - startTime;
            var tElapsed = TimePassedPrefix + runTime.ToString(TimeFmt);
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
            bool ignore = (newChunks.TotalLength() > _project.BlendData.TotalFrames) 
                        || newChunks.SequenceEqual(_project.ChunkList);

            if (ignore)
                return;

            if (_project.ChunkList.Count > 0)
                _project.ChunkList.Clear();

            foreach (var chnk in newChunks)
            {
                _project.ChunkList.Add(chnk);
            }

            _project.ChunkLenght = _project.ChunkList.First().Length;

            logger.Debug("ChunkLenght: " + _project.ChunkLenght);
            logger.Trace(string.Join(", " ,_project.ChunkList));

#if UNIX
            ForceBindingSourceUpdate();
#endif

        }

        private void UpdateRecentBlendsMenu()
        {
            // clear local blends
            recentBlendsMenu.Items.Clear();

            // make blends from recent list
            var recentList = _appSettings.GetRecentBlends();
            foreach (string item in recentList)
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

            if (!File.Exists(blendPath))
            {
                var res = MessageBox.Show("Blend file not found, remove it from the recents list?", "Not found", 
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (res == DialogResult.Yes)
                {
                    _appSettings.RemoveRecentBlend(blendPath);
                }

                return;
            }

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

            ctrl.InvokeAction(() => ctrl.Text = msg);
        }

        #endregion

        #region Mixdown

        private Process GetMixdownProcess()
        {
            var mixdownCom = new Process();
            var info = new ProcessStartInfo()
            {
                FileName = _appSettings.BlenderProgram,
                StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format(CommandARGS.MixdownComARGS,
                                           _project.BlendPath,
                                           _project.BlendData.Start,
                                           _project.BlendData.End,
                                           Path.Combine(_appSettings.ScriptsFolder, Constants.PyMixdown),
                                           _project.BlendData.OutputPath)
            };
            mixdownCom.StartInfo = info;
            mixdownCom.EnableRaisingEvents = true;
            mixdownCom.Exited += (pSender, pe) =>
            {
                logger.Info("Mixdown done");
            };

            return mixdownCom;
        }

        private async void mixDownButton_Click(object sender, EventArgs e)
        {
            IsWorking = true;
            //afterRenderCancelSrc = new CancellationTokenSource();
            ResetCTS();

            UpdateUI(AppState.RENDERING_ALL, "Rendering Mixdown");
            renderProgressBar.Style = ProgressBarStyle.Marquee;

            var proc = GetMixdownProcess();
            var exitCode = await proc.StartAsync(afterRenderCancelSrc.Token);

            renderProgressBar.Style = ProgressBarStyle.Blocks;
            UpdateUI(AppState.READY_FOR_RENDER, "Mixdown complete");
            IsWorking = false;

            Trace.WriteLine("Mixdown proc exit code: " + exitCode, "Mixdown");
        }

        #endregion

        #region Concatenate

        bool CreateChunksTxtFile(string chunksFolder)
        {
            // TODO: Find a way to get the videos file ext
            // before rendering ends

            var fileListSorted = Utilities.GetChunkFiles(chunksFolder);

            if (fileListSorted.Count == 0)
            {
                MessageBox.Show("Failed to get chunk files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

        private Process GetConcatenateProcess(string mixdownFile = null)
        {
            var chunksFolder = Helper.GetChunksFolder(_project.BlendData);
            var chunkFiles = Utilities.GetChunkFiles(chunksFolder);
            var chunksTxtFile = Helper.GetChunkTxt(_project.BlendData);
            var videoExt = Path.GetExtension(chunkFiles.FirstOrDefault());
            var projPath = Path.Combine(_project.BlendData.OutputPath, _project.BlendData.ProjectName + videoExt);

            return GetConcatenateProcess(projPath, chunksTxtFile, mixdownFile);
        }

        private Process GetConcatenateProcess(string finalPath, string chunkTxtPath, string mixdownFile = null)
        {
            string comArgs;

            //mixdown audio NOT found
            if (string.IsNullOrWhiteSpace(mixdownFile))
            {
                Status("Joining chunks, please wait...");
                comArgs = string.Format(CommandARGS.ConcatenateOnly,
                            chunkTxtPath,
                            finalPath);
            }
            //mixdown audio found
            else
            {
                Status("Joining chunks with mixdown audio...");
                comArgs = string.Format(CommandARGS.ConcatenateMixdown,
                            chunkTxtPath,
                            mixdownFile,
                            finalPath);
            }

            var concatenateCom = new Process();
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

            logger.Info("Joining chunks");

            return concatenateCom;
        }

        private async void concatenatePartsButton_Click(object sender, EventArgs e)
        {
            IsWorking = true;
            //afterRenderCancelSrc = new CancellationTokenSource();
            ResetCTS();

            UpdateUI(AppState.RENDERING_ALL, "Concatenating...");
            renderProgressBar.Style = ProgressBarStyle.Marquee;

            var manConcat = new ConcatForm();
            manConcat.ShowDialog();

            if (manConcat.DialogResult == DialogResult.OK)
            {
                var concatProc = GetConcatenateProcess(manConcat.OutputFile, manConcat.ChunksTextFile, manConcat.MixdownAudioFile);
                var exitCode = await concatProc.StartAsync(afterRenderCancelSrc.Token);

                Trace.WriteLine("Concatenation proc exit code: " + exitCode, "Concat");
            }

            renderProgressBar.Style = ProgressBarStyle.Blocks;
            IsWorking = false;
            var done = "Concatenation complete!";

            if (string.IsNullOrEmpty(_project.BlendPath))
            {
                UpdateUI(AppState.AFTER_START, done);
            }
            else
                UpdateUI(AppState.READY_FOR_RENDER, done);
        }

        #endregion

        async Task AfterRender(AfterRenderAction action, CancellationToken token)
        {
            var chunksFolder = Helper.GetChunksFolder(_project.BlendData);

            string mixFile = null;
            if ((action & AfterRenderAction.MIXDOWN) != 0)
            {
                mixFile = Path.Combine(_project.BlendData.OutputPath, 
                                       _project.BlendData.ProjectName + '.' + _project.BlendData.AudioFileFormat);
            }

            if ((action & AfterRenderAction.JOIN) != 0)
            {
                // create chunklist.txt
                if (!CreateChunksTxtFile(chunksFolder))
                {
                    // did not create txtFile
                    return;
                }
            }

            Process mixdownProc, concatProc;
            int? mixEC = null, concEC = null;

            switch (action)
            {
                case AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN:
                    mixdownProc = GetMixdownProcess();
                    mixEC = await mixdownProc.StartAsync(token);

                    concatProc = GetConcatenateProcess(mixFile);
                    concEC = await concatProc.StartAsync(token);
                    break;
                case AfterRenderAction.JOIN:
                    concatProc = GetConcatenateProcess(mixFile);
                    concEC = await concatProc.StartAsync(token);
                    break;
                case AfterRenderAction.MIXDOWN:
                    mixdownProc = GetMixdownProcess();
                    mixEC = await mixdownProc.StartAsync(token);
                    break;
                case AfterRenderAction.NOTHING:
                default:
                    return;
            }
            Trace.WriteLineIf(mixEC != null, "Mixdown proc exit code: " + mixEC, "After Render");
            Trace.WriteLineIf(concEC != null, "Concat proc exit code: " + concEC, "After Render");
        }

        void WorkDone()
        {
            StopWork(true);

            var openOutputFolderQuestion =
                    MessageBox.Show("Open the folder with the video?",
                                    "Work complete!",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Information);

            if (openOutputFolderQuestion == DialogResult.Yes)
                OpenOutputFolder();

            UpdateUI(appState);
        }

        void ResetCTS()
        {
            if (afterRenderCancelSrc != null)
            {
                afterRenderCancelSrc.Dispose();
                afterRenderCancelSrc = null;
            }
            afterRenderCancelSrc = new CancellationTokenSource();
        }


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
            if (Directory.Exists(_project.BlendData.OutputPath))
            {
                Process.Start(_project.BlendData.OutputPath);
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
                    var currentProcessors = processCountNumericUpDown.Value;

                    var expectedChunkLen = (int)Math.Ceiling((currentEnd - currentStart + 1) / currentProcessors);
                    _project.ChunkLenght = expectedChunkLen;
                }
            }
            else if (radio.Name == startEndBlendRadio.Name)
            {
                if (radio.Checked)
                {
                    // set to blend values
                    _project.BlendData.Start = _autoRefStart;
                    _project.BlendData.End = _autoRefEnd;

                }
            }
#if UNIX
            ForceBindingSourceUpdate();
#endif
        }

        private void AfterRenderAction_Changed(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;

            if (radio.Checked)
            {
                if (radio.Name == afterRenderJoinMixdownRadio.Name)
                    _appSettings.AfterRender = AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN;

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
                InitialDirectory = _project.BlendData.OutputPath,
                IsFolderPicker = true,
                Title = "Select output location",

            };

            var result = folderPicker.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                _project.BlendData.OutputPath = folderPicker.FileName;
                UpdateUI();
            }
#else
            var folderPicker = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = _project.BlendData.OutputPath,
                ShowNewFolderButton = true
            };
            var result = folderPicker.ShowDialog();

            if (result == DialogResult.OK)
            {
                _project.BlendData.OutputPath = folderPicker.SelectedPath;
                UpdateUI();

                blendDataBindingSource.ResetBindings(false);
            }
#endif
        }

        private void StartEndNumeric_Validated(object sender, EventArgs e)
        {
            var currentStart = totalStartNumericUpDown.Value;
            var currentEnd = totalEndNumericUpDown.Value;
            var currentProcessors = processCountNumericUpDown.Value;

            if (renderOptionsAutoRadio.Checked)
            {
                var expectedChunkLen = (int)Math.Ceiling((currentEnd - currentStart + 1) / currentProcessors);
                _project.ChunkLenght = expectedChunkLen;
#if UNIX
                ForceBindingSourceUpdate();
#endif
            }

            // set max chunk size to total frames
            chunkLengthNumericUpDown.Maximum = currentEnd - currentStart + 1;
        }

        private void StartEnd_Validating(object sender, CancelEventArgs e)
        {
            // TODO: Handle invalid values
            if (totalStartNumericUpDown.Value >= totalEndNumericUpDown.Value)
            {
                errorProvider.
                    SetError(totalStartNumericUpDown, "Start frame cannot be equal or greater then End frame");
                e.Cancel = true;
            }
            else if (totalEndNumericUpDown.Value <= totalStartNumericUpDown.Value)
            {
                errorProvider
                    .SetError(totalEndNumericUpDown, "End frame cannot be equal or less then Start frame");
                e.Cancel = true;
            }
            else
                errorProvider.Clear();
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
                    "?cmd=_donations" +
                    "&business=" + business +
                    "&lc=" + country +
                    "&item_name=" + description +
                    "&currency_code=" + currency +
                    "&bn=PP%2dDonationsBF";

            Process.Start(url);
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


        private void ForceBindedElementsUpdate(object sender, EventArgs e)
        {
            ForceBindingSourceUpdate();
        }

        private void ForceBindingSourceUpdate()
        {
            // WinForm databinding in Mono doesn't update the UI elements 
            // properly, so do it manually
            blendDataBindingSource.ResetBindings(false);
            projectSettingsBindingSource.ResetBindings(false);
        }

        /// <summary>
        /// Updates UI according to <see cref="AppState"/>
        /// </summary>
        /// <param name="statusMsg">Status message</param>
        private void UpdateUI(string statusMsg)
        {
            UpdateUI(null, statusMsg);
        }

        /// <summary>
        /// Updates UI according to <see cref="AppState"/>
        /// </summary>
        /// <param name="newState">New state, leave empty to refresh using the current state</param>
        /// <param name="statusMsg">Overrides the default message sent to <see cref="Status"/></param>
        private void UpdateUI(AppState? newState = null, string statusMsg = null)
        {
            appState = newState ?? appState;
            string msgToSend = null;

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
                    concatenatePartsButton.Enabled = true;
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
                    //Status("Select a file");
                    msgToSend = statusMsg ?? "Select a file";
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
                    //Status("Required program(s) not found, see Settings");
                    msgToSend = statusMsg ?? "Required program(s) not found, see Settings";
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
                    concatenatePartsButton.Enabled = true;
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
                    //Status("Ready");
                    msgToSend = statusMsg ?? "Ready";
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
                    msgToSend = statusMsg;
                    break;
            }

            if (msgToSend != null)
                Status(msgToSend);
        }

    }

}
