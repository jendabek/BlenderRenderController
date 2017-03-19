using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Reflection;

using BlenderRenderController.newLogger;

namespace BlenderRenderController
{
    public partial class MainForm : Form
    {
        bool lastChunkStarted = false;

        string appState = AppStates.AFTER_START;

        //processes
        List<Process> processes = new List<Process>();
        List<int> framesRendered = new List<int>();
        int framesRenderedCount_PrevSecond = 0;
        int processesCompletedCount = 0;
        BlendData blendData;
        ProjectData p;
        DateTime startTime;
        Timer processTimer;
        SettingsForm settingsForm;
        AppSettings appSettings;
        ContextMenuStrip recentBlendsMenu;
        List<int> renderingSpeedsFPS = new List<int>();
        LogService _log = new LogService();

        // CMD args
        string[] CMDargs = Environment.GetCommandLineArgs();

        public MainForm()
        {
            InitializeComponent();
        }
        public void MainForm_Shown(object sender, EventArgs e)
        {
            
            //add version numbers to window title
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            versionLabel.Text = " v" + assemblyVersion.Split('.')[0] + "." + assemblyVersion.Split('.')[1] + "." + assemblyVersion.Split('.')[2];

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(onAppExit);


            settingsForm = new SettingsForm();
            appSettings = new AppSettings();
            p = new ProjectData();

            appSettings.settingsForm = settingsForm;
            appSettings.init();

            settingsForm.FormClosed += new FormClosedEventHandler(onSettingsFormClosed);
            settingsForm.init(appSettings);

            p.chunkLength = chunkLengthNumericUpDown.Value;
            p.start = totalStartNumericUpDown.Value;
            p.end = totalEndNumericUpDown.Value;
            statusLabel.Text = "Hello 3D world!";

            Text = AppSettings.APP_TITLE;
            
            processTimer = new Timer();
            processTimer.Interval = appSettings.processCheckInterval;
            processTimer.Tick += new EventHandler(updateProcessManagement);

            recentBlendsMenu = new ContextMenuStrip();
            blendFileBrowseButton.Menu = recentBlendsMenu;

            // initialize logger service
            _log.RegisterLogSevice(new FileLogger());
            _log.RegisterLogSevice(new ConsoleLogger());


            //_fileLog = new newLogger.FileLogger(appSettings.verboseLog);
            
            applySettings();
            if (!appSettings.appConfigured)
            {
                //appState = AppStates.NOT_CONFIGURED;/
                settingsForm.ShowDialog();
            }
            updateRecentBlendsMenu();
            updateUI();
            _log.Info("Program Started");
        }

        private void onSettingsFormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void applySettings()
        {
            renderOptionsRadio_CheckedChanged(null, null);
            p.afterRenderAction = appSettings.afterRenderAction;

            //tooltips
            toolTipInfo.Active =
            toolTipWarn.Active =
            renderInfoLabel.Visible =
            tipsToolStripMenuItem.Checked =
            appSettings.displayTooltips;

            switch (p.afterRenderAction)
            {
                case AppStrings.AFTER_RENDER_JOIN_MIXDOWN:
                    afterRenderJoinMixdownRadio.Checked = true;
                    break;
                case AppStrings.AFTER_RENDER_JOIN:
                    afterRenderJoinRadio.Checked = true;
                    break;
                case AppStrings.AFTER_RENDER_NOTHING:
                    afterRenderDoNothingRadio.Checked = true;
                    break;
            }

            //renderer
            p.renderer = appSettings.renderer;
            if (p.renderer == AppStrings.RENDERER_BLENDER)
            {
                rendererRadioButtonBlender.Checked = true;
            } else
            {
                rendererRadioButtonCycles.Checked = true;
            }
        }

        private void onAppExit(object sender, EventArgs e)
        {
            stopRender(false);
            appSettings.save();
        }

        public void updateRecentBlendsMenu()
        {
            //last blends
            recentBlendsMenu.Items.Clear();
            foreach (string item in appSettings.recentBlends)
            {
                var menuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(item), Properties.Resources.blend_icon);
                menuItem.ToolTipText = item;
                menuItem.Click += new EventHandler(recentBlendsMenuItem_Click);
                recentBlendsMenu.Items.Add(menuItem);
            }
        }

