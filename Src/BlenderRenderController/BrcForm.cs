// For Mono compatible Unix builds compile with /d:UNIX
#if !WIN && !UNIX
#error You must define a platform (WIN or UNIX)
#elif UNIX
#undef WIN
#endif

using BlenderRenderController.Properties;
using BRClib;
using BRClib.Commands;
using BRClib.Extentions;
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

namespace BlenderRenderController
{
    using ScriptShelf = BRClib.Scripts.Shelf;

    /// <summary>
    /// Main Window
    /// </summary>
    public partial class BrcForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        const string TimePassedPrefix = "Time: ",
                     ETR_Prefix = "ETR: ",
                     TimeFmt = @"hh\:mm\:ss";

        int _autoStartF, _autoEndF;

        AppSettings _appSettings;
        RenderManager _renderMngr;
        Stopwatch _chrono;
        ETACalculator _etaCalc;
        SettingsForm _settingsForm;
        CancellationTokenSource _afterRenderCancelSrc;

        BrcViewModel _vm;


        public BrcForm()
        {
            InitializeComponent();

            _appSettings = AppSettings.Current;
            _vm = new BrcViewModel();
            _vm.PropertyChanged += ViewModel_PropertyChanged;
#if WIN
            // set the form icon outside designer
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
#endif
            // RenderManager
            _renderMngr = new RenderManager();
            _renderMngr.Finished += RenderManager_Finished;
            _renderMngr.AfterRenderStarted += RenderManager_AfterRenderStarted;
            _renderMngr.ProgressChanged += (s, prog) => UpdateProgress(prog);

            _chrono = new Stopwatch();
            _etaCalc = new ETACalculator(5, 1);
        }


        private void BrcForm_Load(object sender, EventArgs e)
        {
            _vm.ConfigOk = _appSettings.CheckCorrectConfig();

            // setup sources for ComboBoxes
            cbRenderer.DataSource = Enum.GetValues(typeof(Renderer));
            cbRenderer.SelectedItem = _appSettings.Renderer;
            cbRenderer.SelectedIndexChanged += Renderer_Changed;

            cbAfterRenderAction.DisplayMember = "Value";
            cbAfterRenderAction.ValueMember = "Key";
            cbAfterRenderAction.DataSource = Helper.AfterRenderResources.ToList();
            cbAfterRenderAction.SelectedValue = _appSettings.AfterRender;
            cbAfterRenderAction.SelectedIndexChanged += AfterRenderAction_Changed;

            // load recent blends from file
            UpdateRecentBlendsMenu();

            _appSettings.RecentProjects.CollectionChanged += delegate { UpdateRecentBlendsMenu(); };

            processCountNumericUpDown.Maximum = Environment.ProcessorCount;

            // Time duration format
            infoDuration.DataBindings["Value"].Format += (fs, fe) =>
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

            exitToolStripMenuItem.Click += delegate { Close(); };

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

            if (!_vm.ConfigOk)
            {

                _settingsForm = new SettingsForm();
                _settingsForm.FormClosed += SettingsForm_FormClosed;

                string errMsg = "One or more required program(s) were not found " +
                    "(Path invalid OR first time run), set the paths in the Settings window";
                string cap = "Setup required";
                string info = "Paths missing";
#if WIN
                var td = new TaskDialog()
                {
                    Caption = cap,
                    InstructionText = info,
                    Text = errMsg,
                    Icon = TaskDialogStandardIcon.Warning,
                    StandardButtons = TaskDialogStandardButtons.Close,
                    OwnerWindowHandle = this.Handle
                };

                var tdCmdLink = new TaskDialogCommandLink("BtnOpenSettings", "Goto Settings");
                tdCmdLink.Click += delegate
                {
                    _settingsForm.Show(this);
                    td.Close();
                };

                td.Controls.Add(tdCmdLink);
                td.Show();
#else
                errMsg += "\n\n" + "Click 'Retry' to open Settings";
                var res = MessageBox.Show(errMsg, cap + " - " + info, MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);

                if (res == DialogResult.Retry)
                {
                    // fix width and show
                    _settingsForm.MaximumSize = _settingsForm.Size;
                    _settingsForm.Show();
                }
#endif
            }

        }

        private void BrcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // BUG: e.Cancel is set to true after a render process is done

            if (_vm.IsBusy)
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

