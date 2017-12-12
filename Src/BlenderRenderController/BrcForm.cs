// For Mono compatible Unix builds compile with /d:UNIX
#if !WIN && !UNIX
#error You must define a platform (WIN or UNIX)
#elif UNIX
#undef WIN
#endif

using BlenderRenderController.Properties;
using BRClib;
using BRClib.Commands;
#if WIN
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
using static BRClib.CommandARGS;

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

        AppSettings _appSettings;
        ProjectSettings _project;
        RenderManager _renderMngr;
        Progress<RenderProgressInfo> _renderProg;
        int _autoStartF, _autoEndF;
        AppState appState;
        Stopwatch _chrono;
        ETACalculator _etaCalc;
        SettingsForm _settingsForm;
        CancellationTokenSource _afterRenderCancelSrc;

        public bool IsWorking { get; private set; }


        public BrcForm()
        {
            InitializeComponent();

            _project = new ProjectSettings();
            _project.BlendData = new BlendData();
            _appSettings = AppSettings.Current;
#if WIN
            // set the form icon outside designer
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
#endif
            // RenderManager
            _renderMngr = new RenderManager();
            _renderMngr.Finished += RenderManager_Finished; ;
            _renderMngr.AfterRenderStarted += RenderManager_AfterRenderStarted;
            //renderManager.ProgressChanged += (s, prog) => UpdateProgress(prog);
            _renderProg = new Progress<RenderProgressInfo>(UpdateProgress);

            _chrono = new Stopwatch();
            _etaCalc = new ETACalculator(5, 1);
        }


        private void BrcForm_Load(object sender, EventArgs e)
        {
            //add version numbers to label
            verToolStripLbl.Text = " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

            // save appSettings on exit
            AppDomain.CurrentDomain.ProcessExit += (ad, cd) => _appSettings.Save();

            // load recent blends from file
            UpdateRecentBlendsMenu();

            _appSettings.RecentBlends_Changed += (s, args) => UpdateRecentBlendsMenu();

            processCountNumericUpDown.Maximum = 
            _project.MaxConcurrency = Environment.ProcessorCount;

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

            // set what afterRenderRadio button is checked
            switch (_appSettings.AfterRender)
            {
                case AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN:
                    afterRenderJoinMixdownRadio.Checked = true;
                    break;
                case AfterRenderAction.JOIN:
                    afterRenderJoinRadio.Checked = true;
                    break;
                case AfterRenderAction.NOTHING:
                    afterRenderDoNothingRadio.Checked = true;
                    break;
                default:
                    goto case AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN;
            }
#if UNIX
            forceUIUpdateToolStripMenuItem.Visible = true;
            forceUIUpdateToolStripMenuItem.Click += (s,args) => ForceBindingSourceUpdate();
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
#if WIN
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

            // exec process asynchronously
            var giScript = Path.Combine(_appSettings.ScriptsFolder,
                                        Constants.PyGetInfo);

            var giCmd = new GetInfoCmd(_appSettings.BlenderProgram, 
                                        blendFile, 
                                        giScript);

            //var pResult = await giCmd.GetProcess().StartAsync(true, true);
            var pResult = await giCmd.StartAsync(true, true);

            // errors
            if (pResult.StdOutput.Length > 0)
            {
                logger.Debug("Blender output errors detected");
                //Debug.WriteLine('\n' + pResult.StdError + '\n');
            }

            if (pResult.StdOutput.Length == 0)
            {
                var detailsContent = $"Blender's exit code {pResult.ExitCode}\nOutput:\n\n" + pResult.StdError;

                var err = Ui.Dialogs.ShowErrorBox("Could not open project, no information was received.", 
                                                  "Failed to read project", 
                                                  "Error", 
                                                  detailsContent);
                //err.Show();
                ReadFail();
                return;
            }

            var blendData = Utilities.ParsePyOutput(pResult.StdOutput);

            if (blendData != null)
            {
                _project.BlendData = blendData;
                _project.BlendPath = blendFile;

                // save copy of start and end frames values
                _autoStartF = blendData.Start;
                _autoEndF = blendData.End;

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
                    "Read error", "Error output:\n\n" + pResult.StdError);

                //errorBox.Show();
                ReadFail();
                return;
            }

            var chunks = Chunk.CalcChunks(blendData.Start, blendData.End, _project.MaxConcurrency);
            UpdateCurrentChunks(chunks.ToArray());
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
            var chunks = customLen 
                ? Chunk.CalcChunksByLength(_project.BlendData.Start, 
                                           _project.BlendData.End,
                                           _project.ChunkLenght)
                
                : Chunk.CalcChunks(_project.BlendData.Start, 
                                   _project.BlendData.End,
                                   _project.MaxConcurrency);

            UpdateCurrentChunks(chunks.ToArray());

            logger.Info(() => "Chunks: " + string.Join(", ", chunks));

            IsWorking = true;

            _renderMngr.Setup(_project);
            _renderMngr.Action = _appSettings.AfterRender;

            totalTimeLabel.Text = TimePassedPrefix + TimeSpan.Zero.ToString(TimeFmt);

