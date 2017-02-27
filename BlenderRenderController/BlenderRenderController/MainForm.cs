using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Globalization;

namespace BlenderRenderController
{
    public partial class MainForm : Form
    {
        
        bool lastChunkStarted = false;

        string appState = AppStates.AFTER_START;

        //processes
        List<Process> processes = new List<Process>();
        List<int> framesRendered = new List<int>();
        int processesCompletedCount = 0;

        BlendData blendData;
        ProjectData p;
        DateTime startTime;
        Timer processTimer;
        SettingsForm settingsForm;
        AppSettings appSettings;
        ContextMenuStrip recentBlendsMenu;
        private double fps;

        //string[] args = Environment.GetCommandLineArgs();

        public MainForm()
        {
            InitializeComponent();
        }
        public void MainForm_Shown(object sender, EventArgs e)
        {
            
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
            
            processTimer = new Timer();
            processTimer.Interval = appSettings.processCheckInterval;
            processTimer.Tick += new EventHandler(updateProcessManagement);

            recentBlendsMenu = new ContextMenuStrip();
            blendFileBrowseButton.Menu = recentBlendsMenu;
            
            applySettings();
            if (!appSettings.appConfigured)
            {
                //appState = AppStates.NOT_CONFIGURED;
                settingsForm.ShowDialog();
            }
            updateRecentBlendsMenu();
            updateUI();
        }

        private void onSettingsFormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void applySettings()
        {
            processCountNumericUpDown.Value = p.processCount = appSettings.processCount;
            chunkLengthNumericUpDown.Value = p.chunkLength = appSettings.chunkLength;

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
            chunkEndNumericUpDown.Text = p.chunkEnd.ToString();
            chunkStartNumericUpDown.Text = p.chunkStart.ToString();
            chunkLengthNumericUpDown.Value = p.chunkLength;
            totalStartNumericUpDown.Value = p.start;
            totalEndNumericUpDown.Value = p.end;
            
            //top infos
            if (blendData != null)
            {
                var durationSeconds = (Convert.ToDouble(p.end - p.start + 1) / fps);
                TimeSpan t = TimeSpan.FromSeconds(durationSeconds);

                infoDuration.Text = string.Format("{0:D1}h {1:D1}m {2:D1}s {3:D1}ms",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);
                infoFramesTotal.Text = (p.end - p.start + 1).ToString();
            }

            renderAllButton.Text = (appState == AppStates.RENDERING_CHUNK_ONLY || appState == AppStates.RENDERING_ALL) ? "Stop" : "Render";
            //enabling / disabling UI according to current app state
            switch (appState)
            {
                case AppStates.AFTER_START:
                    renderAllButton.Enabled = false;
                    menuStrip.Enabled = true;
                    renderChunkButton.Enabled = false;
                    prevChunkButton.Enabled = false;
                    nextChunkButton.Enabled = false;
                    blendFileBrowseButton.Enabled = true;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    openOutputFolderButton.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    renderAllButton.Image = Properties.Resources.render_icon_small;
                    startEndBlendRadio.Enabled = false;
                    startEndCustomRadio.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    break;
                case AppStates.NOT_CONFIGURED:
                    renderAllButton.Enabled = false;
                    menuStrip.Enabled = true;
                    renderChunkButton.Enabled = false;
                    prevChunkButton.Enabled = false;
                    nextChunkButton.Enabled = false;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    blendFileBrowseButton.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    openOutputFolderButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    renderAllButton.Image = Properties.Resources.render_icon_small;
                    startEndBlendRadio.Enabled = false;
                    startEndCustomRadio.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    break;
                case AppStates.READY_FOR_RENDER:
                    renderAllButton.Enabled = true;
                    menuStrip.Enabled = true;
                    renderChunkButton.Enabled = true;
                    prevChunkButton.Enabled = true;
                    nextChunkButton.Enabled = true;
                    mixDownButton.Enabled = true;
                    totalStartNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    totalEndNumericUpDown.Enabled = startEndCustomRadio.Checked;
                    chunkLengthNumericUpDown.Enabled = true;
                    concatenatePartsButton.Enabled = Directory.Exists(p.chunksPath);
                    reloadBlenderDataButton.Enabled = true;
                    blendFileBrowseButton.Enabled = true;
                    outputFolderBrowseButton.Enabled = true;
                    outputFolderTextBox.Enabled = true;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    totalTimeLabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    renderAllButton.Image = Properties.Resources.render_icon_small;
                    startEndBlendRadio.Enabled = true;
                    startEndCustomRadio.Enabled = true;
                    processCountNumericUpDown.Enabled = true;
                    break;
                case AppStates.RENDERING_ALL:
                case AppStates.RENDERING_CHUNK_ONLY:
                    renderAllButton.Enabled = true;
                    menuStrip.Enabled = false;
                    renderChunkButton.Enabled = false;
                    prevChunkButton.Enabled = false;
                    nextChunkButton.Enabled = false;
                    mixDownButton.Enabled = false;
                    totalStartNumericUpDown.Enabled = false;
                    totalEndNumericUpDown.Enabled = false;
                    chunkLengthNumericUpDown.Enabled = false;
                    concatenatePartsButton.Enabled = false;
                    reloadBlenderDataButton.Enabled = false;
                    blendFileBrowseButton.Enabled = false;
                    outputFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = false;
                    rendererRadioButtonCycles.Enabled = false;
                    renderAllButton.Image = Properties.Resources.stop_icon_small;
                    startEndBlendRadio.Enabled = false;
                    startEndCustomRadio.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    break;
            }
        }