            //e.Cancel = false;
            _appSettings.SaveCurrent();
            logger.Info("Program closing");
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // when closing the Settings window, check if valid
            // and update UI if needed
            _vm.ConfigOk = _appSettings.CheckCorrectConfig();
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

            logger.Info("Loading .blend");
            Status("Reading .blend file...");
            UpdateProgressBars(-1);

            // exec process asynchronously
            var giScript = ScriptShelf.GetProjectInfo;

            var giProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _appSettings.BlenderProgram,
                    Arguments = $"-b \"{blendFile}\" -P \"{giScript}\"",

                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },

                EnableRaisingEvents = true,
            };

            var pResult = await giProc.StartAsync();
            //var pResult = await giCmd.RunAsync();

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

            Project proj;
            var blendData = Utilities.ParsePyOutput(pResult.StdOutput);

            if (blendData != null)
            {
                proj = new Project(blendData)
                {
                    BlendFilePath = blendFile,
                    MaxConcurrency = Environment.ProcessorCount
                };

                // save copy of start and end frames values
                _autoStartF = blendData.Start;
                _autoEndF = blendData.End;


                if (RenderFormats.IMAGES.Contains(blendData.FileFormat))
                {
                    var eMsg = string.Format(Resources.AppErr_RenderFormatIsImage,
                                             blendData.FileFormat);

                    MessageBox.Show(eMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }

                // output path w/o project name
                if (string.IsNullOrWhiteSpace(proj.OutputPath))
                {
                    // use .blend folder path if outputPath is unset, display a warning about it
                    MessageBox.Show(Resources.AppErr_BlendOutputInvalid, "",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    proj.OutputPath = Path.GetDirectoryName(blendFile);
                }
                else
                    proj.OutputPath = Path.GetDirectoryName(proj.OutputPath);


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

            _vm.Project = proj;
            projectBindingSrc.DataSource = _vm.Project;

            var chunks = Chunk.CalcChunks(blendData.Start, blendData.End, proj.MaxConcurrency);
            UpdateCurrentChunks(chunks.ToArray());
            _appSettings.RecentProjects.Add(blendFile);
            UpdateProgressBars();
            // ---

            // call this if GetBlendInfo fails
            void ReadFail()
            {
                logger.Error(".blend was NOT loaded");
                Status("Error loading blend file");
                UpdateProgressBars();
            };
        }

        private void OpenBlend_Click(object sender, EventArgs e)
        {
            var result = openBlendDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var blend = openBlendDialog.FileName;
                GetBlendInfo(blend);
            }
        }

        private void ReloadBlend_Click(object sender, EventArgs e)
        {
            var blend = _vm.Project.BlendFilePath;
            if (!string.IsNullOrEmpty(blend))
            {
                GetBlendInfo(blend);
            }

            Status(_vm.DefaultStatusMessage);
        }

        private void RecentBlendsItem_Click(object sender, EventArgs e)
        {
            var recentItem = (ToolStripMenuItem)sender;
            var blendPath = recentItem.ToolTipText;

            if (!File.Exists(blendPath))
            {
                var res = MessageBox.Show("Blend file not found, remove it from the recents list?", "Not found",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (res == DialogResult.Yes)
                {
                    _appSettings.RecentProjects.Remove(blendPath);
                }

                return;
            }

            GetBlendInfo(blendPath);
        }
        #endregion

        #region RenderMethods

        private void RenderAll()
        {
            // Calculate chunks
            bool customLen = renderOptionsCustomRadio.Checked;
            var chunks = customLen
                ? Chunk.CalcChunksByLength(_vm.Project.Start,
                                           _vm.Project.End,
                                           _vm.Project.ChunkLenght)

                : Chunk.CalcChunks(_vm.Project.Start,
                                   _vm.Project.End,
                                   _vm.Project.MaxConcurrency);

            UpdateCurrentChunks(chunks.ToArray());

            logger.Info("Chunks: " + string.Join(", ", chunks));

            _vm.IsBusy = true;

            _renderMngr.Setup(_vm.Project);
            _renderMngr.Action = _appSettings.AfterRender;
            _renderMngr.BlenderProgram = _appSettings.BlenderProgram;
            _renderMngr.FFmpegProgram = _appSettings.FFmpegProgram;
            _renderMngr.Renderer = _appSettings.Renderer;

            statusTime.Text = TimePassedPrefix + TimeSpan.Zero.ToString(TimeFmt);

            UpdateProgressBars();

            Status("Starting render...");

            _chrono.Start();
            _renderMngr.StartAsync();
        }

        private void RenderManager_Finished(object sender, EventArgs e)
        {
            // all slow work is done
            StopWork(true);

            if (_renderMngr.GetAfterRenderResult()) // AfterActions ran Ok
            {
                if (_renderMngr.Action != AfterRenderAction.NOTHING &&
                    _appSettings.DeleteChunksFolder)
                {
                    try
                    {
                        Directory.Delete(_vm.Project.DefaultChunksDirPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to clear 'chunks' folder:\n\n" + ex.Message,
                                        Resources.Error,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }

                var dialog = MessageBox.Show("Open destination folder?",
                                                "Work complete!",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Information);

                if (dialog == DialogResult.Yes)
                    OpenOutputFolder();

            }
            else if (!_renderMngr.WasAborted) // Erros detected
            {
                //var dialog = MessageBox.Show(Resources.AR_error_msg, Resources.Error,
                //                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                Status("Errors detected");
            }
            else // operation aborted
            {
                Status("Operation Aborted");
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
            _vm.IsBusy = false;
            UpdateProgressBars(0);

            statusETR.Text = ETR_Prefix + TimeSpan.Zero.ToString(TimeFmt);
            statusTime.Text = TimePassedPrefix + TimeSpan.Zero.ToString(TimeFmt);

            Text = Constants.APP_TITLE;
        }

        private void renderAllButton_Click(object sender, EventArgs e)
        {
            if (_vm.IsBusy)
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
                var outputDir = _vm.Project.OutputPath;

                if ((Directory.Exists(outputDir) && Directory.GetFiles(outputDir).Length > 0)
                    || Directory.Exists(_vm.Project.DefaultChunksDirPath))
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
            int progressPercentage = (int)Math.Floor((info.FramesRendered / (double)_vm.Project.TotalFrames) * 100);

            Status($"Completed {info.PartsCompleted} / {_vm.Project.ChunkList.Count} chunks, {info.FramesRendered} frames rendered");

            UpdateProgressBars(progressPercentage);

            _etaCalc.Update(progressPercentage / 100f);

            if (_etaCalc.ETAIsAvailable)
            {
                var etr = ETR_Prefix + _etaCalc.ETR.ToString(TimeFmt);
                Status(etr, statusETR);
            }

            //time elapsed display
            TimeSpan runTime = _chrono.Elapsed;
            var tElapsed = TimePassedPrefix + runTime.ToString(TimeFmt);
            Status(tElapsed, statusTime);
        }

        void UpdateProgressBars(int progressPercent = 0)
        {
            string titleProg = Constants.APP_TITLE;

            if (progressPercent < 0)
            {
                renderProgressBar.Style = ProgressBarStyle.Marquee;
#if WIN
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
#endif
            }
            else
            {
                renderProgressBar.Style = ProgressBarStyle.Blocks;
                renderProgressBar.Value = progressPercent;

                if (progressPercent != 0)
                {
                    titleProg = $"{progressPercent}% complete - {Constants.APP_TITLE}";
#if WIN
                    TaskbarManager.Instance.SetProgressValue(progressPercent, 100);
                }
                else
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
#else
                }
#endif

            }

            SafeChangeText(titleProg, this);
        }

        /// <summary>
        /// Updates the list of chunks that will be rendered
        /// </summary>
        /// <param name="newChunks"></param>
        private void UpdateCurrentChunks(params Chunk[] newChunks)
        {
            bool ignore = (newChunks.TotalLength() > _vm.Project.TotalFrames)
                        || newChunks.SequenceEqual(_vm.Project.ChunkList);

            if (ignore) return;

            if (_vm.Project.ChunkList.Count > 0)
                _vm.Project.ChunkList.Clear();

            foreach (var chnk in newChunks)
            {
                _vm.Project.ChunkList.Add(chnk);
            }

            _vm.Project.ChunkLenght = _vm.Project.ChunkList.First().Length;

#if UNIX
            ForceBindingSourceUpdate();
#endif

        }

        private void UpdateRecentBlendsMenu()
        {
            // clear local
            var localItems = recentBlendsMenu.Items.Find("recent", false);
            foreach (var item in localItems)
            {
                recentBlendsMenu.Items.Remove(item);
            }


            // show placeholder if recents list is empty
            if (_appSettings.RecentProjects.Count == 0)
            {
                miEmptyPH.Visible = true;
                return;
            }
            else
            {
                miEmptyPH.Visible = false;
            }

            // make items from recent list
            foreach (string item in _appSettings.RecentProjects)
            {
                var menuItem = new ToolStripMenuItem
                {
                    ToolTipText = item,
                    Text = Path.GetFileNameWithoutExtension(item),
                    Image = Resources.blender_icon,
                    Name = "recent"
                };
                menuItem.Click += RecentBlendsItem_Click;
                recentBlendsMenu.Items.Add(menuItem);
            }

        }


        delegate void ChangeTextDelegate(string msg, Control control);

        /// <summary>
        /// Thread safe method to change UI text
        /// </summary>
        /// <param name="msg">new text</param>
        /// <param name="ctrl">Control to update</param>
        private void SafeChangeText(string msg, Control ctrl)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new ChangeTextDelegate(SafeChangeText), msg, ctrl);
            }
            else
            {
                ctrl.Text = msg;
            }
        }

        private void Status(string msg, ToolStripItem tsItem = null)
        {
            if (tsItem == null) tsItem = statusMessage;

            // Needs Invoke?

            tsItem.Text = msg;
        }

        #endregion


        private async void mixDownButton_Click(object sender, EventArgs e)
        {
            _vm.IsBusy = true;
            ResetCTS();
            UpdateProgressBars(-1);

            Status("Rendering mixdown...");

            var mix = new MixdownCmd(_appSettings.BlenderProgram,
                                    _vm.Project.BlendFilePath,
                                    _vm.Project.Start,
                                    _vm.Project.End,
                                    ScriptShelf.MixdownAudio,
                                    _vm.Project.OutputPath);

            var result = await mix.RunAsync(_afterRenderCancelSrc.Token);

            if (result == 0)
            {
                Status("Mixdown complete");
            }
            else
            {
                MessageBox.Show("Something went wrong, check logs at the output folder...",
                        Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);

                mix.SaveReport(_vm.Project.OutputPath);

                Status("Something went wrong...");
            }

            UpdateProgressBars();
            _vm.IsBusy = false;
        }

        private async void concatenatePartsButton_Click(object sender, EventArgs e)
        {
            _vm.WorkToggle();
            ResetCTS();
            UpdateProgressBars(-1);


            var manConcat = new ConcatForm();
            var dResult = manConcat.ShowDialog();

            if (dResult == DialogResult.OK)
            {
                var concat = new ConcatCmd(_appSettings.FFmpegProgram,
                                        manConcat.ChunksTextFile,
                                        manConcat.OutputFile,
                                        manConcat.MixdownAudioFile);

                var result = await concat.RunAsync(_afterRenderCancelSrc.Token);

                if (result == 0)
                {
                    Status("Concatenation complete");
                }
                else
                {
                    MessageBox.Show("Something went wrong, check logs at the output folder...",
                             Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    var outFolder = Path.GetDirectoryName(manConcat.OutputFile);
                    concat.SaveReport(outFolder);

                    Status("Something went wrong...");
                }
            }

            UpdateProgressBars();
            _vm.WorkToggle();
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

        private void OpenOutputFolder()
        {
            if (Directory.Exists(_vm.Project.OutputPath))
            {
                Process.Start(_vm.Project.OutputPath);
            }
            else
            {
                MessageBox.Show("Output folder does not exist.", "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }

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

        private void AutoOptionsRadio_CheckedChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;

            if (radio.Name == renderOptionsAutoRadio.Name)
            {
                if (radio.Checked)
                {
                    _vm.Project.MaxConcurrency = Environment.ProcessorCount;
                    // recalc auto chunks:
                    var currentStart = totalStartNumericUpDown.Value;
                    var currentEnd = totalEndNumericUpDown.Value;
                    var currentProcessors = processCountNumericUpDown.Value;

                    var expectedChunkLen = Math.Ceiling((currentEnd - currentStart + 1) / currentProcessors);

                    _vm.Project.ChunkLenght = (int)expectedChunkLen;
                    //chunkLengthNumericUpDown.Value = expectedChunkLen;
                }
            }
            else if (radio.Name == startEndBlendRadio.Name)
            {
                if (radio.Checked)
                {
                    // set to blend values
                    _vm.Project.Start = _autoStartF;
                    _vm.Project.End = _autoEndF;
                }
            }
#if UNIX
            ForceBindingSourceUpdate();
#endif
        }

        private void AfterRenderAction_Changed(object sender, EventArgs e)
        {
            _appSettings.AfterRender = (AfterRenderAction)cbAfterRenderAction.SelectedValue;
        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            string dialogTxt = "Select output location";

            var nPath = Ui.Dialogs.OutputFolderSelection(dialogTxt, _vm.Project.OutputPath);

            if (nPath != null)
            {
                _vm.Project.OutputPath = nPath;
            }
        }

        private void StartEndNumeric_Validated(object sender, EventArgs e)
        {
            var currentStart = totalStartNumericUpDown.Value;
            var currentEnd = totalEndNumericUpDown.Value;
            var currentProcessors = processCountNumericUpDown.Value;

            if (renderOptionsAutoRadio.Checked)
            {
                var expectedChunkLen = Math.Ceiling((currentEnd - currentStart + 1) / currentProcessors);
                _vm.Project.ChunkLenght = (int)expectedChunkLen;
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

        private void Renderer_Changed(object sender, EventArgs e)
        {
            _appSettings.Renderer = (Renderer)cbRenderer.SelectedValue;
        }



        private void donateButton_Click(object sender, EventArgs e)
        {
            string business = "9SGQVK6TK2UJG";
            string description = "Donation%20for%20Blender%20Render%20Controller";
            string country = "BR";
            string currency = "USD";

            string url = "https://www.paypal.com/cgi-bin/webscr" +
                    "?cmd=_donations" +
                    "&business=" + business +
                    "&lc=" + country +
                    "&item_name=" + description +
                    "&item_number=BRC" +
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
            string url = "https://github.com/jendabek/BlenderRenderController/wiki/Reporting-an-issue";

            Process.Start(url);
        }

        private void clearRecentProjectsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_appSettings.RecentProjects.Count == 0)
                return;

            var response = MessageBox.Show(
                 "This will clear all files in the recent blends list, are you sure?",
                 "Clear recent blends?",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (response == DialogResult.Yes)
                _appSettings.RecentProjects.Clear();
        }

        private void UnloadCurrent_Click(object sender, EventArgs e)
        {
            projectBindingSrc.Clear();
            _vm.Project = null;
        }

        private void miGithub_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/jendabek/BlenderRenderController");
        }

        private void ForceBindingSourceUpdate()
        {
            // WinForm databinding in Mono doesn't update the UI elements 
            // properly, so do it manually
            projectBindingSrc.ResetBindings(false);
        }


        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (BrcViewModel)sender;

            if (vm.IsBusy)
            {
                renderAllButton.Text = "Stop Render";
                renderAllButton.Image = Resources.stop_icon;

                if (!projectBindingSrc.IsBindingSuspended)
                    projectBindingSrc.SuspendBinding();
            }
            else
            {
                renderAllButton.Text = "Start Render";
                renderAllButton.Image = Resources.render_icon;

                if (projectBindingSrc.IsBindingSuspended)
                    projectBindingSrc.ResumeBinding();
            }

            renderAllButton.Enabled = vm.CanRender;


            unloadToolStripMenuItem.Enabled = vm.CanEditCurrentProject;

            miRenderMixdown.Enabled =
            miJoinChunks.Enabled = !vm.IsBusy;

            miSettings.Enabled = !vm.IsBusy;

            miReloadCurrent.Enabled =
            reloadTSButton.Enabled = vm.CanReloadCurrentProject;

            frOutputFolder.Enabled = vm.CanEditCurrentProject;

            panelChunkSize.Enabled =
            panelFrameRange.Enabled = vm.CanEditCurrentProject;

            cbRenderer.Enabled =
            cbAfterRenderAction.Enabled = !vm.IsBusy;

            miOpenFile.Enabled =
            openFileTSButton.Enabled = vm.CanLoadNewProject;

            miOpenRecent.Enabled =
            openRecentsTSButton.Enabled = vm.CanLoadNewProject;


            Status(vm.DefaultStatusMessage);
        }



    }

}