#if WIN
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            TaskbarManager.Instance.SetProgressValue(0, 100);
#endif
            UpdateUI(AppState.RENDERING_ALL, "Starting render...");

            _chrono.Start();
            _renderMngr.StartAsync(_renderProg);
        }

        private void RenderManager_Finished(object sender, EventArgs e)
        {
            // all slow work is done
            StopWork(true);

            if (_renderMngr.GetAfterRenderResult()) // AfterActions ran Ok
            {
                var dialog =
                    MessageBox.Show("Open destination folder?",
                        "Work complete!",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                if (dialog == DialogResult.Yes)
                    OpenOutputFolder();

                UpdateUI(AppState.READY_FOR_RENDER);

            }
            else if (!_renderMngr.WasAborted) // Erros detected
            {
                var dialog = MessageBox.Show(Resources.AR_error_msg, Resources.Error,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);



                UpdateUI(AppState.READY_FOR_RENDER, "Errors detected");
            }
            else // operation aborted
            {
                UpdateUI(AppState.READY_FOR_RENDER, "Operation Aborted");
            }

        }

        private void RenderManager_AfterRenderStarted(object sender, AfterRenderAction e)
        {
            renderProgressBar.Style = ProgressBarStyle.Marquee;
#if WIN
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
#endif

            switch (e)
            {
                case AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN:
                    Status("Joining chunks w/ custom mixdown");
                    break;
                case AfterRenderAction.JOIN:
                    Status("Joining chunks");
                    break;
                case AfterRenderAction.MIXDOWN:
                    Status("Rendering mixdown");
                    break;
                default:
                    break;
            }
        }

        private void StopWork(bool wasComplete)
        {
            if (!wasComplete)
            {
                if (_renderMngr != null && _renderMngr.InProgress)
                {
                    _renderMngr.Abort();
                }

                if (_afterRenderCancelSrc != null)
                    _afterRenderCancelSrc.Cancel();
            }

            _etaCalc.Reset();
            _chrono.Reset();
            IsWorking = false;
            renderProgressBar.Value = 0;
            renderProgressBar.Style = ProgressBarStyle.Blocks;
            renderProgressBar.Refresh();

            ETALabel.Text = ETR_Prefix + TimeSpan.Zero.ToString(TimeFmt);
            totalTimeLabel.Text = TimePassedPrefix + TimeSpan.Zero.ToString(TimeFmt);

            Text = Constants.APP_TITLE;

#if WIN
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
#endif

            UpdateUI(AppState.READY_FOR_RENDER);
        }

        private void renderAllButton_Click(object sender, EventArgs e)
        {
            if (IsWorking)
            {
                var result = MessageBox.Show("Are you sure you want to stop?",
                                                "Cancel",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Exclamation);

                if (result == DialogResult.No)
                    return;

                // cancel
                StopWork(false);
            }
            else
            {
                var outputDir = _project.BlendData.OutputPath;

                if ((Directory.Exists(outputDir) && Directory.GetFiles(outputDir).Length > 0)
                    || Directory.Exists(_project.ChunkSubdirPath))
                {
                    var dialogResult = MessageBox.Show("All existing files will be deleted!\n" +
                                                        "Do you want to continue?",
                                                        "Output folder not empty",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Exclamation);

                    if (dialogResult == DialogResult.No)
                        return;

                    var tryToClear = Helper.ClearOutputFolder(outputDir);

                    if (!tryToClear) return;
                }

                RenderAll();
            }
        }

        #endregion

        #region UpdateElements
        /// <summary>
        /// Updates the UI on the render process
        /// </summary>
        private void UpdateProgress(RenderProgressInfo info)
        {
            int progressPercentage = (int)Math.Floor((info.FramesRendered / (double)_project.BlendData.TotalFrames) * 100);

            Status($"Completed {info.PartsCompleted} / {_project.ChunkList.Count} chunks, {info.FramesRendered} frames rendered");

            // progress bar
#if WIN
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
            TimeSpan runTime = _chrono.Elapsed;
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

            if (ignore) return;

            if (_project.ChunkList.Count > 0)
                _project.ChunkList.Clear();

            foreach (var chnk in newChunks)
            {
                _project.ChunkList.Add(chnk);
            }

            _project.ChunkLenght = _project.ChunkList.First().Length;

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


        delegate void StatusDelegate(string msg, Control control);

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
                ctrl.Invoke(new StatusDelegate(Status), msg, ctrl);
            }
            else
            {
                ctrl.Text = msg;
            }
        }

        #endregion


        private async void mixDownButton_Click(object sender, EventArgs e)
        {
            IsWorking = true;
            ResetCTS();

            UpdateUI(AppState.RENDERING_ALL, "Rendering mixdown...");
            renderProgressBar.Style = ProgressBarStyle.Marquee;

            var mix = new MixdownCmd(_appSettings.BlenderProgram,
                                    _project.BlendPath, 
                                    _project.BlendData.Start, 
                                    _project.BlendData.End, 
                                    Path.Combine(_appSettings.ScriptsFolder, 
                                                Constants.PyMixdown),
                                    _project.BlendData.OutputPath);

            var result = await mix.Run(_afterRenderCancelSrc.Token);

            if (result)
            {
                UpdateUI(AppState.READY_FOR_RENDER, "Mixdown complete");
            }
            else
            {
                MessageBox.Show("Something went wrong, check logs at the output folder...", 
                        Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);

                mix.SaveReport(_project.BlendData.OutputPath);

                UpdateUI(AppState.READY_FOR_RENDER, "Something went wrong...");
            }


            renderProgressBar.Style = ProgressBarStyle.Blocks;
            IsWorking = false;
        }

        private async void concatenatePartsButton_Click(object sender, EventArgs e)
        {
            IsWorking = true;
            ResetCTS();

            UpdateUI(AppState.RENDERING_ALL, "Concatenating...");
            renderProgressBar.Style = ProgressBarStyle.Marquee;

            var manConcat = new ConcatForm();
            manConcat.ShowDialog();

            if (manConcat.DialogResult == DialogResult.OK)
            {
                //var concatArgs = GetConcatenationArgs(manConcat.ChunksTextFile,
                //                                        manConcat.OutputFile,
                //                                        manConcat.MixdownAudioFile);


                //var concatProc = ProcessFactory.ConcatProcess(_appSettings.FFmpegProgram, concatArgs);
                //var pr = new ProcessRunner(concatProc);
                //var result = await pr.Run(_afterRenderCancelSrc.Token);

                var concat = new ConcatCmd(_appSettings.FFmpegProgram,
                                        manConcat.ChunksTextFile,
                                        manConcat.OutputFile,
                                        manConcat.MixdownAudioFile);

                var result = await concat.Run(_afterRenderCancelSrc.Token);

                if (result)
                {
                    UpdateUI(AppState.READY_FOR_RENDER, "Concatenation complete");
                }
                else
                {
                    MessageBox.Show("Something went wrong, check logs at the output folder...",
                             Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    var outFolder = Path.GetDirectoryName(manConcat.OutputFile);
                    concat.SaveReport(outFolder);

                    UpdateUI(AppState.READY_FOR_RENDER, "Something went wrong...");
                }
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


        void ResetCTS()
        {
            if (_afterRenderCancelSrc != null)
            {
                _afterRenderCancelSrc.Dispose();
                _afterRenderCancelSrc = null;
            }
            _afterRenderCancelSrc = new CancellationTokenSource();
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
                    _project.MaxConcurrency = Environment.ProcessorCount;
                    // recalc auto chunks:
                    var currentStart = totalStartNumericUpDown.Value;
                    var currentEnd = totalEndNumericUpDown.Value;
                    var currentProcessors = processCountNumericUpDown.Value;

                    var expectedChunkLen = Math.Ceiling((currentEnd - currentStart + 1) / currentProcessors);

                    _project.ChunkLenght = (int)expectedChunkLen;
                    //chunkLengthNumericUpDown.Value = expectedChunkLen;
                }
            }
            else if (radio.Name == startEndBlendRadio.Name)
            {
                if (radio.Checked)
                {
                    // set to blend values
                    _project.BlendData.Start = _autoStartF;
                    _project.BlendData.End = _autoEndF;
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

#if WIN
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
                var expectedChunkLen = Math.Ceiling((currentEnd - currentStart + 1) / currentProcessors);
                _project.ChunkLenght = (int)expectedChunkLen;
#if UNIX
                ForceBindingSourceUpdate();
#endif
            }

            // set max chunk size to total frames
            chunkLengthNumericUpDown.Maximum = currentEnd - currentStart + 1;
        }

        private void StartEnd_Validating(object sender, CancelEventArgs e)
        {
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
            else if ((totalEndNumericUpDown.Value - totalStartNumericUpDown.Value + 1) < 50)
            {
                var msg = "Project must be at least 50 frames long";

                errorProvider.SetError(totalEndNumericUpDown, msg);
                errorProvider.SetError(totalStartNumericUpDown, msg);
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
