using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Web.Script.Serialization;

namespace BlenderRenderController
{

    
    public partial class MainForm : Form
    {
        string blendFilePath;
        string outFolderPath;

        decimal chunkStart, chunkEnd, chunkLength, totalStart, totalEnd;

        bool lastChunkStarted = false;

        String[] allowedFormats = { "avi", "mp4", "mov", "mkv", "mpg", "flv" };

        string audioFormat = "ac3";
        string chunksSubfolder = "chunks";

        string appState = AppStates.AFTER_START;

        List<Process> processes = new List<Process>();

        BlenderData blendData;

        DateTime startTime;

		string scriptsPath;

        Timer processTimer;
        int ErrorCode;

        public class BlenderData {
			public int    StartFrame;
			public int    EndFrame;
            public string    Framerate;
            public string OutputDirectory;
			public string ProjectName;
            // new
            public string NumScenes;
            public string ActiveScene;

            public string AltDir;
            public int ErrorCode;
            //public int SegFrame = 1000;
        }


        string[] args = Environment.GetCommandLineArgs();


        public MainForm()
        {
            InitializeComponent();
            

			//string execPath = Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().CodeBase );
			string execPath = new Uri( System.Reflection.Assembly.GetExecutingAssembly().CodeBase ).LocalPath;

			scriptsPath = Path.Combine( Path.GetDirectoryName( execPath ), "Scripts" );
			Trace.WriteLine( String.Format( "Scripts Path: '{0}'", scriptsPath ) );
            chunkLength = chunkLengthNumericUpDown.Value;
            totalStart = totalStartNumericUpDown.Value;
            totalEnd = totalEndNumericUpDown.Value;
            updateUI();
        }
        public void updateUI()
        {
            chunkEndNumericUpDown.Text = chunkEnd.ToString();
            chunkStartNumericUpDown.Text = chunkStart.ToString();
            chunkLengthNumericUpDown.Value = chunkLength;
            totalStartNumericUpDown.Value = totalStart;
            totalEndNumericUpDown.Value = totalEnd;

            //top infos
            if (blendData != null)
            {
                var durationSeconds = Convert.ToDouble((totalEnd - totalStart) / int.Parse(blendData.Framerate.Split('.')[0]));
                TimeSpan t = TimeSpan.FromSeconds(durationSeconds);

                infoDuration.Text = string.Format("{0:D1}h {1:D1}m {2:D1}s {3:D1}ms",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);
                infoFramesTotal.Text = (totalEnd - totalStart).ToString();
            }

            renderAllButton.Text = (processTimer != null && processTimer.Enabled) ? "Stop" : "Render All";

            //enabling / disabling UI according to current app state
            switch (appState)
            {
                case AppStates.AFTER_START:
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
                    openOutputFolderButton.Enabled = false;
                    partsFolderBrowseButton.Enabled = false;
                    partsFolderPathTextBox.Enabled = false;
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
                    partsFolderPathTextBox.Enabled = true;
                    //removeFileFromPathCheckbox.Enabled = true;
                    processCountNumericUpDown.Enabled = true;
                    rendererComboBox.Enabled = true;
                    openOutputFolderButton.Enabled = true;
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
                    partsFolderPathTextBox.Enabled = false;
                    //removeFileFromPathCheckbox.Enabled = false;
                    processCountNumericUpDown.Enabled = false;
                    rendererComboBox.Enabled = false;
                    openOutputFolderButton.Enabled = true;
                    break;
            }
        }

        // Deletes json on form close
        private void MainForm_Close(object sender, FormClosedEventArgs e)
        {
            jsonDel();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Arguments
            if (args.Length > 1)
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
            }

            blendFilePath = "";
            outFolderPath = "";

            processTimer = new Timer();
            processTimer.Interval = 2500;
            processTimer.Tick += new EventHandler(updateProcessManagement);
        }

