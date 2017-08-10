using BlenderRenderController.Properties;
using BRClib;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace BlenderRenderController
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class BrcForm : Form
    {
        // controls render all btn if it will stop current
        // or start new
        private bool _renderInProgress; 

        private int _autoRefStart, _autoRefEnd;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AppSettings _appSettings;
        private ProjectSettings _project;
        private BlendData _blendData;
        private int rendersInProgress, processesCompletedCount, _currentChunkIndex;
        private List<Process> _processList;
        private List<int> framesRendered;
        private AppStates appState = AppStates.AFTER_START;
        private DateTime startTime;
        private ETACalculator _etaCalc;
        private TaskbarManager _taskbar;

        public BrcForm()
        {
            InitializeComponent();

            _project = new ProjectSettings();
            _blendData = _project.BlendData;
            _processList = new List<Process>();
            framesRendered = new List<int>();
            _taskbar = TaskbarManager.Instance;
            _appSettings = AppSettings.Current;
        }

        private void BrcForm_Load(object sender, EventArgs e)
        {
            //add version numbers to window title
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            versionLabel.Text = " v" + assemblyVersion.Split('.')[0] + "." 
                                    + assemblyVersion.Split('.')[1]
                                    + "." + assemblyVersion.Split('.')[2];

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(onAppExit);

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

            UpdateUI();
            UpdateRecentBlendsMenu();

            // extra bindings
            totalStartNumericUpDown.DataBindings.Add("Enabled", startEndCustomRadio, "Checked");
            totalEndNumericUpDown.DataBindings.Add("Enabled", startEndCustomRadio, "Checked");

            chunkLengthNumericUpDown.DataBindings.Add("Enabled", renderOptionsCustomRadio, "Checked");
            processCountNumericUpDown.DataBindings.Add("Enabled", renderOptionsCustomRadio, "Checked");

            if (!_appSettings.CheckCorrectConfig())
            {
                var setForm = new SettingsForm();
                setForm.ShowInTaskbar = true;
                setForm.ShowDialog();
            }

            logger.Info("Program Started");
        }

        private void BrcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_renderInProgress)
            {
               var result = MessageBox.Show(
                            "Closing now will cancel rendering process. Close anyway?",
                            "Render in progress", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void onAppExit(object sender, EventArgs e)
        {
            StopRender(false);
            _appSettings.Save();
        }

        private void AppSettings_RecentBlends_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateRecentBlendsMenu();
        }

        private void Enter_GotoNext(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
                e.SuppressKeyPress = true; //disables sound
            }

        }

        private void GetBlendInfo(string blendFile, string scriptName = Constants.PyGetInfo)
        {
            logger.Info("Loading .blend");

            statusLabel.Text = "Reading the .blend file...";
            statusLabel.Update();

            if (!File.Exists(blendFile))
            {
                MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // remove and update recent blends...
                return;
            }
            if (!Directory.Exists(_appSettings.ScriptsFolder))
            {
                // Error scriptsfolder not found
                MessageBox.Show("Scripts folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #region getBlendInfo process

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

            #endregion

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

            #region Getting streams
            // Get values from streams
            var streamOutput = new List<string>();
            var streamErrors = new List<string>();

            while (!getBlendInfoCom.StandardOutput.EndOfStream)
                streamOutput.Add(getBlendInfoCom.StandardOutput.ReadLine());

            while (!getBlendInfoCom.StandardError.EndOfStream)
                streamErrors.Add(getBlendInfoCom.StandardError.ReadLine()); 
            #endregion

            // log errors
            if (streamErrors.Count > 0)
            {
                string allErrors = string.Join("\n", streamErrors);
                logger.Debug("Blender output errors detected.");
                logger.Trace(allErrors);
            }

            if (streamOutput.Count == 0)
            {
                var e = new Ui.ErrorBox("Could not open project, no information was received",
                                         streamErrors);
                e.Text = "Error";
                e.ShowDialog(this);
                StopRender(false);
                return;
            }
            // folder count exception
            if (streamErrors.Contains(Constants.PY_FolderCountError))
            {
                var err = new Ui.ErrorBox("There was an error parsing the output", streamErrors);
                err.Text = "Error";
                err.ShowDialog(this);
                return;
            }

            var blendData = Parsers.ParsePyOutput(streamOutput);
            _blendData =
            _project.BlendData = blendData;
            _project.BlendPath = blendFile;

            // save copy of start and end frames values
            _autoRefStart = blendData.Start;
            _autoRefEnd = blendData.End;

            // refresh binding source for blend data
            blendDataBindingSource.DataSource = _blendData;
            blendDataBindingSource.ResetBindings(false);

            var chunks = Chunk.CalcChunks(blendData.Start, blendData.End, 8);
            _project.ChunkLenght = (int)chunks.First().Length;

            if (blendData != null)
            {
                if (RenderFormats.IMAGES.Contains(blendData.RenderFormat))
                {
                    Helper.ShowErrors(MessageBoxIcon.Asterisk, AppErrorCode.RENDER_FORMAT_IS_IMAGE);
                }

                // output path w/o project name
                if (string.IsNullOrWhiteSpace(_blendData.OutputPath))
                {
                    // get .blend folder, if outputPath is unset, display a warning about it
                    Helper.ShowErrors(MessageBoxIcon.Information, AppErrorCode.BLEND_OUTPUT_INVALID);
                    _blendData.OutputPath = Path.GetDirectoryName(blendFile);
                }
                else
                    _blendData.OutputPath = Path.GetDirectoryName(_blendData.OutputPath);

                
                logger.Info(".blend loaded successfully");
            }
            else
                logger.Error(".blend was NOT loaded");


            UpdateCurrentChunks(chunks);
            _appSettings.AddRecentBlend(blendFile);
            appState = AppStates.READY_FOR_RENDER;
            statusLabel.Text = "Done";
            UpdateUI();
        }

        private void MixdownAudio(string mixdownScript = Constants.PyMixdown)
        {
            statusLabel.Text = "Rendering mixdown, it can take a while for larger projects...";
            statusLabel.Update();
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
                                           _blendData.Start,
                                           _blendData.End,
                                           Path.Combine(_appSettings.ScriptsFolder, mixdownScript),
                                           _blendData.OutputPath)
            };
            mixdownCom.StartInfo = info;
            #endregion

            Trace.WriteLine(mixdownCom.StartInfo.Arguments);

            try
            {
                mixdownCom.Start();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Mixdown cancelled.";
                var err = new Ui.ErrorBox("An unexpected error occurred", new string[] { ex.Message, "", ex.StackTrace });
                err.Text = "Error";
                err.ShowDialog();
                return;
            }


            string message = "Mixdown complete";
            Trace.WriteLine(message);
            logger.Info(message);
            statusLabel.Text = message;

        }

        List<string> GetChunkFiles(string chunksFolderPath)
        {
            string[] exts = RenderFormats.AllowedFormats;
            var dirFiles = Directory.GetFiles(chunksFolderPath, "*.*", SearchOption.TopDirectoryOnly).ToList();

            List<string> fileList = dirFiles
                .FindAll(f =>
                {
                    // skip '.' in ext
                    var ext = Path.GetExtension(f).Substring(1);
                    if (exts.Contains(ext))
                        return true;
                    else
                        return false;
                });

            var sortedFiles = fileList
                .FindAll(f =>
                {
                    // add only files w/ frame number
                    try{
                        string numStr = f.Split('-')[f.Split('-').Length - 2];
                        return int.TryParse(numStr, out int x);
                    }
                    catch (IndexOutOfRangeException)
                    { return false; }
                })
                .OrderBy(s => 
                {
                    //sort files in list by starting frame
                    string numStr = s.Split('-')[s.Split('-').Length - 2];
                    return int.Parse(numStr);
                });

            return sortedFiles.ToList();
        }

        private void Concatenate()
        {
            var chunksFolder = Helper.ParseChunksFolder(_blendData.OutputPath);

            if (!Directory.Exists(chunksFolder))
            {
                concatenatePartsButton.Enabled = false;
                return;
            }

            string chunksTxtPath = Helper.ParseChunksTxt(_blendData.OutputPath);
            string audioFileName = _blendData.ProjectName + "." + "ac3";
            string projPath = Path.Combine(_blendData.OutputPath, _blendData.ProjectName);
            string mixdownAudioPath = Path.Combine(_blendData.OutputPath, audioFileName);

            var fileListSorted = GetChunkFiles(chunksFolder);
            var videoExtensionFound = Path.GetExtension(fileListSorted.FirstOrDefault());

            if (string.IsNullOrEmpty(videoExtensionFound))
            {
                MessageBox.Show("Failed to parse video extention", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

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
                statusLabel.Text = "Joining chunks, please wait...";
                comArgs = string.Format(CommandARGS.GetConcatenateArgs(false),
                            chunksTxtPath,
                            projPath,
                            videoExtensionFound);
            }
            //mixdown audio found
            else
            {
                statusLabel.Text = "Joining chunks with mixdown audio...";
                comArgs = string.Format(CommandARGS.GetConcatenateArgs(true),
                            chunksTxtPath,
                            mixdownAudioPath,
                            projPath,
                            videoExtensionFound);
            }

            #region Concatenate Command
            var concatenateCom = new Process();
            var info = new ProcessStartInfo();
            info.FileName = _appSettings.FFmpegProgram;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.Arguments = comArgs;
            concatenateCom.StartInfo = info;
            concatenateCom.Exited += (pSender, pArgs) =>
            {
                logger.Info("FFmpeg exited");
            };
            #endregion

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
                Helper.ShowErrors(MessageBoxIcon.Error, AppErrorCode.FFMPEG_PATH_NOT_SET);
                statusLabel.Text = "Joining cancelled.";
                return;
            }

            var msg = "Chunks Joined.";
            Status(msg);
            logger.Info(msg);
        }

        private void RenderChunk(Chunk chunk)
        {
            var chunksFolder = Helper.ParseChunksFolder(_blendData.OutputPath);

            #region Render Com

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

            renderCom.ErrorDataReceived += (ps, pe) => Console.WriteLine(pe.Data);
            renderCom.OutputDataReceived += OnRenderProcessDataRecived;

            renderCom.Exited += (pSender, pArgs) =>
            {
                logger.Info($"Render done {rendersInProgress}/{_project.ProcessesCount}");
                rendersInProgress--;
                processesCompletedCount++;
            };
            
            #endregion

            try
            {
                renderCom.Start();
                rendersInProgress++;
                _processList.Add(renderCom);
                logger.Info($"Render {rendersInProgress}/{_project.ProcessesCount} in process");
                //renderCom.WaitForExit();
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

        private void OnRenderProcessDataRecived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.IndexOf("Fra:") == 0)
            {
                int frameBeingRendered = int.Parse(e.Data.Split(' ')[0].Replace("Fra:", ""));
                var frame = e.Data.Split(' ')[0].Replace("Fra:", "");
                //Console.Write("\r{0}    ", frame);
                //Console.WriteLine(e.Data.Split(' ')[0].Replace("Fra:", ""));
                if (!framesRendered.Contains(frameBeingRendered))
                {
                    framesRendered.Add(frameBeingRendered);
                }
            }
        }

        private void Status(string msg)
        {
            statusLabel.Text = msg;
            //statusLabel.Invalidate();
            statusLabel.Refresh();
        }

        private void UpdateCurrentChunks(params Chunk[] newChunks)
        {
            if (_project.ChunkList.Count > 0)
                _project.ChunkList.Clear();

            foreach (var chnk in newChunks)
            {
                _project.ChunkList.Add(chnk);
            }

            //chunkLengthNumericUpDown.Value = _project.ChunkList.First().Length;
        }

        private void UpdateRecentBlendsMenu()
        {
            //last blends
            recentBlendsMenu.Items.Clear();

            foreach (string item in _appSettings.RecentBlends)
            {
                var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(item), Resources.blend_icon);
                menuItem.ToolTipText = item;
                menuItem.Click += (sender, args) =>
                {
                    ToolStripMenuItem recentItem = (ToolStripMenuItem)sender;
                    var blendPath = recentItem.ToolTipText;
                    GetBlendInfo(blendPath);
                };
                recentBlendsMenu.Items.Add(menuItem);
            }
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

        private void AutoOptionsRadio_CheckedChanged(object sender, EventArgs e)
        {
            var radio = sender as RadioButton;

            if (radio.Name == renderOptionsAutoRadio.Name)
            {
                if (radio.Checked)
                {
                    _project.ProcessesCount = Environment.ProcessorCount;
                    // recalc auto chunks:
                    var autoChunks = Chunk.CalcChunks(_project.BlendData.Start, _project.BlendData.End, _project.ProcessesCount);
                    chunkLengthNumericUpDown.Value = autoChunks.First().Length;
                }
            }
            else if (radio.Name == startEndBlendRadio.Name)
            {
                if (radio.Checked)
                {
                    // set to blend values
                    _blendData.Start = _autoRefStart;
                    _blendData.End = _autoRefEnd;
                }
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

        private void renderAllButton_Click(object sender, EventArgs e)
        {
            if (_renderInProgress)
            {
                var result = MessageBox.Show("Are you sure you want to stop?",
                                                "",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Exclamation);

                if (result == DialogResult.No) return;
                StopRender(false);
                logger.Warn("RENDER ABORTED");

            }
            else 
            {
                var chunksFolder = Helper.ParseChunksFolder(_blendData.OutputPath);

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
                    catch (IOException ex)
                    {
                        logger.Error("Can't clear chunk folder, files are in use");
                        MessageBox.Show("It can't be deleted, files are in use by some program.\n");
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        logger.Trace(ex.StackTrace);
                        MessageBox.Show("An unexpected error ocurred, sorry.");
                        return;
                    }

                }

                RenderAll();
                logger.Info("RENDER STARTED");
            }


        }

        private void RenderAll()
        {
            appState = AppStates.RENDERING_ALL;
            startTime = DateTime.Now;
            framesRendered.Clear();
            Status("Starting render...");
            processesCompletedCount = 0;
            rendersInProgress = 0;
            _currentChunkIndex = 0;
            _renderInProgress = true;
            processManager.Enabled = true;

            //render ETA reset
            totalTimeLabel.Text = Helper.SecondsToString(0, true);
            _etaCalc = new ETACalculator(5, 1);

            _taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate);
            _taskbar.SetProgressValue(0, 100);

            UpdateUI();
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
            _renderInProgress = false;
            renderProgressBar.Style = ProgressBarStyle.Blocks;
            renderProgressBar.Value = 0;
            renderProgressBar.Refresh();

            ETALabel.Text =
            totalTimeLabel.Text = Helper.SecondsToString(0, true);

            Text = Constants.APP_TITLE;

            _taskbar.SetProgressState(TaskbarProgressBarState.NoProgress);

            appState = AppStates.READY_FOR_RENDER;

            UpdateUI();
        }

        private void AfterRender()
        {
            // turn manager off
            //processManager.Enabled = false;
            bool wasComplete = (framesRendered.Count > Math.Round(Convert.ToDouble(_blendData.TotalFrames)) * 0.75);

            if (wasComplete)
            {
                //if we rendered the project (not chunk only)
                //and some of automatic join checkboxes is checked
                //we continue with join (with mixdown if the checkbox is checked)
                if (appState == AppStates.RENDERING_ALL)
                {
                    if (_appSettings.AfterRender == AfterRenderAction.JOIN_MIXDOWN)
                    {
                        //await MixdownAudio();
                        MixdownAudio();
                    }

                    //await Concatenate();
                    Concatenate();
                    StopRender(true);

                    var openOutputFolderQuestion =
                        MessageBox.Show("Open the folder with the video?",
                                        "Open folder",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                    if (openOutputFolderQuestion == DialogResult.Yes)
                        Process.Start(_blendData.OutputPath);
                }
                else
                    StopRender(true);
            }
            else
                StopRender(false);


            UpdateUI();
        }

        /// <summary>
        /// This method is looped by <see cref="processManager"/> and controls what
        /// processes get started for rendering and when to call <see cref="AfterRender"/>
        /// </summary>
        private void UpdateProcessManagement(object sender, EventArgs e)
        {
            // if anything goes wrong, make sure rendering stops
            try
            {
                Chunk currentChunk;
                UpdateProgress();
                if (_currentChunkIndex < _project.ChunkList.Count)
                {
                    currentChunk = _project.ChunkList[_currentChunkIndex];

                    if (_processList.Count < _project.ProcessesCount)
                    {
                        RenderChunk(currentChunk);
                        _currentChunkIndex++;
                    }

                }

                // do after render once all processes exits
                if (_processList.All(p => p.HasExited == true))
                {
                    renderProgressBar.Style = ProgressBarStyle.Marquee;
                    AfterRender();
                }
            }
            catch (Exception)
            {
                StopRender(false);
                throw;
            }
        }

        void UpdateProgress()
        {

            int progressPercentage = (int)Math.Floor((framesRendered.Count / (decimal)_blendData.TotalFrames) * 100);
            var chunksTotalCount = Math.Ceiling((decimal)_blendData.TotalFrames / _project.ChunkLenght);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Completed {0} / {1}", processesCompletedCount, chunksTotalCount);
            sb.AppendFormat(" chunks, rendered {0} frames in {1} processes", framesRendered.Count, _processList.Count);
            Status(sb.ToString());

            // taskbar progress
            _taskbar.SetProgressValue(progressPercentage, 100);

            //progress bar
            if (progressPercentage > 100)
                throw new Exception("Progress is over 100%");

            else
                renderProgressBar.Value = progressPercentage;

            _etaCalc.Update(progressPercentage / 100f);

            if (_etaCalc.ETAIsAvailable)
                ETALabel.Text = Helper.SecondsToString(_etaCalc.ETR.TotalSeconds, true);

            ETALabel.Refresh();

            //time elapsed display
            TimeSpan runTime = DateTime.Now - startTime;
            string lastTotalTimeText = totalTimeLabel.Text;
            totalTimeLabel.Text = Helper.SecondsToString(runTime.TotalSeconds, true);

            //title progress
            Text = progressPercentage.ToString() + "% rendered - " + Constants.APP_TITLE;

        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            var outputFolderBrowseDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = _blendData.OutputPath,
            };
            
            var result = outputFolderBrowseDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                _blendData.OutputPath = outputFolderBrowseDialog.SelectedPath;
                UpdateUI();
            }
        }

        private void StartEndNumeric_Changed(object sender, EventArgs e)
        {
            if (renderOptionsAutoRadio.Checked)
            {
                var autoChunks = Chunk.CalcChunks(_project.BlendData.Start, _project.BlendData.End, _project.ProcessesCount);
                _project.ChunkLenght = (int)autoChunks.First().Length;
                AutoOptionsRadio_CheckedChanged(renderOptionsAutoRadio, EventArgs.Empty);
                UpdateCurrentChunks(autoChunks);
            }
        }

        private void mixDownButton_Click(object sender, EventArgs e)
        {
            MixdownAudio();
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void toolStripMenuItemBug_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/jendabek/BlenderRenderController/issues");
        }

        private void concatenatePartsButton_Click(object sender, EventArgs e)
        {
            Concatenate();
        }

        private IEnumerable<Control> FindControlsByTag(Control.ControlCollection controls, string key)
        {
            List<Control> controlsWithTags = new List<Control>();

            foreach (Control c in controls)
            {
                if (c.Tag != null)
                {
                    // splits tag content into string array
                    string[] tags = c.Tag.ToString().Split(Constants.TAG_SEP);

                    // if key maches, add to list
                    if (tags.Contains(key))
                        controlsWithTags.Add(c);
                }

                if (c.HasChildren)
                {
                    //Recursively check all children controls as well; ie groupboxes or tabpages
                    controlsWithTags.AddRange(FindControlsByTag(c.Controls, key)); 
                }
            }

            return controlsWithTags;
        }

        private void clearRecentProjectsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _appSettings.ClearRecentBlend();
            UpdateRecentBlendsMenu();
        }

        void UpdateUI()
        {

            renderAllButton.Text = (appState == AppStates.RENDERING_ALL)
                                    ? "Stop Render"
                                    : "Start Render";

            renderAllButton.Image = (appState == AppStates.RENDERING_ALL)
                                    ? Resources.stop_icon_small
                                    : Resources.render_icon_small;

            // the world's longest switch block!
            // enabling / disabling UI according to current app state
            switch (appState)
            {
                case AppStates.AFTER_START:
                    blendNameLabel.Visible = false;
                    renderAllButton.Enabled = false;
                    menuStrip.Enabled = true;
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
                    timeElapsedLabel.Visible = false;
                    ETALabel.Visible = ETALabelTitle.Visible = false;
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
                    break;
                case AppStates.NOT_CONFIGURED:
                    renderAllButton.Enabled = false;
                    menuStrip.Enabled = true;
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
                    timeElapsedLabel.Visible = false;
                    ETALabel.Visible = ETALabelTitle.Visible = false;
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
                    break;
                case AppStates.READY_FOR_RENDER:
                    blendNameLabel.Visible = true;
                    renderAllButton.Enabled = true;
                    menuStrip.Enabled = true;
                    mixDownButton.Enabled = true;
                    //totalStartNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    //totalEndNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    //chunkLengthNumericUpDown.Enabled = renderOptionsCustomRadio.Checked;
                    //processCountNumericUpDown.Enabled = renderOptionsCustomRadio.Checked;
                    concatenatePartsButton.Enabled = Directory.Exists(Helper.ParseChunksFolder(_blendData.OutputPath));
                    reloadBlenderDataButton.Enabled = true;
                    blendBrowseBtn.Enabled = true;
                    showRecentBlendsBtn.Enabled = true;
                    outputFolderBrowseButton.Enabled = true;
                    outputFolderTextBox.Enabled = true;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    ETALabel.Visible = ETALabelTitle.Visible = true;
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
                    break;
                case AppStates.RENDERING_ALL:
                    menuStrip.Enabled = false;
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
                    timeElapsedLabel.Visible = true;
                    ETALabel.Visible = ETALabelTitle.Visible = true;
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