        // Deletes json on form close
        private void MainForm_Close(object sender, FormClosedEventArgs e)
        {
            //jsonDel();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Arguments
            /*if (args.Length > 1)
            {
                //test arguments
                //for (int i = 0; i < args.Length; i++)
                //{
                //    string teste = string.Format("Arg[{0}] = [{1}] \r\n", i, args[i]);
                //    MessageBox.Show(teste);
                //}

                // arg 1 = .blend path
                blendFilePath = args[1];
                //blendFilePathTextBox.Text = blendFilePath;
                loadBlend();
            }*/

            
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
                Logger.add(ex.ToString());
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
                renderAllButton.Text = "Render";
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
                        Logger.add(ex.ToString());
                        MessageBox.Show("It can't be deleted, files are in use by some program.\n");
                        return;
                    }
                    renderAllButton_Click(null, null);
                }
                renderAll();
                renderAllButton.Text = "Stop";
            }
        }
        
        private void renderAll()
        {
            appState = AppStates.RENDERING_ALL;
            startTime = DateTime.Now;
            totalTimeLabel.Text = "00:00:00";
            statusLabel.Text = "Starting render...";
            processesCompletedCount = 0;
            framesRendered.Clear();
            processTimer.Enabled = true;
            updateUI();
        }

        private decimal getChunksTotalCount()
        {
            return Math.Ceiling((p.end - p.start + 1) / p.chunkLength);
        }

        private void stopRender(bool wasComplete)
        {
            statusLabel.Text = wasComplete ? "Render complete, press Join Chunks to get the final video.\nIf your anim has a sound, press Audio Mixdown before Join Chunks." : "Render cancelled.";
            
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
                    Logger.add(ex.ToString());
                    Trace.WriteLine(ex);
                }
                processes.Remove(process);
            }

            processTimer.Enabled = false;
            lastChunkStarted = false;
            renderProgressBar.Value = 0;

            appState = AppStates.READY_FOR_RENDER;

            p.chunkStart = p.start;
            checkCurrentChunkStartEnd();

            updateUI();
        }

        private void updateProcessManagement(object sender, EventArgs e)
        {
            //PROGRESS display
            if (appState == AppStates.RENDERING_ALL)
            {
                renderProgressBar.Value = (int)Math.Floor((framesRendered.Count / (p.end - p.start + 1)) * 100);

                var statusText = "";
                statusText = "Completed " + processesCompletedCount.ToString() + " / " + getChunksTotalCount().ToString();
                statusText += " chunks, rendered " + framesRendered.Count + " frames in " + processes.Count;
                statusText += (processes.Count > 1) ? " processes." : " process.";
                statusLabel.Text = statusText;
            }
            else
            {
                renderProgressBar.Value = (int)Math.Floor((framesRendered.Count / (p.chunkEnd - p.chunkStart + 1)) * 100);
                
                if (framesRendered.Count > 0)
                {
                    statusLabel.Text = "Rendering chunk frame " + framesRendered.ElementAt(framesRendered.Count - 1) + ".";
                }
            }

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
			totalTimeLabel.Text = String.Format( "{0,2:D2}:{1,2:D2}:{2,2:D2}", (int)runTime.TotalHours, runTime.Minutes, runTime.Seconds );

            if (processes.Count == 0)
            {

                bool wasComplete = (framesRendered.Count == p.end - p.start + 1);
                stopRender(wasComplete);
                checkCurrentChunkStartEnd();
                return;
            }
            updateUI();
        }
        
        private void concatenatePartsButton_Click(object sender, EventArgs e)
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
                statusLabel.Text = "Joining chunks...";
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
                Logger.add(ex.ToString());
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
                Logger.add(ex.ToString());
                Trace.WriteLine(ex);
                Helper.showErrors(new List<string> { AppErrorCodes.BLENDER_PATH_NOT_SET });
                settingsForm.ShowDialog();
                stopRender(false);
                return;
			}
            
			StringBuilder jsonInfo    = new StringBuilder();
			bool          jsonStarted = false;
            double test = 1.8;
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

			if( blendData != null ) {

                p.start       = blendData.start;
                p.end         = blendData.end;
                
                fps = Convert.ToDouble(blendData.fps) / Convert.ToDouble(blendData.fpsBase, CultureInfo.InvariantCulture);
                
                //reset chunk range according to new timeline
                checkCurrentChunkStartEnd();
                checkChunkLength();
                
                statusLabel.Text = "Successfully opened " + blendData.projectName + ".blend.";
                blendFileLabel.Visible = false;
                blendFileNameLabel.Text            = blendData.projectName;
                infoActiveScene.Text               = blendData.sceneActive;
                infoFramerate.Text                 = fps.ToString();
                infoNoScenes.Text                  = blendData.scenesNum;

                try
                {
                    p.outputPath = Path.GetFullPath(blendData.outputPath);
                    p.outputPath = Path.GetDirectoryName(p.outputPath);
                }
                catch (Exception)
                {
                    p.outputPath = Path.Combine(Path.GetDirectoryName(p.blendFilePath), blendData.outputPath.Replace("//", ""));
                }
                //remove trailing slash
                p.outputPath = Helper.fixPath(p.outputPath);
                outputFolderTextBox.Text = p.outputPath;

                p.chunksPath = Path.Combine(p.outputPath, appSettings.chunksSubfolder);
                appSettings.addRecentBlend(p.blendFilePath);
                appSettings.save();
                updateRecentBlendsMenu();

                appState = AppStates.READY_FOR_RENDER;

                updateRecentBlendsMenu();
            }
            updateUI();
            Trace.WriteLine( ".blend data = " + jsonInfo.ToString() );
		}
        
		private void reloadBlenderDataButton_Click( object sender, EventArgs e ) {
            loadBlend();
		}

        private void MixdownAudio_Click(object sender, EventArgs e) {

            statusLabel.Text = "Rendering mixdown...";
            statusLabel.Update();

            if (!File.Exists(p.blendFilePath)) {
                return;
            }

            if (!Directory.Exists(appSettings.scriptsPath))
            {
                // Error scriptsfolder not found
                string caption = "Error";
                string message = "Scripts folder not found. Separate audio mixdown and automatic project info detection will not work, but you can still use the basic rendering functionality.";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                                                  p.chunkStart,
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

            process.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds);

            Trace.WriteLine("Mixdown completed");
            statusLabel.Text = "Mixdown complete.";
        }

        // show / hide tooltips
        private void tipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolTipInfo.Active = toolTipWarn.Active = tipsToolStripMenuItem.Checked;
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
            if (Directory.Exists(p.outputPath)) {
                Process.Start(p.outputPath);
            } else
            {
                MessageBox.Show("Folder does not exist.", "",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }
        private void processCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            appSettings.processCount = p.processCount = processCountNumericUpDown.Value;
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
            checkCurrentChunkStartEnd();
            checkChunkLength();
            updateUI();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm.ShowDialog();
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
            checkCurrentChunkStartEnd();
            checkChunkLength();
            updateUI();
        }

        //chunk length numericUpDown change
        private void chunkLengthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            p.chunkStart = p.start;
            p.chunkLength = appSettings.chunkLength = chunkLengthNumericUpDown.Value;
            checkCurrentChunkStartEnd();
            checkChunkLength();
            updateUI();
            appSettings.save();
        }

        private void checkCurrentChunkStartEnd()
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

        private void checkChunkLength()
        {
            if(p.chunkLength - 1 > p.end - p.start || p.chunkEnd > p.end) {
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
                checkCurrentChunkStartEnd();
                checkChunkLength();
                updateUI();
            }
        }
    }
}