        private void blendFileBrowseButton_Click(object sender, EventArgs e)
        {
            var blendFileBrowseDialog = new OpenFileDialog();
            blendFileBrowseDialog.Filter = "Blend|*.blend";

            var result = blendFileBrowseDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                blendFilePath             = blendFileBrowseDialog.FileName;
                //blendFilePathTextBox.Text = blendFilePath;
				loadBlend();
            }

        }

        private void partsFolderBrowseButton_Click(object sender, EventArgs e)
        {
            var partsFolderBrowseDialog = new FolderBrowserDialog();
            //outFileBrowseDialog.Filter = "Blend|*.blend";

            var result = partsFolderBrowseDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                outFolderPath = partsFolderBrowseDialog.SelectedPath;
                partsFolderPathTextBox.Text = outFolderPath;
            }
        }

        private void outFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {
            outFolderPath = partsFolderPathTextBox.Text;
        }

        private void renderChunkButton_Click(object sender, EventArgs e)
        {
            appState = AppStates.RENDERING_CHUNK_ONLY;
            startTime = DateTime.Now;
            totalTimeLabel.Text = "00:00:00";
            lastChunkStarted = true;
            processTimer.Enabled = true;
            renderCurrentChunk();
            updateUI();
        }
        private void renderCurrentChunk()
        {
            Process p = new Process();
            p.StartInfo.WorkingDirectory = outFolderPath;
            p.StartInfo.FileName = "blender";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            
            if (chunkEnd == totalEnd)
            {
                lastChunkStarted = true;
            }
            
            p.StartInfo.Arguments = String.Format("-b \"{0}\" -o {1} -E {2} -s {3} -e {4} -a",
                                                  blendFilePath,
                                                  outFolderPath + "\\" + chunksSubfolder + "\\" + blendData.ProjectName + "-#",
                                                  rendererComboBox.Text,
                                                  chunkStart,
                                                  chunkEnd
                                                  );

            Trace.WriteLine(String.Format("CEW: {0}", p.StartInfo.Arguments));

            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(chunkRendered);

            p.Start();

            processes.Add(p);
        }

        private void chunkRendered(object sender, EventArgs e)
        {
            processes.Remove((Process) sender);
            Trace.WriteLine(processes.Count);
        }

        private void prevChunkButton_Click(object sender, EventArgs e)
        {

            if (chunkStart - chunkLength < totalStart)
            {
                chunkStart = totalStart;
                chunkEnd = totalStart + chunkLength;
            }

            else
            {
                chunkEnd = chunkStart - 1;
                chunkStart = chunkEnd - chunkLength;
            }
            updateUI();
        }
        private void moveToNextChunk()
        {

            if (chunkEnd + 1 > totalEnd)
            {
                
            }
            else
            {
                chunkStart = chunkEnd + 1;

                if (chunkEnd + chunkLength + 1 > totalEnd)
                {
                    chunkEnd = totalEnd;
                }
                else
                {
                    chunkEnd += chunkLength + 1;
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
                stopRender();
                renderAllButton.Text = "Render All";
            }
            //we want to start render
            else {
                var chunksPath = Path.Combine(outFolderPath, chunksSubfolder);
                if (Directory.Exists(chunksPath) && Directory.GetFiles(chunksPath).Length > 0)
                {
                    // Configure the message box to be displayed
                    DialogResult dialogResult = MessageBox.Show("Can I delete all previously rendered chunks?",
                                                                "Chunks folder not empty",
                                                                MessageBoxButtons.YesNo,
                                                                MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.No) {
                        return;
                    }
                    Helper.clearFolder(chunksPath);
                    renderAllButton_Click(null, null);
                }
                startRenderAll();
                renderAllButton.Text = "Stop";
            }

        }
        
        private void startRenderAll()
        {
            appState = AppStates.RENDERING_ALL;
            startTime = DateTime.Now;
            totalTimeLabel.Text = "00:00:00";
            processTimer.Enabled = true;
            renderCurrentChunk();
            moveToNextChunk();
        }
        private void stopRender()
        {
            
            //Show time run
            TimeSpan runTime = DateTime.Now - startTime;
            totalTimeLabel.Text = String.Format("{0,2:D2}:{1,2:D2}:{2,2:D2}", (int)runTime.TotalHours, runTime.Minutes, runTime.Seconds);
            //startTime = DateTime.MaxValue;
            
            foreach (var process in processes.ToList())
            {
                process.CloseMainWindow();
                process.Dispose();
                processes.Remove(process);
            }
            chunkStart = totalStart;
            processTimer.Enabled = false;
            lastChunkStarted = false;
            renderProgressBar.Value = 0;


            appState = AppStates.READY;
            resetChunkStartEnd();
            updateUI();
        }

        private void updateProcessManagement(object sender, EventArgs e)
        {

            if(lastChunkStarted)
            {
                renderProgressBar.Value = 100;
            }
            else
            {
                renderProgressBar.Value = (int)((chunkEnd / totalEnd) * 100);

                if(processes.Count < processCountNumericUpDown.Value)
                {
                    renderCurrentChunk();
                    moveToNextChunk();
                }
            }

			TimeSpan runTime = DateTime.Now - startTime;
			totalTimeLabel.Text = String.Format( "{0,2:D2}:{1,2:D2}:{2,2:D2}", (int)runTime.TotalHours, runTime.Minutes, runTime.Seconds );

            if (processes.Count == 0)
            {
                stopRender();
                resetChunkStartEnd();
                return;
            }
            updateUI();
        }
        
        private void concatenatePartsButton_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(outFolderPath))
            {
                errorMsgs(-100);
                return;
            }
            string chunksPath = Path.Combine(outFolderPath, chunksSubfolder);
            string chunksTxtPath = Path.Combine(chunksPath, "partList.txt");
            string videoExtensionFound = "";
            string audioFileName = blendData.ProjectName + "." + audioFormat;
            string audioSettings = string.Empty; //"-c:a aac -b:a 256k";


            List<string> fileList = new List<string>();
            foreach (var format in allowedFormats)
            {
                videoExtensionFound = format;
                fileList = Directory.GetFiles(chunksPath, "*." + videoExtensionFound, SearchOption.TopDirectoryOnly).ToList();
                if (fileList.Count > 0) break;
            }

            // no chunks found
            if (fileList.Count == 0) return;

            //audio found
            var audioArgument = "";
            if (!File.Exists(Path.Combine(outFolderPath, audioFileName)))
            {
                audioFileName = string.Empty;
                audioSettings = string.Empty;
            } else
            {
                audioArgument = "-i " + Path.Combine(outFolderPath, audioFileName);
            }

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


            //write txt for ffmpeg concat
            StreamWriter partListWriter = new StreamWriter(chunksTxtPath);
            foreach (var filePath in fileListSorted)
            {
                partListWriter.WriteLine("file '{0}'", filePath);
            }
            partListWriter.Close();


            Process ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ffmpegProcess.StartInfo.FileName = "ffmpeg";
            ffmpegProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

            ffmpegProcess.StartInfo.Arguments = String.Format("-f concat -safe 0 -i {0} {1} -c:v copy {2} {3}.{4} -y",
                                                   chunksTxtPath,
                                                   audioArgument,
                                                   audioSettings,
                                                   Path.Combine(outFolderPath, blendData.ProjectName),
                                                   videoExtensionFound
                                    );
            Trace.WriteLine(ffmpegProcess.StartInfo.Arguments);
            ffmpegProcess.Start();
        }
		private void loadBlend() {

            if ( !File.Exists( blendFilePath) ) {
                // file does not exist
                errorMsgs(-104);
                return;
			}

            if (!Directory.Exists(scriptsPath))
            {
                // Error scriptsfolder not found
                string caption = "Error";
                string message = "Scripts folder not found";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process p = new Process();
            //p.StartInfo.WorkingDirectory     = outFolderPath;
            p.StartInfo.WorkingDirectory       = scriptsPath;
            p.StartInfo.FileName               = "blender";
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.CreateNoWindow         = true;
			p.StartInfo.UseShellExecute        = false;

            p.StartInfo.Arguments = String.Format("-b \"{0}\" -P \"{1}\"",
                                                  blendFilePath,
                                                  Path.Combine(scriptsPath, "get_project_info.py")
                                    );

			try {
				p.Start();
			}
			catch( Exception ex ) {
				Trace.WriteLine( ex );
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
				blendData = serializer.Deserialize<BlenderData>( jsonInfo.ToString() );

            }

			if( blendData != null ) {
                
                totalStart       = blendData.StartFrame;
				totalEnd         = blendData.EndFrame;
                blendFileLabel.Text = "1. Blend File:";
                blendFileNameLabel.Text = " " + blendData.ProjectName + ".blend";


                //chunkEndNumericUpDown.Text = int.Parse(chunkStartNumericUpDown.Text) + chunkEndNumericUpDown.Text;

                // Remove last bit from file path, if checked

                partsFolderPathTextBox.Text = blendData.AltDir;
                

                //outFolderPathTextBox.Text          = outFolderPath = blendData.AltDir;
                infoActiveScene.Text               = blendData.ActiveScene;
                infoFramerate.Text                 = blendData.Framerate.ToString();
                infoNoScenes.Text                  = blendData.NumScenes;
                ErrorCode                          = blendData.ErrorCode;

                chunkEnd = chunkLength + chunkStart;

                if ( blendData.EndFrame < chunkEnd ) {
					chunkEnd = blendData.EndFrame;
				}

			}

            // Error checker
            errorMsgs(ErrorCode);

            resetChunkStartEnd();
            updateUI();

            Trace.WriteLine( "Json data = " + jsonInfo.ToString() );

		}

        /// <summary>
        /// Error central, displays message and does actions
        /// according to given code, return's int equal to 
        /// error code
        /// </summary>
        /// <param name="er"></param>
        /// <returns>same as er</returns>
        int errorMsgs(int er)
        {
            int input = er;

            // Actions

            // disable buttons if invalid
            var invalid_list = new List<int> { -1, -2, -3, -104 };
            var isbad = invalid_list.Contains(input);
            if (isbad == true)
            {
                appState = AppStates.AFTER_START;
            }
            else
            {
                appState = AppStates.READY;
            }

            // Messages
            string message;
            string caption = string.Format("Error ({0})", input);

            if (input == -1)
            {
                message = "Output file path empty, please set a valid path in project";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            else if (input == -2)
            {
                message = "Invalid Output path";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -2;
            }
            else if (input == -3)
            {
                message = "Output path is relative, you MUST use absolute paths ONLY";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -3;
            }
            else if (input == -100)
            {
                message = "FFmpeg can't find working folder";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -100;
            }
            else if (input == -104)
            {
                message = "Invalid project";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -104;
            }
            else
            {
                // no problems, don't show error message
                return 0;
            }
        }

		private void reloadBlenderDataButton_Click( object sender, EventArgs e ) {

            loadBlend();
		}

        private void MixdownAudio_Click(object sender, EventArgs e) {

            if (!File.Exists(blendFilePath)) {
                return;
            }

            if (!Directory.Exists(scriptsPath))
            {
                // Error scriptsfolder not found
                string caption = "Error";
                string message = "Scripts folder not found. Separate audio mixdown and automatic project info detection will not work, but you can still use the basic rendering functionality.";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            if (!Directory.Exists(partsFolderPathTextBox.Text)) {
                Directory.CreateDirectory(partsFolderPathTextBox.Text);
            }

            Process p = new Process();

            p.StartInfo.FileName = "blender";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            //Using minimized instead so we get feedback
            //p.StartInfo.CreateNoWindow         = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

            p.StartInfo.Arguments = String.Format("-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- {4}",
                                                  blendFilePath,
                                                  chunkStart,
                                                  totalEnd,
                                                  Path.Combine(scriptsPath, "mixdown_audio.py"),
                                                  outFolderPath
                                    );
            Trace.WriteLine(p.StartInfo.Arguments);
            p.Start();

            p.WaitForExit((int)TimeSpan.FromMinutes(5).TotalMilliseconds);

            Trace.WriteLine("MixDown Completed");


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
            json_info op = new json_info();
            op.Show();
        }

        private void ajustOutDir_CheckedChanged(object sender, EventArgs e)
        {
            loadBlend();
        }

        // DEBUG OPTIONS
        private void debugMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugShow.Checked == false)
            {
                debugToolStripMenuItem.Visible = false;
            }
            else if (debugShow.Checked == true)
            {
                debugToolStripMenuItem.Visible = true;
            }
        }

        void jsonDel()
        {
            // delete json
            string jsonfile = Path.Combine(scriptsPath, "blend_info.json");
            if (File.Exists(jsonfile))
            {
                File.Delete(jsonfile);
                //MessageBox.Show("Json deleted", "Ok");
            }
            else
            {
                //MessageBox.Show("Json not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void deleteJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jsonDel();
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
            Process.Start(outFolderPath);
        }

        //total start numericUpDown change
        private void totalStartNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            totalStart = totalStartNumericUpDown.Value;
            if (totalStart >= totalEnd)
            {
                totalEnd = totalStart + chunkLength;
            }
            chunkStart = totalStart;
            resetChunkStartEnd();
        }
        //total end numericUpDown change
        private void totalEndNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            totalEnd = totalEndNumericUpDown.Value;
            if (totalEnd <= totalStart)
            {
                totalStart -= chunkLength;
            }
            if (totalStart < 0) totalStart = 0;
            resetChunkStartEnd();
        }

        //chunk length numericUpDown change
        private void chunkLengthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            chunkLength = chunkLengthNumericUpDown.Value;
            chunkStart = totalStart;
            resetChunkStartEnd();
        }

        private void resetChunkStartEnd()
        {
            if(chunkStart >= totalEnd)
            {
                chunkStart = 0;
            }
            if (chunkStart + chunkLength > totalEnd)
            {
                chunkLength = totalEnd - totalStart;
            }
            else
            {
                chunkEnd = chunkStart + chunkLength;
            }
            updateUI();
        }
    }
}
