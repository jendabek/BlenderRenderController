using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;


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
        ProjectData pData;
        DateTime startTime;
        Timer processTimer;
        SettingsForm settingsForm;
        AppSettings appSettings;
        ContextMenuStrip recentBlendsMenu;
        List<int> renderingSpeedsFPS = new List<int>();
        PlatformID Os = Environment.OSVersion.Platform;
        
        // Logger service
        private static Logger logger = LogManager.GetLogger("BRC");

        public MainForm()
        {
            InitializeComponent();

            PlatAdjust(Os);
        }
        public void MainForm_Shown(object sender, EventArgs e)
        {
            
            //add version numbers to window title
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            versionLabel.Text = " v" + assemblyVersion.Split('.')[0] + "." + assemblyVersion.Split('.')[1] + "." + assemblyVersion.Split('.')[2];

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(onAppExit);


            settingsForm = new SettingsForm();
            appSettings = new AppSettings();
            pData = new ProjectData();

            appSettings.settingsForm = settingsForm;
            appSettings.init();

            settingsForm.FormClosed += new FormClosedEventHandler(onSettingsFormClosed);
            settingsForm.init(appSettings);

            pData.chunkLength = chunkLengthNumericUpDown.Value;
            pData.start = totalStartNumericUpDown.Value;
            pData.end = totalEndNumericUpDown.Value;
            statusLabel.Text = "Hello 3D world!";

            Text = AppSettings.APP_TITLE;
            
            processTimer = new Timer();
            processTimer.Interval = appSettings.processCheckInterval;
            processTimer.Tick += new EventHandler(updateProcessManagement);

            recentBlendsMenu = new ContextMenuStrip();

            if (blendFileBrowseButton.Visible == true)
                blendFileBrowseButton.Menu = recentBlendsMenu;

            else if (blendBrowseOver.Visible == true)
                blendBrowseOver.MenuOvr = recentBlendsMenu;


            applySettings();
            if (!appSettings.appConfigured)
            {
                //appState = AppStates.NOT_CONFIGURED;/
                settingsForm.ShowDialog();
            }
            updateRecentBlendsMenu();
            updateUI();
            logger.Info("Program Started");
        }

        private void onSettingsFormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Info("Settings saved");
        }

        private void applySettings()
        {
            renderOptionsRadio_CheckedChanged(null, null);
            pData.afterRenderAction = appSettings.afterRenderAction;

            //tooltips
            toolTipInfo.Active =
            toolTipWarn.Active =
            renderInfoLabel.Visible =
            tipsToolStripMenuItem.Checked = appSettings.displayTooltips;

            switch (pData.afterRenderAction)
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
            pData.renderer = appSettings.renderer;
            if (pData.renderer == AppStrings.RENDERER_BLENDER)
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
            pData.blendFilePath = item.ToolTipText;
            loadBlend();
        }

        public void updateUI()
        {
            chunkLengthNumericUpDown.Value = pData.chunkLength;
            totalStartNumericUpDown.Value = pData.start;
            totalEndNumericUpDown.Value = pData.end;
            
            //top infos
            if (blendData != null)
            {
                var durationSeconds = (Convert.ToDouble(pData.end - pData.start + 1) / pData.fps);
                infoDuration.Text = Helper.secondsToString(durationSeconds, false);
                infoFramesTotal.Text = (pData.end - pData.start + 1).ToString();
            }

            renderAllButton.Text = (appState == AppStates.RENDERING_CHUNK_ONLY || appState == AppStates.RENDERING_ALL) 
                                    ? "Stop Render" 
                                    : "Start Render";
            
            // the world's longest switch block!
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
                    concatenatePartsButton.Enabled = Directory.Exists(pData.chunksPath);
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

        private void MainForm_Close(object sender, FormClosedEventArgs e)
        {
            logger.Info("Program Closed");
        }

        private void blendFileBrowseButton_Click(object sender, EventArgs e)
        {
            var blendFileBrowseDialog = new OpenFileDialog();
            blendFileBrowseDialog.Filter = "Blend|*.blend";

            var result = blendFileBrowseDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                pData.blendFilePath = Path.GetFullPath(blendFileBrowseDialog.FileName);
                loadBlend();
            }
        }

        private void outputFolderBrowseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = pData.outputPath;
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                pData.outputPath = outputFolderTextBox.Text = Path.GetFullPath(dialog.SelectedPath);
                pData.chunksPath = Path.Combine(pData.outputPath, appSettings.chunksSubfolder);
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
                outputFolderTextBox.Text = pData.outputPath;
                return;
            }
            pData.outputPath = outputFolderTextBox.Text;
            pData.chunksPath = Path.Combine(pData.outputPath, appSettings.chunksSubfolder);
            updateUI();
        }

        //confirm text input by enter & go to next control
        private void Enter_GotoNext(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || (e.KeyCode == Keys.Return))
            {
                SelectNextControl((Control)sender, true, true, true, true);
                e.SuppressKeyPress = true; //disables sound
            }
        }

        // nf
        //private void renderChunkButton_Click(object sender, EventArgs e)
        //{
        //    appState = AppStates.RENDERING_CHUNK_ONLY;
        //    startTime = DateTime.Now;
        //    totalTimeLabel.Text = "00:00:00";
        //    statusLabel.Text = "Starting render...";
        //    processesCompletedCount = 0;
        //    lastChunkStarted = true;
        //    processTimer.Enabled = true;
        //    framesRendered.Clear();
        //    renderCurrentChunk();
        //    updateUI();
        //}

        private void renderCurrentChunk()
        {
            if (pData.chunkEnd >= pData.end)
            {
                lastChunkStarted = true;
                pData.chunkEnd = (pData.chunkEnd > pData.end) ? pData.end : pData.chunkEnd;
            }

            Process process = new Process();

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = appSettings.blenderPath;
            process.StartInfo.FileName = Path.Combine(appSettings.blenderPath, appSettings.BlenderExeName);
            
            process.StartInfo.Arguments = String.Format("-b \"{0}\" -o \"{1}\" -E {2} -s {3} -e {4} -a",
                                                  pData.blendFilePath,
                                                  Path.Combine(pData.chunksPath, blendData.ProjectName) + "-#",
                                                  pData.renderer,
                                                  pData.chunkStart,
                                                  pData.chunkEnd
                                                  );
            process.ErrorDataReceived += (pSender, pArgs) => Console.WriteLine(pArgs.Data);
            process.OutputDataReceived += onRenderProcessDataReceived;
            process.EnableRaisingEvents = true;

            Trace.WriteLine(String.Format("CEW: {0}", process.StartInfo.Arguments));

            //process.Exited += new EventHandler(chunkRendered);
            process.Exited += (pSender, pArgs) =>
            {
                processes.Remove((Process)pSender);
                logger.Info($"Render {processes.Count}/{pData.processCount} in process");
                processesCompletedCount++;
            };
            try
            {
                process.Start(); 
                processes.Add(process);
                logger.Info($"Render {processes.Count}/{pData.processCount} in process");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                logger.Error(ex.ToString());
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

        private void moveToNextChunk()
        {
            //start of next chunk must not be above totalEnd, so we can move to another chunk
            if (!(pData.chunkEnd + 1 > pData.end))
            {
                pData.chunkStart = pData.chunkEnd + 1;

                if (pData.chunkEnd + pData.chunkLength - 1 > pData.end)
                {
                    pData.chunkEnd = pData.end;
                }
                else
                {
                    pData.chunkEnd += pData.chunkLength;
                }
                updateUI();
                
            }
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
                logger.Warn("RENDER ABORTED");
            }

            //we want to start render
            else {
                if (Directory.Exists(pData.chunksPath) && Directory.GetFiles(pData.chunksPath).Length > 0)
                {
                    // Configure the message box to be displayed
                    DialogResult dialogResult = MessageBox.Show("All previously rendered chunks will be deleted.\nDo you want to continue?",
                                                                "Chunks folder not empty",
                                                                MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Exclamation);

                    if (dialogResult == DialogResult.No) return;

                    // clean folders
                    try {
                        Helper.clearFolder(pData.chunksPath);
                    }
                    catch (IOException ex){
                        logger.Error(ex.ToString());
                        MessageBox.Show("It can't be deleted, files are in use by some program.\n");
                        return;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        MessageBox.Show("An unexpected error ocurred, sorry.");
                        return;
                    }
                }
                renderAll();
                logger.Info("RENDER STARTED");
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
            catch (Exception){}

            updateUI();
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
                    logger.Error("An error ocurred while killing processes");
                    logger.Error("Stack:\n" + ex.StackTrace);
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
            catch (Exception){}

            appState = AppStates.READY_FOR_RENDER;

            pData.chunkStart = pData.start;
            updateCurrentChunkStartEnd();

            updateUI();
        }

        private void updateProcessManagement(object sender, EventArgs e)
        {
            Func<decimal> chunksTotalCount = () => Math.Ceiling((pData.end - pData.start + 1) / pData.chunkLength);

            //PROGRESS display
            int progressPercentage = 0;
            if (appState == AppStates.RENDERING_ALL)
            {
                progressPercentage = (int)Math.Floor((framesRendered.Count / (pData.end - pData.start + 1)) * 100);

                var statusText = "";
                statusText = "Completed " + processesCompletedCount.ToString() + " / " + chunksTotalCount().ToString();
                statusText += " chunks, rendered " + framesRendered.Count + " frames in " + processes.Count;
                statusText += (processes.Count > 1) ? " processes." : " process.";
                statusLabel.Text = statusText;
            }
            else
            {
                progressPercentage = (int)Math.Floor((framesRendered.Count / (pData.chunkEnd - pData.chunkStart + 1)) * 100);
                
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
            catch (Exception){}
            //progress bar
            renderProgressBar.Value = progressPercentage;

            //title progress
            Text = progressPercentage.ToString() + "% rendered - " + AppSettings.APP_TITLE;

            //start next chunk if needed
            if (!lastChunkStarted)
            {
                if (processes.Count < pData.processCount)
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
                    int framesRemaining = (int)(pData.end - pData.start) - framesRendered.Count;
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
            bool wasComplete = (framesRendered.Count > Math.Round(Convert.ToDouble(pData.end - pData.start + 1)) * 0.75);

            if (wasComplete)
            {
                //if we rendered the project (not chunk only)
                //and some of automatic join checkboxes is checked
                //we continue with join (with mixdown if the checkbox is checked)
                if (appState == AppStates.RENDERING_ALL && (pData.afterRenderAction == AppStrings.AFTER_RENDER_JOIN_MIXDOWN || pData.afterRenderAction == AppStrings.AFTER_RENDER_JOIN))
                {
                    if (pData.afterRenderAction == AppStrings.AFTER_RENDER_JOIN_MIXDOWN)
                        mixdown();

                    concatenate();
                    stopRender(true);

                    DialogResult openOutputFolderQuestion = MessageBox.Show("Open the folder with the video?",
                           "Open folder",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question);
                    if (openOutputFolderQuestion == DialogResult.Yes)
                        openOutputFolder();
                }
                else
                    stopRender(true);
            }
            else
                stopRender(false);

            updateCurrentChunkStartEnd();
            updateUI();
        }
        
        private void concatenate()
        {
            if (!Directory.Exists(pData.chunksPath))
            {
                concatenatePartsButton.Enabled = false;
                return;
            }

            string chunksTxtPath = Path.Combine(pData.chunksPath, appSettings.chunksTxtFileName);
            string audioFileName = blendData.ProjectName + "." + appSettings.audioFormat;
            string audioSettings = string.Empty; //"-c:a aac -b:a 256k";
            string videoExtensionFound = "";

            List<string> fileList = new List<string>();
            foreach (var format in appSettings.allowedFormats)
            {
                videoExtensionFound = format;
                fileList = Directory.GetFiles(pData.chunksPath, "*." + videoExtensionFound, SearchOption.TopDirectoryOnly).ToList();
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
            using (StreamWriter partListWriter = new StreamWriter(chunksTxtPath))
            {
                foreach (var filePath in fileListSorted)
                {
                    partListWriter.WriteLine("file '{0}'", filePath);
                }
            }

            var audioArguments = "";
            
            //mixdown audio NOT found
            if (!File.Exists(Path.Combine(pData.outputPath, audioFileName)))
            {
                statusLabel.Text = "Joining chunks, please wait...";
                audioFileName = string.Empty;
                audioSettings = string.Empty;
            }
            //mixdown audio found
            else
            {
                statusLabel.Text = "Joining chunks with mixdown audio...";
                audioArguments = "-i \"" + Path.Combine(pData.outputPath, audioFileName) + "\" -map 0:v -map 1:a";
            }
            Process process = new Process();
            process.StartInfo.WorkingDirectory = appSettings.ffmpegPath;
            process.StartInfo.FileName = Path.Combine(appSettings.ffmpegPath, appSettings.FFmpegExeName);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = 
                String.Format("-f concat -safe 0 -i \"{0}\" {1} -c:v copy \"{3}.{4}\" -y",
                chunksTxtPath,
                audioArguments,
                audioSettings,
                Path.Combine(pData.outputPath, blendData.ProjectName),
                videoExtensionFound
            );

            Trace.WriteLine(process.StartInfo.Arguments);

            try
            {
                process.Start();        logger.Info(statusLabel.Text);
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                logger.Error(ex.ToString());
                Helper.showErrors(AppErrorCodes.FFMPEG_PATH_NOT_SET);
                settingsForm.ShowDialog();
                statusLabel.Text = "Joining cancelled.";
                return;
            }
            var msg = "Chunks Joined.";
            statusLabel.Text = msg; logger.Info(msg);
        }

		private void loadBlend() {
            logger.Info("Loading .blend");

            statusLabel.Text = "Reading the .blend file...";
            statusLabel.Update();

            if ( !File.Exists(pData.blendFilePath) ) {
                Helper.showErrors(AppErrorCodes.BLEND_FILE_NOT_EXISTS, MessageBoxIcon.Exclamation);

                appSettings.recentBlends.Remove(pData.blendFilePath);
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
            process.StartInfo.FileName               = Path.Combine(appSettings.blenderPath, appSettings.BlenderExeName);
			process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError  = true;
            process.StartInfo.CreateNoWindow         = true;
			process.StartInfo.UseShellExecute        = false;
            process.StartInfo.StandardOutputEncoding = 
            process.StartInfo.StandardErrorEncoding  = Encoding.UTF8;

            process.StartInfo.Arguments = String.Format("-b \"{0}\" -P \"{1}\"",
                                                  pData.blendFilePath,
                                                  Path.Combine(appSettings.scriptsPath, "get_project_info.py"));

            Trace.WriteLine(process.StartInfo.Arguments);

            try {
			    process.Start();
                //process.WaitForExit();
			}
			catch( Exception ex ) {
                logger.Error(ex.ToString());
                Trace.WriteLine(ex);
                Helper.showErrors(AppErrorCodes.BLENDER_PATH_NOT_SET );
                settingsForm.ShowDialog();
                stopRender(false);
                return;
			}

            // Get values from streams
            var streamOutput = new List<string>();
            var streamErrors = new List<string>();

            while (!process.StandardOutput.EndOfStream)
                streamOutput.Add(process.StandardOutput.ReadLine());

            while (!process.StandardError.EndOfStream)
                streamErrors.Add(process.StandardError.ReadLine());

            // log errors
            if (streamErrors.Count > 0)
                    logger.Error(streamErrors);


            if (streamOutput.Count == 0)
            {
                var e = new ui.ErrorBox("Could not open project, no information was received",
                                         streamErrors);
                e.Text = "Error";
                e.ShowDialog(this);
                stopRender(false);
                return;
            }

            StringBuilder jsonInfo    = new StringBuilder();
			bool          jsonStarted = false;
			int           curlyStack  = 0;

            foreach (var line in streamOutput)
            {
                if (line.Contains("{"))
                {
                    jsonStarted = true;
                    curlyStack++;
                }
                if (jsonStarted)
                {
                    if (!line.ToLower().Contains("blender quit") && curlyStack > 0)
                    {
                        jsonInfo.AppendLine(line);
                    }
                    if (line.Contains("}"))
                    {
                        curlyStack--;
                        if (curlyStack == 0)
                        {
                            jsonStarted = false;
                        }
                    }
                }
            }

            blendData = null;
			if( jsonInfo.Length > 0 ) {

                // for when praser fails
                if (jsonInfo.ToString() == "{}")
                {
                    var e = new ui.ErrorBox("Could not open project, failed to parse project info", streamErrors);
                    e.Text = "Error";
                    e.ShowDialog(this);
                    stopRender(false);
                    return;
                }

				JavaScriptSerializer serializer = new JavaScriptSerializer();
				blendData = serializer.Deserialize<BlendData>(jsonInfo.ToString());
            }

            if (blendData != null) {

                //notify we are going to render an image
                if (RenderFormats.IMAGES.Contains(pData.renderFormat))
                {
                    Helper.showErrors(AppErrorCodes.RENDER_FORMAT_IS_IMAGE, MessageBoxIcon.Asterisk, pData.renderFormat);
                }

                //FIX RELATIVE RENDER OUTPUT PATHS
                try
                {
                    pData.outputPath = Path.GetFullPath(blendData.OutputPath);
                    pData.outputPath = Path.GetDirectoryName(pData.outputPath);
                }
                catch (Exception ex)
                {
                    pData.outputPath = Path.Combine(Path.GetDirectoryName(pData.blendFilePath), blendData.OutputPath.Replace("//", ""));
                    logger.Error(ex.Message);
                }

                // use blendFile location if p.outputpath is null, display a warning about it
                var fixedPath = Helper.fixPath(pData.outputPath);
                if (string.IsNullOrEmpty(fixedPath))
                {
                    Helper.showErrors(AppErrorCodes.BLEND_OUTPUT_INVALID);
                    pData.outputPath = Path.GetDirectoryName(pData.blendFilePath);
                }

                //SETTING PROJECT VARS
                //remove trailing slash
                pData.outputPath = outputFolderTextBox.Text = Helper.fixPath(pData.outputPath);
                pData.renderFormat = blendData.RenderFormat;
                pData.chunksPath = Path.Combine(pData.outputPath, appSettings.chunksSubfolder);

				double fpsBaseD;
				double fpsD;

                // Fixes FpsBase separator been "," insted of "." on Linux
				if (blendData.FpsBase.Contains(","))
                {
                    var tmp = System.Text.RegularExpressions.Regex.Replace(blendData.FpsBase, ",", ".");
                    fpsBaseD = Convert.ToDouble(tmp, CultureInfo.InvariantCulture);
				}
				else 
				    fpsBaseD = Convert.ToDouble(blendData.FpsBase, CultureInfo.InvariantCulture);
				

                fpsD = Convert.ToDouble (blendData.Fps);

				pData.fps = fpsD / fpsBaseD;

                if (startEndBlendRadio.Checked)
                {
                    pData.start = blendData.Start;
                    pData.end = blendData.End;
                }
                
                //INFO TEXTS
                statusLabel.Text                = "Opened " + blendData.ProjectName + ".blend";
                blendFileLabel.Visible          = false;
                blendFileNameLabel.Text         = blendData.ProjectName;
                infoActiveScene.Text            = blendData.SceneActive;
                infoFramerate.Text              = pData.fps.ToString("###.##");
                infoNoScenes.Text               = blendData.ScenesNum;
                infoResolution.Text             = blendData.Resolution;

                appSettings.addRecentBlend(pData.blendFilePath);
                appSettings.save();

                //reset chunk range according to new timeline
                updateCurrentChunkStartEnd();
                updateChunkLength();

                updateRecentBlendsMenu();

                appState = AppStates.READY_FOR_RENDER;
                logger.Info(".blend loaded successfully");
            }
            else
                logger.Error(".blend was NOT loaded");

            Trace.WriteLine( ".blend data = " + jsonInfo.ToString());
            logger.Info(".blend data = " + jsonInfo.ToString());
            updateUI();
        }

        private void reloadBlenderDataButton_Click( object sender, EventArgs e ) {
            loadBlend();
		}

        private void mixdown() {

            statusLabel.Text = "Rendering mixdown, it can take a while for larger projects...";
            statusLabel.Update();
            logger.Info("Mixdown started");

            if (!File.Exists(pData.blendFilePath)) {
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

            if (!Directory.Exists(pData.outputPath)) {
                Directory.CreateDirectory(pData.outputPath);
            }

            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(appSettings.blenderPath, appSettings.BlenderExeName);
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.Arguments = String.Format("-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- \"{4}\"",
                                                  pData.blendFilePath,
                                                  pData.start,
                                                  pData.end,
                                                  Path.Combine(appSettings.scriptsPath, "mixdown_audio.py"),
                                                  pData.outputPath
                                                  );
            Trace.WriteLine(process.StartInfo.Arguments);
            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                Helper.showErrors(AppErrorCodes.FFMPEG_PATH_NOT_SET);
                settingsForm.ShowDialog();
                statusLabel.Text = "Mixdown cancelled.";
                return;
            }

            process.WaitForExit();

            string message = "Mixdown complete";
            Trace.WriteLine(message); logger.Info(message);
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

        #region Credits Links

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

        #endregion

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm.ShowDialog();
        }

        private void rendererComboBox_CheckedChanged(object sender, EventArgs e)
        {
            if(rendererRadioButtonBlender.Checked)
            {
                pData.renderer = appSettings.renderer = AppStrings.RENDERER_BLENDER;
            }
            else if (rendererRadioButtonCycles.Checked)
            {
                pData.renderer = appSettings.renderer = AppStrings.RENDERER_CYCLES;
            }
        }

        private void outputFolderOpenButton_Click(object sender, EventArgs e)
        {
            openOutputFolder();
        }
        private void openOutputFolder()
        {
            if (Directory.Exists(pData.outputPath))
            {
                Process.Start(pData.outputPath);
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
            appSettings.processCount = pData.processCount = processCountNumericUpDown.Value;
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
            pData.start = totalStartNumericUpDown.Value;
            if (pData.start > pData.end)
            {
                pData.end = pData.start + pData.chunkLength - 1;
            }
            pData.chunkStart = pData.start;
            updateCurrentChunkStartEnd();
            updateChunkLength();
            updateUI();
        }

        //total end numericUpDown change
        private void totalEndNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            pData.end = totalEndNumericUpDown.Value;
            if (pData.end < pData.start)
            {
                pData.start -= pData.chunkLength - 1;
            }
            if (pData.start < 0) pData.start = 0;
            updateCurrentChunkStartEnd();
            updateChunkLength();
            updateUI();
        }

        //chunk length numericUpDown change
        private void chunkLengthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            pData.chunkStart = pData.start;
            pData.chunkLength = appSettings.chunkLength = chunkLengthNumericUpDown.Value;
            updateCurrentChunkStartEnd();
            updateChunkLength();
            updateUI();
            appSettings.save();
        }

        private void updateCurrentChunkStartEnd()
        {
            pData.chunkEnd = pData.chunkStart + pData.chunkLength - 1;
            if (pData.chunkStart < pData.start)
            {
                pData.chunkStart = pData.start;
            }
            if (pData.chunkEnd > pData.end)
            {
                pData.chunkEnd = pData.end;
            }
            updateUI();
        }

        private void updateChunkLength()
        {
            if (renderOptionsAutoRadio.Checked) {
                pData.chunkLength = Math.Ceiling((pData.end - pData.start + 1) / pData.processCount);

                /*//it could fix some blender rendering issues with small chunks & high fps
                if (p.chunkLength < (decimal) p.fps)
                {
                    p.chunkLength = Math.Ceiling((decimal) p.fps);
                }*/
            }
            if (pData.chunkLength - 1 > pData.end - pData.start || pData.chunkEnd > pData.end) {
                pData.chunkLength = pData.end - pData.start + 1;
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
                pData.start = totalStartNumericUpDown.Value = blendData.Start;
                pData.end = totalEndNumericUpDown.Value = blendData.End;
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
                processCountNumericUpDown.Value = pData.processCount = Environment.ProcessorCount;
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
                    pData.afterRenderAction = appSettings.afterRenderAction = radio.Name.Replace("Radio", "");
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

        // debug stuff bellow
        private void infoMore_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    exeption_test();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Test Exeption thrown...");
            //    LogService.Log.Error(ex.ToString());
            //}
        }

        private void exeption_test()
        {
            throw new Exception("this is a test Exeption");
        }

        private void showErrorBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var er = new ui.ErrorBox();
            er.ShowDialog();
        }

    }
}