        private void recentBlendsMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem) sender;
            p.blendFilePath = item.ToolTipText;
            loadBlend();
        }

        public void updateUI()
        {
            chunkLengthNumericUpDown.Value = p.chunkLength;
            totalStartNumericUpDown.Value = p.start;
            totalEndNumericUpDown.Value = p.end;
            
            //top infos
            if (blendData != null)
            {
                var durationSeconds = (Convert.ToDouble(p.end - p.start + 1) / p.fps);
                infoDuration.Text = Helper.secondsToString(durationSeconds, false);
                infoFramesTotal.Text = (p.end - p.start + 1).ToString();
            }

            renderAllButton.Text = (appState == AppStates.RENDERING_CHUNK_ONLY || appState == AppStates.RENDERING_ALL) ? "Stop Render" : "Start Render";
            
            //enabling / disabling UI according to current app state
            switch (appState)
            {
                case AppStates.AFTER_START:
                    renderAllButton.Enabled = false;
                    menuStrip.Enabled = true;
                    blendFileBrowseButton.Enabled = true;
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
                    renderAllButton.Image = Properties.Resources.render_icon_small;
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
                    blendFileBrowseButton.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    openOutputFolderButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = false;
                    ETALabel.Visible = ETALabelTitle.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    renderAllButton.Image = Properties.Resources.render_icon_small;
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
                    renderAllButton.Enabled = true;
                    menuStrip.Enabled = true;
                    mixDownButton.Enabled = true;
                    totalStartNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    totalEndNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    chunkLengthNumericUpDown.Enabled = renderOptionsCustomRadio.Checked;
                    processCountNumericUpDown.Enabled = renderOptionsCustomRadio.Checked;
                    concatenatePartsButton.Enabled = Directory.Exists(p.chunksPath);
                    reloadBlenderDataButton.Enabled = true;
                    blendFileBrowseButton.Enabled = true;
                    outputFolderBrowseButton.Enabled = true;
                    outputFolderTextBox.Enabled = true;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    ETALabel.Visible = ETALabelTitle.Visible = true;
                    totalTimeLabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    renderAllButton.Image = Properties.Resources.render_icon_small;
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
                case AppStates.RENDERING_CHUNK_ONLY:
                    renderAllButton.Enabled = true;
                    menuStrip.Enabled = false;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    blendFileBrowseButton.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    ETALabel.Visible = ETALabelTitle.Visible = true;
                    rendererRadioButtonBlender.Enabled = false;
                    rendererRadioButtonCycles.Enabled = false;
                    renderAllButton.Image = Properties.Resources.stop_icon_small;
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

        // Deletes json on form close
        private void MainForm_Close(object sender, FormClosedEventArgs e)
        {
            //jsonDel();
            _log.Info("Program Closed");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Arguments
           /* if (CMDargs.Length > 1)
            {
                //test arguments
                //for (int i = 0; i < args.Length; i++)
                //{
                //    string teste = string.Format("Arg[{0}] = [{1}] \r\n", i, args[i]);
                //    MessageBox.Show(teste);
                //}

                var CMDpath = CMDargs[1];
                p.blendFilePath = CMDpath;
                //blendFilePathTextBox.Text = p.blendFilePath;
                loadBlend();
            }
            */
        }

        private void blendFileBrowseButton_Click(object sender, EventArgs e)
        {
            var blendFileBrowseDialog = new OpenFileDialog();
            blendFileBrowseDialog.Filter = "Blend|*.blend";

            var result = blendFileBrowseDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                p.blendFilePath = Path.GetFullPath(blendFileBrowseDialog.FileName);
                loadBlend();
            }
        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = p.outputPath;
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                p.outputPath = outputFolderTextBox.Text = Path.GetFullPath(dialog.SelectedPath);
                p.chunksPath = Path.Combine(p.outputPath, appSettings.chunksSubfolder);
                updateUI();
            }
        }

        private void outputFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {
            outputFolderTextBox.Text = Helper.fixPath(outputFolderTextBox.Text);
            
            try {
                Path.GetFullPath(outputFolderTextBox.Text);
            }
            catch (Exception)
            {
                outputFolderTextBox.Text = p.outputPath;
                return;
            }
            p.outputPath = outputFolderTextBox.Text;
            p.chunksPath = Path.Combine(p.outputPath, appSettings.chunksSubfolder);
            updateUI();
        }

        //confirm text input by enter & go to next control
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
                e.SuppressKeyPress = true; //disables sound
            }
        }

        private void renderChunkButton_Click(object sender, EventArgs e)
        {
            appState = AppStates.RENDERING_CHUNK_ONLY;
            startTime = DateTime.Now;
            totalTimeLabel.Text = "00:00:00";
            statusLabel.Text = "Starting render...";
            processesCompletedCount = 0;
            lastChunkStarted = true;
            processTimer.Enabled = true;
            framesRendered.Clear();
            renderCurrentChunk();
            updateUI();
        }
        private void renderCurrentChunk()
        {
            if (p.chunkEnd == p.end) lastChunkStarted = true;

            Process process = new Process();

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = appSettings.blenderPath;
            process.StartInfo.FileName = Path.Combine(appSettings.blenderPath, "blender.exe");
            
            process.StartInfo.Arguments = String.Format("-b \"{0}\" -o \"{1}\" -E {2} -s {3} -e {4} -a",
                                                  p.blendFilePath,
                                                  Path.Combine(p.chunksPath, blendData.projectName) + "-#",
                                                  p.renderer,
                                                  p.chunkStart,
                                                  p.chunkEnd
                                                  );
            process.ErrorDataReceived += onRenderProcessErrorDataReceived;
            process.OutputDataReceived += onRenderProcessDataReceived;
            process.EnableRaisingEvents = true;

            Trace.WriteLine(String.Format("CEW: {0}", process.StartInfo.Arguments));
            
            process.Exited += new EventHandler(chunkRendered);

            try
            {
                process.Start();
                processes.Add(process);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                //oldLogger.add(ex.ToString());
                _log.Error(ex.ToString());
                stopRender(false);
                return;
            }
            //process.PriorityClass = ProcessPriorityClass.High;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void onRenderProcessDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine("Output from other process");
            if(e.Data != null && e.Data.IndexOf("Fra:") == 0)
            {
                int frameBeingRendered = int.Parse(e.Data.Split(' ')[0].Replace("Fra:", ""));
                Console.WriteLine(e.Data.Split(' ')[0].Replace("Fra:", ""));
                if(!framesRendered.Contains(frameBeingRendered))
                {
                    framesRendered.Add(frameBeingRendered);
                }
            }
            //Console.WriteLine(e.Data);
        }

        private void onRenderProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //Console.WriteLine("Error from other process");
            Console.WriteLine(e.Data);
        }

        private void chunkRendered(object sender, EventArgs e)
        {
            processes.Remove((Process) sender);
            processesCompletedCount++;
        }

        private void prevChunkButton_Click(object sender, EventArgs e)
        {
            if (p.chunkStart - p.chunkLength - 1 < p.start)
            {
                p.chunkStart = p.start;
                p.chunkEnd = p.start + p.chunkLength - 1;
            }
            else
            {
                p.chunkEnd = p.chunkStart - 1;
                p.chunkStart = p.chunkEnd - p.chunkLength + 1;
            }
            updateUI();
        }

        private void moveToNextChunk()
        {
            //start of next chunk must not be above totalEnd, so we can move to another chunk
            if (!(p.chunkEnd + 1 > p.end))
            {
                p.chunkStart = p.chunkEnd + 1;

                if (p.chunkEnd + p.chunkLength - 1 > p.end)
                {
                    p.chunkEnd = p.end;
                }
                else
                {
                    p.chunkEnd += p.chunkLength;
                }
                updateUI();
                
            }
        }

        private void nextChunkButton_Click(object sender, EventArgs e)
        {
            moveToNextChunk();
        }

        private void renderAllButton_Click(object sender, EventArgs e)
        {
            //we are stopping
            if (processTimer.Enabled) {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to stop?",
                                                                "",
                                                                MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.No) return;
                stopRender(false);
            }

            //we want to start render
            else {
                if (Directory.Exists(p.chunksPath) && Directory.GetFiles(p.chunksPath).Length > 0)
                {
                    // Configure the message box to be displayed
                    DialogResult dialogResult = MessageBox.Show("All previously rendered chunks will be deleted.\nDo you want to continue?",
                                                                "Chunks folder not empty",
                                                                MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Exclamation);

                    if (dialogResult == DialogResult.No) return;

                    try {
                        Helper.clearFolder(p.chunksPath);
                    }
                    catch (Exception ex){
                        //oldLogger.add(ex.ToString());
                        _log.Error(ex.ToString());
                        MessageBox.Show("It can't be deleted, files are in use by some program.\n");
                        return;
                    }
                    renderAllButton_Click(null, null);
                }
                renderAll();
            }
        }
        
        private void renderAll()
        {
            appState = AppStates.RENDERING_ALL;
            startTime = DateTime.Now;
            renderingSpeedsFPS.Clear();
            statusLabel.Text = "Starting render...";
            processesCompletedCount = 0;
            framesRendered.Clear();
            processTimer.Enabled = true;

            //render ETA reset
            totalTimeLabel.Text = Helper.secondsToString(0, true);
            ETALabel.Text = Helper.secondsToString(0, true);
            framesRenderedCount_PrevSecond = 0;

            //taskbar progress
            try
            {
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.Indeterminate);
                TaskbarProgress.SetValue(Handle, 0, 100);
            }
            catch (Exception)
            {
                
            }

            updateUI();
        }

        private decimal getChunksTotalCount()
        {
            return Math.Ceiling((p.end - p.start + 1) / p.chunkLength);
        }

        private void stopRender(bool wasComplete)
        {
            statusLabel.Text = wasComplete ? "Render complete." : "Render cancelled.";
            
            foreach (var process in processes.ToList())
            {
                try
                {
                    if (process != null && !process.HasExited)
                    {
                        process.Kill();
                    }
                    process.Dispose();
                }
                catch(Exception ex)
                {
                    _log.Error(ex.ToString());
                    //oldLogger.add(ex.ToString());
                    Trace.WriteLine(ex);
                }
                processes.Remove(process);
            }

            processTimer.Enabled = false;
            lastChunkStarted = false;
            renderProgressBar.Value = 0;
            Text = AppSettings.APP_TITLE;

            try
            {
                TaskbarProgress.SetState(Handle, TaskbarProgress.TaskbarStates.NoProgress);
            }
            catch (Exception)
            {
            }

            appState = AppStates.READY_FOR_RENDER;

            p.chunkStart = p.start;
            updateCurrentChunkStartEnd();

            updateUI();
        }

        private void updateProcessManagement(object sender, EventArgs e)
        {
            //PROGRESS display
            int progressPercentage = 0;
            if (appState == AppStates.RENDERING_ALL)
            {
                progressPercentage = (int)Math.Floor((framesRendered.Count / (p.end - p.start + 1)) * 100);

                var statusText = "";
                statusText = "Completed " + processesCompletedCount.ToString() + " / " + getChunksTotalCount().ToString();
                statusText += " chunks, rendered " + framesRendered.Count + " frames in " + processes.Count;
                statusText += (processes.Count > 1) ? " processes." : " process.";
                statusLabel.Text = statusText;
            }
            else
            {
                progressPercentage = (int)Math.Floor((framesRendered.Count / (p.chunkEnd - p.chunkStart + 1)) * 100);
                
                if (framesRendered.Count > 0)
                {
                    statusLabel.Text = "Rendering chunk frame " + framesRendered.ElementAt(framesRendered.Count - 1) + ".";
                }
            }
            //taskbar progress
            try
            {
                TaskbarProgress.SetValue(Handle, progressPercentage, 100);
            }
            catch (Exception)
            {

            }
            //progress bar
            renderProgressBar.Value = progressPercentage;

            //title progress
            Text = progressPercentage.ToString() + "% rendered - " + AppSettings.APP_TITLE;

            //start next chunk if needed
            if (!lastChunkStarted)
            {
                if (processes.Count < p.processCount)
                {
                    renderCurrentChunk();
                    moveToNextChunk();
                }
            }

            //time elapsed display
            TimeSpan runTime = DateTime.Now - startTime;
            string lastTotalTimeText = totalTimeLabel.Text;
            totalTimeLabel.Text = Helper.secondsToString(runTime.TotalSeconds, true);

            //ESTIMATED TIME
            //-----
            if (lastTotalTimeText != totalTimeLabel.Text) //amateurish way to run once a second without an additional timer
            {
                //we add rendering speed in previous second to renderingSpeedsFPS list
                renderingSpeedsFPS.Add(framesRendered.Count - framesRenderedCount_PrevSecond);

                //removing the most latest speed from the list if the list is larger than appSettings.renderETAFromSecondsAgo
                if (renderingSpeedsFPS.Count > appSettings.renderETAFromSecondsAgo)
                {
                    renderingSpeedsFPS.RemoveAt(0);
                }
                framesRenderedCount_PrevSecond = framesRendered.Count;
                
                //computing speed average based on speed in previous seconds (stored in renderingSpeedsFPS)
                int speeds = 0;
                foreach (int speed in renderingSpeedsFPS)
                {
                    speeds += speed;
                }
                int speedAverage = speeds / renderingSpeedsFPS.Count;

                //getting remaining time & displaying it
                if(speedAverage > 0)
                {
                    int framesRemaining = (int)(p.end - p.start) - framesRendered.Count;
                    int secondsRemaining = framesRemaining / speedAverage;
                    ETALabel.Text = Helper.secondsToString(secondsRemaining, true);
                }
            }

            if (processes.Count == 0)
            {
                afterRender();
            }
        }
        private void afterRender()
        {
            //bool wasComplete = (framesRendered.Count == p.end - p.start + 1);
            bool wasComplete = (framesRendered.Count > Math.Round(Convert.ToDouble(p.end - p.start + 1)) * 0.75);

            if (wasComplete)
            {
                //if we rendered the project (not chunk only)
                //and some of automatic join checkboxes is checked
                //we continue with join (with mixdown if the checkbox is checked)
                if (appState == AppStates.RENDERING_ALL && (p.afterRenderAction == AppStrings.AFTER_RENDER_JOIN_MIXDOWN || p.afterRenderAction == AppStrings.AFTER_RENDER_JOIN))
                {
                    if (p.afterRenderAction == AppStrings.AFTER_RENDER_JOIN_MIXDOWN)
                    {
                        mixdown();
                    }
                    concatenate();
                    stopRender(true);

                    DialogResult openOutputFolderQuestion = MessageBox.Show("Open the folder with the video?",
                           "Open folder",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question);
                    if (openOutputFolderQuestion == DialogResult.Yes)
                    {
                        openOutputFolder();
                    }
                } else
                {
                    stopRender(true);
                }
            }
            else
            {
                stopRender(false);
            }
            updateCurrentChunkStartEnd();
            updateUI();
        }
        
        private void concatenate()
        {
            if (!Directory.Exists(p.chunksPath))
            {
                concatenatePartsButton.Enabled = false;
                return;
            }

            string chunksTxtPath = Path.Combine(p.chunksPath, appSettings.chunksTxtFileName);
            string audioFileName = blendData.projectName + "." + appSettings.audioFormat;
            string audioSettings = string.Empty; //"-c:a aac -b:a 256k";
            string videoExtensionFound = "";

            List<string> fileList = new List<string>();
            foreach (var format in appSettings.allowedFormats)
            {
                videoExtensionFound = format;
                fileList = Directory.GetFiles(p.chunksPath, "*." + videoExtensionFound, SearchOption.TopDirectoryOnly).ToList();
                if (fileList.Count > 0) break;
            }

            // no chunks found
            if (fileList.Count == 0) return;
            
            string[] pathSplitted;

            //add only correct videos to list
            List<string> fileListFiltered = new List<string>();

            for (int i = 0; i < fileList.Count; i++)
            {
                //does not contain frame number, we do not add it
                pathSplitted = fileList[i].Split('-');
                int x;
                if (int.TryParse(pathSplitted[pathSplitted.Length - 2], out x) == false) {
                    continue;
                }
                fileListFiltered.Add(fileList[i]);
            }

            //sort files in list by starting frame
            var fileListSorted = fileListFiltered.OrderBy(s =>
            {
                pathSplitted = s.Split('-');
                return Int32.Parse(pathSplitted[pathSplitted.Length - 2]);
            });

            //write txt for FFmpeg concatenation
            StreamWriter partListWriter = new StreamWriter(chunksTxtPath);
            foreach (var filePath in fileListSorted)
            {
                partListWriter.WriteLine("file '{0}'", filePath);
            }
            partListWriter.Close();

            var audioArguments = "";
            
            //mixdown audio NOT found
            if (!File.Exists(Path.Combine(p.outputPath, audioFileName)))
            {
                statusLabel.Text = "Joining chunks, please wait...";
                audioFileName = string.Empty;
                audioSettings = string.Empty;
            }
            //mixdown audio found
            else
            {
                statusLabel.Text = "Joining chunks with mixdown audio...";
                audioArguments = "-i \"" + Path.Combine(p.outputPath, audioFileName) + "\" -map 0:v -map 1:a";
            }
            Process process = new Process();
            process.StartInfo.WorkingDirectory = appSettings.ffmpegPath;
            process.StartInfo.FileName = Path.Combine(appSettings.ffmpegPath, "ffmpeg.exe");
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = 
                String.Format("-f concat -safe 0 -i \"{0}\" {1} -c:v copy \"{3}.{4}\" -y",
                chunksTxtPath,
                audioArguments,
                audioSettings,
                Path.Combine(p.outputPath, blendData.projectName),
                videoExtensionFound
            );
            Trace.WriteLine(process.StartInfo.Arguments);

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                //oldLogger.add(ex.ToString());
                _log.Error(ex.ToString());
                Helper.showErrors(new List<string> { AppErrorCodes.FFMPEG_PATH_NOT_SET });
                settingsForm.ShowDialog();
                statusLabel.Text = "Joining cancelled.";
                return;
            }
            statusLabel.Text = "Chunks Joined.";
        }

		private void loadBlend() {

            statusLabel.Text = "Reading the .blend file...";
            statusLabel.Update();

            if ( !File.Exists(p.blendFilePath) ) {
                var errors = new List<string>();
                errors.Add(AppErrorCodes.BLEND_FILE_NOT_EXISTS);
                Helper.showErrors(errors, MessageBoxIcon.Exclamation);

                appSettings.recentBlends.Remove(p.blendFilePath);
                updateRecentBlendsMenu();
                return;
			}

            if (!Directory.Exists(appSettings.scriptsPath))
            {
                // Error scriptsfolder not found
                string caption = "Error";
                string message = "Scripts folder not found.";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process process = new Process();
            process.StartInfo.WorkingDirectory       = appSettings.blenderPath;
            process.StartInfo.FileName               = Path.Combine(appSettings.blenderPath, "blender.exe");
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow         = true;
			process.StartInfo.UseShellExecute        = false;

            process.StartInfo.Arguments = String.Format("-b \"{0}\" -P \"{1}\"",
                                                  p.blendFilePath,
                                                  Path.Combine(appSettings.scriptsPath, "get_project_info.py")
                                    );
            Trace.WriteLine(appSettings.blenderPath);
            try {
				process.Start();
			}
			catch( Exception ex ) {
                //oldLogger.add(ex.ToString());
                _log.Error(ex.ToString());
                Trace.WriteLine(ex);
                Helper.showErrors(new List<string> { AppErrorCodes.BLENDER_PATH_NOT_SET });
                settingsForm.ShowDialog();
                stopRender(false);
                return;
			}
            
			StringBuilder jsonInfo    = new StringBuilder();
			bool          jsonStarted = false;
			int           curlyStack  = 0;

            while ( !process.StandardOutput.EndOfStream ) {
				string line = process.StandardOutput.ReadLine();
				if( line.Contains( "{" ) ) {
					jsonStarted = true;
					curlyStack++;
				}
				if( jsonStarted ) {
					if( !line.ToLower().Contains( "blender quit" ) && curlyStack > 0 ) {
						jsonInfo.AppendLine( line );
					}
					if( line.Contains( "}" ) ) {
						curlyStack--;
						if( curlyStack == 0 ) {
							jsonStarted = false;
						}
					}
				}
			}

			blendData = null;
			if( jsonInfo.Length > 0 ) { 
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				blendData = serializer.Deserialize<BlendData>(jsonInfo.ToString());
            }

            if (blendData != null) {

                //notify we are going to render an image
                if (RenderFormats.IMAGES.Contains(p.renderFormat))
                {
                    Helper.showErrors(new List<string> { AppErrorCodes.RENDER_FORMAT_IS_IMAGE }, MessageBoxIcon.Asterisk, p.renderFormat);
                }

                //FIX RELATIVE RENDER OUTPUT PATHS
                try
                {
                    p.outputPath = Path.GetFullPath(blendData.outputPath);
                    p.outputPath = Path.GetDirectoryName(p.outputPath);

                    // use blendFile location if p.outputpath is null, display a warning about it
                    if (string.IsNullOrEmpty(p.outputPath))
                    {
                        var warn = new List<string>();
                        warn.Add(AppErrorCodes.BLEND_OUTPUT_INVALID);
                        Helper.showErrors(warn);
                        p.outputPath = Path.GetDirectoryName(p.blendFilePath);
                        _log.Info("Could not resolve output path... Using .blend file path");
                    }

                }
                catch (Exception)
                {
                    p.outputPath = Path.Combine(Path.GetDirectoryName(p.blendFilePath), blendData.outputPath.Replace("//", ""));
                }

                //SETTING PROJECT VARS
                //remove trailing slash
                p.outputPath = outputFolderTextBox.Text = Helper.fixPath(p.outputPath);
                p.renderFormat = blendData.renderFormat;
                p.chunksPath = Path.Combine(p.outputPath, appSettings.chunksSubfolder);
                p.fps = Convert.ToDouble(blendData.fps) / Convert.ToDouble(blendData.fpsBase, CultureInfo.InvariantCulture);

                if (startEndBlendRadio.Checked)
                {
                    p.start = blendData.start;
                    p.end = blendData.end;
                }
                
                //INFO TEXTS
                statusLabel.Text                = "Opened " + blendData.projectName + ".blend";
                blendFileLabel.Visible          = false;
                blendFileNameLabel.Text         = blendData.projectName;
                infoActiveScene.Text            = blendData.sceneActive;
                infoFramerate.Text              = p.fps.ToString("###.##");
                infoNoScenes.Text               = blendData.scenesNum;
                infoResolution.Text             = blendData.resolution;

                appSettings.addRecentBlend(p.blendFilePath);
                appSettings.save();

                //reset chunk range according to new timeline
                updateCurrentChunkStartEnd();
                updateChunkLength();

                updateRecentBlendsMenu();

                appState = AppStates.READY_FOR_RENDER;
            }
            updateUI();
            Trace.WriteLine( ".blend data = " + jsonInfo.ToString() );
            _log.Info(".blend loaded successfully");
		}
        
		private void reloadBlenderDataButton_Click( object sender, EventArgs e ) {
            loadBlend();
		}

        private void mixdown() {

            statusLabel.Text = "Rendering mixdown, it can take a while for larger projects...";
            statusLabel.Update();
            _log.Info("mixdown started");

            if (!File.Exists(p.blendFilePath)) {
                return;
            }

            if (!Directory.Exists(appSettings.scriptsPath))
            {
                // Error scriptsfolder not found
                string caption = "Error";
                string msg = "Scripts folder not found. Separate audio mixdown and automatic project info detection will not work, but you can still use the basic rendering functionality.";
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!Directory.Exists(p.outputPath)) {
                Directory.CreateDirectory(p.outputPath);
            }

            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(appSettings.blenderPath, AppSettings.BLENDER_EXE_NAME);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.Arguments = String.Format("-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- \"{4}\"",
                                                  p.blendFilePath,
                                                  p.start,
                                                  p.end,
                                                  Path.Combine(appSettings.scriptsPath, "mixdown_audio.py"),
                                                  p.outputPath
                                                  );
            Trace.WriteLine(process.StartInfo.Arguments);
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                Helper.showErrors(new List<string> { AppErrorCodes.FFMPEG_PATH_NOT_SET });
                settingsForm.ShowDialog();
                statusLabel.Text = "Mixdown cancelled.";
                return;
            }

            process.WaitForExit();

            string message = "Mixdown complete";
            Trace.WriteLine(message); _log.Info(message);
            statusLabel.Text = message;
            
        }

        // TOOL STRIP METHODS
        private void tipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolTipInfo.Active =
            toolTipWarn.Active =
            renderInfoLabel.Visible =
            appSettings.displayTooltips =
            tipsToolStripMenuItem.Checked;
            appSettings.save();
        }

        private void clearRecentProjectsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            appSettings.clearRecentBlend();
            updateRecentBlendsMenu();
        }

        private void isti115ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https:\\//github.com/Isti115/BlenderRenderController");
        }

        private void meTwentyFiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https:\\//github.com/MeTwentyFive/BlenderRenderController");
        }

        private void redRaptor93ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https:\\//github.com/RedRaptor93/BlenderRenderController");
        }

        private void jendabekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https:\\//github.com/jendabek/BlenderRenderController");
        }

        private void toolStripMenuItemBug_Click(object sender, EventArgs e)
        {
            Process.Start("https:\\//github.com/jendabek/BlenderRenderController/issues");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm.ShowDialog();
        }

        private void rendererComboBox_CheckedChanged(object sender, EventArgs e)
        {
            if(rendererRadioButtonBlender.Checked)
            {
                p.renderer = appSettings.renderer = AppStrings.RENDERER_BLENDER;
            }
            else if (rendererRadioButtonCycles.Checked)
            {
                p.renderer = appSettings.renderer = AppStrings.RENDERER_CYCLES;
            }
        }

        private void outputFolderOpenButton_Click(object sender, EventArgs e)
        {
            openOutputFolder();
        }
        private void openOutputFolder()
        {
            if (Directory.Exists(p.outputPath))
            {
                Process.Start(p.outputPath);
            }
            else
            {
                MessageBox.Show("Output folder does not exist.", "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }
        private void processCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            appSettings.processCount = p.processCount = processCountNumericUpDown.Value;
        }

        private void concatenatePartsButton_Click(object sender, EventArgs e)
        {
            concatenate();
        }

        private void MixdownAudio_Click(object sender, EventArgs e)
        {
            mixdown();
        }

        //total start numericUpDown change
        private void totalStartNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            p.start = totalStartNumericUpDown.Value;
            if (p.start > p.end)
            {
                p.end = p.start + p.chunkLength - 1;
            }
            p.chunkStart = p.start;
            updateCurrentChunkStartEnd();
            updateChunkLength();
            updateUI();
        }


        //total end numericUpDown change
        private void totalEndNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            p.end = totalEndNumericUpDown.Value;
            if (p.end < p.start)
            {
                p.start -= p.chunkLength - 1;
            }
            if (p.start < 0) p.start = 0;
            updateCurrentChunkStartEnd();
            updateChunkLength();
            updateUI();
        }

        //chunk length numericUpDown change
        private void chunkLengthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            p.chunkStart = p.start;
            p.chunkLength = appSettings.chunkLength = chunkLengthNumericUpDown.Value;
            updateCurrentChunkStartEnd();
            updateChunkLength();
            updateUI();
            appSettings.save();
        }

        private void updateCurrentChunkStartEnd()
        {
            p.chunkEnd = p.chunkStart + p.chunkLength - 1;
            if (p.chunkStart < p.start)
            {
                p.chunkStart = p.start;
            }
            if (p.chunkEnd > p.end)
            {
                p.chunkEnd = p.end;
            }
            updateUI();
        }

        private void updateChunkLength()
        {
            if (renderOptionsAutoRadio.Checked) {
                p.chunkLength = Math.Ceiling((p.end - p.start + 1) / p.processCount);

                /*//it could fix some blender rendering issues with small chunks & high fps
                if (p.chunkLength < (decimal) p.fps)
                {
                    p.chunkLength = Math.Ceiling((decimal) p.fps);
                }*/
            }
            if (p.chunkLength - 1 > p.end - p.start || p.chunkEnd > p.end) {
                p.chunkLength = p.end - p.start + 1;
            }
            updateUI();
        }

        private void startEndCustomRadio_CheckedChanged(object sender, EventArgs e)
        {
            if(startEndCustomRadio.Checked) {
                totalStartNumericUpDown.Enabled = totalEndNumericUpDown.Enabled = true;
            }
            else
            {
                totalStartNumericUpDown.Enabled = totalEndNumericUpDown.Enabled = false;
                p.start = totalStartNumericUpDown.Value = blendData.start;
                p.end = totalEndNumericUpDown.Value = blendData.end;
                updateCurrentChunkStartEnd();
                updateChunkLength();
                updateUI();
            }
        }

        private void renderOptionsRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (renderOptionsCustomRadio.Checked)
            {
                chunkLengthNumericUpDown.Enabled = processCountNumericUpDown.Enabled = true;
            }
            else
            {
                chunkLengthNumericUpDown.Enabled = chunkLengthNumericUpDown.Enabled = false;
                processCountNumericUpDown.Value = Environment.ProcessorCount;
                updateCurrentChunkStartEnd();
                updateChunkLength();
                updateUI();
            }
        }

        private void afterRenderActionRadio_CheckedChanged(object sender, EventArgs e)
        {
            var radios = new List<RadioButton>()
            {
                afterRenderJoinMixdownRadio,
                afterRenderJoinRadio,
                afterRenderDoNothingRadio
            };
            foreach (var radio in radios)
            {
                if (radio.Checked)
                {
                    p.afterRenderAction = appSettings.afterRenderAction = radio.Name.Replace("Radio", "");
                    break;
                }
            }
            updateUI();
        }

        private void donateButton_Click(object sender, EventArgs e)
        {
            string url = "";

            string business = "jendabek@gmail.com";  // your paypal email
            string description = "Donation for Blender Render Controller";            // '%20' represents a space. remember HTML!
            string country = "CZE";                  // AU, US, etc.
            string currency = "USD";                 // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            Process.Start(url);
            Console.WriteLine(url);
        }

        private void infoMore_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    exeption_test();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Test Exeption thrown...");
            //    _log.Error(ex.ToString());
            //}
        }

        private void exeption_test()
        {
            throw new Exception("this is a test Exeption");
        }
        
    }
}
