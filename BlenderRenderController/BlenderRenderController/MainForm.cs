using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace BlenderRenderController
{

    
    public partial class MainForm : Form
    {
        string blendFilePath, outputPath;

        decimal chunkStart, chunkEnd, chunkLength, start, end;
        const string RENDERER_BLENDER = "BLENDER_RENDER";
        const string RENDERER_CYCLES = "CYCLES";

        bool lastChunkStarted = false;

        string appState = AppStates.AFTER_START;

        //processes
        List<Process> processes = new List<Process>();
        List<int> framesRendered = new List<int>();
        int processesCompletedCount = 0;

        BlendData blendData;
        DateTime startTime;
        Timer processTimer;

        //settingsForm
        SettingsForm settingsForm;

        //settings
        AppSettings appSettings;

        //string[] args = Environment.GetCommandLineArgs();


        public MainForm()
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(onAppExit);

            InitializeComponent();

            settingsForm = new SettingsForm();
            appSettings = new AppSettings();


            appSettings.settingsForm = settingsForm;
            appSettings.init();

            settingsForm.FormClosed += new FormClosedEventHandler(onSettingsFormClosed);
            settingsForm.init(appSettings);

            chunkLength = chunkLengthNumericUpDown.Value;
            start = totalStartNumericUpDown.Value;
            end = totalEndNumericUpDown.Value;
            statusLabel.Text = "Hello 3D world!";
            applySettings();
            if (!appSettings.appConfigured)
            {
                appState = AppStates.NOT_CONFIGURED;
                settingsForm.ShowDialog();
            }
            updateUI();
        }

        private void onSettingsFormClosed(object sender, FormClosedEventArgs e)
        {
            if(appSettings.appConfigured)
            {
                appState = AppStates.AFTER_START;
            } else
            {
                appState = AppStates.NOT_CONFIGURED;
            }
            updateUI();
        }

        private void applySettings()
        {
            processCountNumericUpDown.Value = appSettings.processCount;
        }

        private void onAppExit(object sender, EventArgs e)
        {
            stopRender(false);
            appSettings.save();
        }

        public void updateUI()
        {
            chunkEndNumericUpDown.Text = chunkEnd.ToString();
            chunkStartNumericUpDown.Text = chunkStart.ToString();
            chunkLengthNumericUpDown.Value = chunkLength;
            totalStartNumericUpDown.Value = start;
            totalEndNumericUpDown.Value = end;

            //top infos
            if (blendData != null)
            {
                var durationSeconds = Convert.ToDouble((end - start + 1) / int.Parse(blendData.fps.Split('.')[0]));
                TimeSpan t = TimeSpan.FromSeconds(durationSeconds);

                infoDuration.Text = string.Format("{0:D1}h {1:D1}m {2:D1}s {3:D1}ms",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);
                infoFramesTotal.Text = (end - start + 1).ToString();
            }

            renderAllButton.Text = (appState == AppStates.RENDERING_CHUNK_ONLY || appState == AppStates.RENDERING_ALL) ? "Stop" : "Render";

            //enabling / disabling UI according to current app state
            switch (appState)
            {
                case AppStates.AFTER_START:
                    renderAllButton.Enabled = false;
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
                    partsFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    break;
                case AppStates.NOT_CONFIGURED:
                    renderAllButton.Enabled = false;
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
                    partsFolderBrowseButton.Enabled = false;
                    openOutputFolderButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = false;
                    totalTimeLabel.Visible = false;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    break;
                case AppStates.READY:
                    renderAllButton.Enabled = true;
                    renderChunkButton.Enabled = true;
                    prevChunkButton.Enabled = true;
                    nextChunkButton.Enabled = true;
                    mixDownButton.Enabled = true;
                    totalStartNumericUpDown.Enabled = true;
                    totalEndNumericUpDown.Enabled = true;
                    chunkLengthNumericUpDown.Enabled = true;
                    concatenatePartsButton.Enabled = true;
                    reloadBlenderDataButton.Enabled = true;
                    blendFileBrowseButton.Enabled = true;
                    partsFolderBrowseButton.Enabled = true;
                    outputFolderTextBox.Enabled = true;
                    processCountNumericUpDown.Enabled = true;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    totalTimeLabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = true;
                    rendererRadioButtonCycles.Enabled = true;
                    break;
                case AppStates.RENDERING_ALL:
                case AppStates.RENDERING_CHUNK_ONLY:
                    renderAllButton.Enabled = true;
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
                    partsFolderBrowseButton.Enabled = false;
                    outputFolderTextBox.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    openOutputFolderButton.Enabled = true;
                    statusLabel.Visible = true;
                    timeElapsedLabel.Visible = true;
                    rendererRadioButtonBlender.Enabled = false;
                    rendererRadioButtonCycles.Enabled = false;
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

            blendFilePath = "";
            outputPath = "";

            processTimer = new Timer();
            processTimer.Interval = appSettings.processCheckInterval;
            processTimer.Tick += new EventHandler(updateProcessManagement);
        }

        private void blendFileBrowseButton_Click(object sender, EventArgs e)
        {
            var blendFileBrowseDialog = new OpenFileDialog();
            blendFileBrowseDialog.Filter = "Blend|*.blend";

            var result = blendFileBrowseDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                blendFilePath = blendFileBrowseDialog.FileName;
                loadBlend();
            }

        }

        private void partsFolderBrowseButton_Click(object sender, EventArgs e)
        {
            var partsFolderBrowseDialog = new FolderBrowserDialog();
            //outFileBrowseDialog.Filter = "Blend|*.blend";
            Trace.WriteLine(outputFolderTextBox.Text);
            var result = partsFolderBrowseDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                outputPath = outputFolderTextBox.Text = Path.GetFullPath(partsFolderBrowseDialog.SelectedPath);
            }
        }

        private void outFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {

            outputFolderTextBox.Text = outputFolderTextBox.Text.Trim();

            try {
                Path.GetFullPath(outputFolderTextBox.Text);
            }
            catch (Exception)
            {
                outputFolderTextBox.Text = outputPath;
                return;
            }

            outputPath = outputFolderTextBox.Text = Path.GetFullPath(outputFolderTextBox.Text);

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
            if (chunkEnd == end) lastChunkStarted = true;

            Process p = new Process();
            processes.Add(p);

            ProcessStartInfo pStartInfo = new ProcessStartInfo();
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.UseShellExecute = false;
            pStartInfo.CreateNoWindow = true;
            pStartInfo.WorkingDirectory = appSettings.blenderPath;
            pStartInfo.FileName = Path.Combine(appSettings.blenderPath, "blender.exe");

            string renderer = rendererRadioButtonBlender.Checked ? RENDERER_BLENDER : RENDERER_CYCLES;
            pStartInfo.Arguments = String.Format("-b \"{0}\" -o {1} -E {2} -s {3} -e {4} -a",
                                                  blendFilePath,
                                                  outputPath + "\\" + appSettings.chunksSubfolder + "\\" + blendData.projectName + "-#",
                                                  renderer,
                                                  chunkStart,
                                                  chunkEnd
                                                  );
            p.StartInfo = pStartInfo;
            p.ErrorDataReceived += onRenderProcessErrorDataReceived;
            p.OutputDataReceived += onRenderProcessDataReceived;
            p.EnableRaisingEvents = true;

            Trace.WriteLine(String.Format("CEW: {0}", pStartInfo.Arguments));
            
            p.Exited += new EventHandler(chunkRendered);

            try
            {
                p.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                stopRender(false);
                return;
            }

            p.PriorityClass = ProcessPriorityClass.High;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
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

            if (chunkStart - chunkLength - 1 < start)
            {
                chunkStart = start;
                chunkEnd = start + chunkLength - 1;
            }

            else
            {
                chunkEnd = chunkStart - 1;
                chunkStart = chunkEnd - chunkLength + 1;
            }
            updateUI();
        }
        private void moveToNextChunk()
        {
            //start of next chunk must not be above totalEnd, so we can move to another chunk
            if (!(chunkEnd + 1 > end))
            {
                chunkStart = chunkEnd + 1;

                if (chunkEnd + chunkLength - 1 > end)
                {
                    chunkEnd = end;
                }
                else
                {
                    chunkEnd += chunkLength;
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
                var chunksPath = Path.Combine(outputPath, appSettings.chunksSubfolder);
                if (Directory.Exists(chunksPath) && Directory.GetFiles(chunksPath).Length > 0)
                {
                    // Configure the message box to be displayed
                    DialogResult dialogResult = MessageBox.Show("All previously rendered chunks will be deleted.\nDo you want to continue?",
                                                                "Chunks folder not empty",
                                                                MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Exclamation);
                    
                    if (dialogResult == DialogResult.No) {
                        return;
                    }
                    try {
                        Helper.clearFolder(chunksPath);
                    }
                    catch (Exception){
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
            return Math.Ceiling((end - start + 1) / chunkLength);
        }

        private void stopRender(bool wasComplete)
        {

            //Show time run
            statusLabel.Text = wasComplete ? "Render complete, press Join Chunks to get the final video.\nIf your anim has a sound, press Audio Mixdown before Join Chunks." : "Render cancelled.";
            TimeSpan runTime = DateTime.Now - startTime;
            //totalTimeLabel.Text = String.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", (int)runTime.TotalHours, runTime.Minutes, runTime.Seconds);
            //startTime = DateTime.MaxValue;
            
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
                    Trace.WriteLine(ex);
                }
                processes.Remove(process);
            }
            chunkStart = start;
            processTimer.Enabled = false;
            lastChunkStarted = false;
            renderProgressBar.Value = 0;


            appState = AppStates.READY;
            checkCurrentChunkStartEnd();
            updateUI();
        }

        private void updateProcessManagement(object sender, EventArgs e)
        {

            //PROGRESS display
            if (appState == AppStates.RENDERING_ALL)
            {
                renderProgressBar.Value = (int)Math.Floor((framesRendered.Count / (end - start + 1)) * 100);

                var statusText = "";
                statusText = "Completed " + processesCompletedCount.ToString() + " / " + getChunksTotalCount().ToString();
                statusText += " chunks, rendered " + framesRendered.Count + " frames in " + processes.Count;
                statusText += (processes.Count > 1) ? " processes." : " process.";
                statusLabel.Text = statusText;
            } else
            {
                renderProgressBar.Value = (int)Math.Floor((framesRendered.Count / (chunkEnd - chunkStart + 1)) * 100);
                
                if (framesRendered.Count > 0)
                {
                    statusLabel.Text = "Rendering chunk frame " + framesRendered.ElementAt(framesRendered.Count - 1) + ".";
                }
            }


            //start next chunk if needed
            if (!lastChunkStarted)
            {
                if (processes.Count < processCountNumericUpDown.Value)
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

                bool wasComplete = (framesRendered.Count == end - start + 1);
                stopRender(wasComplete);
                checkCurrentChunkStartEnd();
                return;
            }
            updateUI();
        }
        
        private void concatenatePartsButton_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(outputPath))
            {
                //errorMsgs(-100);
                return;
            }
            string chunksPath = Path.Combine(outputPath, appSettings.chunksSubfolder);
            string chunksTxtPath = Path.Combine(chunksPath, appSettings.chunksTxtFileName);
            string audioFileName = blendData.projectName + "." + appSettings.audioFormat;
            string audioSettings = string.Empty; //"-c:a aac -b:a 256k";
            string videoExtensionFound = "";

            List<string> fileList = new List<string>();
            foreach (var format in appSettings.allowedFormats)
            {
                videoExtensionFound = format;
                fileList = Directory.GetFiles(chunksPath, "*." + videoExtensionFound, SearchOption.TopDirectoryOnly).ToList();
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

            var addAudioArguments = "";
            
            //mixdown audio NOT found
            if (!File.Exists(Path.Combine(outputPath, audioFileName)))
            {
                statusLabel.Text = "Joining chunks...";
                audioFileName = string.Empty;
                audioSettings = string.Empty;
            }
            //mixdown audio found
            else
            {
                statusLabel.Text = "Joining chunks with mixdown audio...";
                addAudioArguments = "-i " + Path.Combine(outputPath, audioFileName) + " -map 0:v -map 1:a";
            }
            Process p = new Process();
            p.StartInfo.WorkingDirectory = appSettings.ffmpegPath;
            p.StartInfo.FileName = Path.Combine(appSettings.ffmpegPath, "ffmpeg.exe");
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;


            p.StartInfo.Arguments = 
                String.Format("-f concat -safe 0 -i {0} {1} -c:v copy {3}.{4} -y",
                chunksTxtPath,
                addAudioArguments,
                audioSettings,
                Path.Combine(outputPath, blendData.projectName),
                videoExtensionFound
            );
            Trace.WriteLine(p.StartInfo.Arguments);
            try
            {
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
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

            if ( !File.Exists( blendFilePath) ) {
                // file does not exist
                //errorMsgs(-104);
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

            Process p = new Process();
            p.StartInfo.WorkingDirectory       = appSettings.blenderPath;
            p.StartInfo.FileName               = Path.Combine(appSettings.blenderPath, "blender.exe");
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.CreateNoWindow         = true;
			p.StartInfo.UseShellExecute        = false;

            p.StartInfo.Arguments = String.Format("-b \"{0}\" -P \"{1}\"",
                                                  blendFilePath,
                                                  Path.Combine(appSettings.scriptsPath, "get_project_info.py")
                                    );

            Trace.WriteLine(appSettings.blenderPath);
            try {
				p.Start();
			}
			catch( Exception ex ) {
                Helper.showErrors(new List<string> { AppErrorCodes.BLENDER_PATH_NOT_SET });
                settingsForm.ShowDialog();
                stopRender(false);
                return;
			}
            
			StringBuilder jsonInfo    = new StringBuilder();
			bool          jsonStarted = false;
			int           curlyStack  = 0;

            while ( !p.StandardOutput.EndOfStream ) {
				string line = p.StandardOutput.ReadLine();

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

                start       = blendData.start;
				end         = blendData.end;
                
                //reset chunk range according to new timeline
                checkCurrentChunkStartEnd();
                checkChunkLength();

                statusLabel.Text = "Successfully opened " + blendData.projectName + ".blend.";
                blendFileLabel.Text = "1. Blend File:";
                blendFileNameLabel.Text            = " " + blendData.projectName;
                infoActiveScene.Text               = blendData.sceneActive;
                infoFramerate.Text                 = blendData.fps.ToString();
                infoNoScenes.Text                  = blendData.scenesNum;

                try
                {
                    outputPath = Path.GetFullPath(blendData.outputPath);
                    outputPath = Path.GetDirectoryName(outputPath);
                }
                catch (Exception)
                {

                    outputPath = Path.Combine(Path.GetDirectoryName(blendFilePath), blendData.outputPath.Replace("//", ""));
                }
                
                outputFolderTextBox.Text = outputPath;
                

                appSettings.lastBlendsAdd(blendFilePath);
                appSettings.save();

                appState = AppStates.READY;
			}

            // Error checker
            //errorMsgs(ErrorCode);
            updateUI();

            Trace.WriteLine( ".blend data = " + jsonInfo.ToString() );

		}
        
		private void reloadBlenderDataButton_Click( object sender, EventArgs e ) {
            loadBlend();
		}

        private void MixdownAudio_Click(object sender, EventArgs e) {

            statusLabel.Text = "Rendering mixdown...";
            statusLabel.Update();

            if (!File.Exists(blendFilePath)) {
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


            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            Process p = new Process();

            p.StartInfo.WorkingDirectory = appSettings.ffmpegPath;
            p.StartInfo.FileName = Path.Combine(appSettings.ffmpegPath, "ffmpeg.exe");
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow         = true;
            //Using minimized instead so we get feedback
            //p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

            p.StartInfo.Arguments = String.Format("-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- {4}",
                                                  blendFilePath,
                                                  chunkStart,
                                                  end,
                                                  Path.Combine(appSettings.scriptsPath, "mixdown_audio.py"),
                                                  outputPath
                                    );
            Trace.WriteLine(p.StartInfo.Arguments);
            try
            {
                p.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                Helper.showErrors(new List<string> { AppErrorCodes.FFMPEG_PATH_NOT_SET });
                settingsForm.ShowDialog();
                statusLabel.Text = "Mixdown cancelled.";
                return;
            }

            p.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds);

            Trace.WriteLine("Mixdown completed");
            statusLabel.Text = "Mixdown complete.";


        }

        /* About this app
        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }
        */

        private void tipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show / hide tooltips
            if (tipsToolStripMenuItem.Checked == false)
            {
                activeWarn.Active = false;
                toolTip1.Active = false;
                //toolTip.Active = false;
            }
            else if (tipsToolStripMenuItem.Checked == true)
            {
                activeWarn.Active = true;
                toolTip1.Active = true;
            }

        }
        
        private void jsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //json_info op = new json_info();
            //op.Show();
        }
        
        private void chunkEndNumericUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        private void isti115ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Isti115/BlenderRenderController");
        }

        private void meTwentyFiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/MeTwentyFive/BlenderRenderController");
        }

        private void redRaptor93ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/RedRaptor93/BlenderRenderController");
        }
        private void jendabekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/jendabek/BlenderRenderController");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void totalFrameCountLabel_Click(object sender, EventArgs e)
        {

        }

        private void rendererComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void rendererLabel_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void chunkEndNumericUpDown_Click(object sender, EventArgs e)
        {

        }

        private void chunkEndLabel_Click(object sender, EventArgs e)
        {

        }

        private void infoFramesTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void infoFramesTotalLabel_Click(object sender, EventArgs e)
        {

        }

        private void openOutputFolderButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(outputPath)) {
                Process.Start(outputPath);
            } else
            {
                MessageBox.Show("Folder does not exist.", "",
                                                                MessageBoxButtons.OK,
                                                                MessageBoxIcon.Exclamation);
            }
        }

        private void processCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            appSettings.processCount = processCountNumericUpDown.Value;
        }

        //total start numericUpDown change
        private void totalStartNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            start = totalStartNumericUpDown.Value;
            if (start > end)
            {
                end = start + chunkLength - 1;
            }
            chunkStart = start;
            checkCurrentChunkStartEnd();
            checkChunkLength();
            updateUI();
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            
            settingsForm.StartPosition = FormStartPosition.CenterParent;
            settingsForm.ShowDialog();
        }

        //total end numericUpDown change
        private void totalEndNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            end = totalEndNumericUpDown.Value;
            if (end < start)
            {
                start -= chunkLength - 1;
            }
            if (start < 0) start = 0;
            checkCurrentChunkStartEnd();
            checkChunkLength();
            updateUI();
        }

        //chunk length numericUpDown change
        private void chunkLengthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            chunkStart = start;
            chunkLength = chunkLengthNumericUpDown.Value;
            checkCurrentChunkStartEnd();
            checkChunkLength();
            updateUI();
        }

        private void checkCurrentChunkStartEnd()
        {

            chunkEnd = chunkStart + chunkLength - 1;
            if (chunkStart < start)
            {
                chunkStart = start;
            }
            if (chunkEnd > end)
            {
                chunkEnd = end;
            }
            
            updateUI();
        }
        private void checkChunkLength()
        {
            if(chunkLength - 1 > end - start || chunkEnd > end) {
                chunkLength = end - start + 1;
            }
            updateUI();
        }
    }
}
