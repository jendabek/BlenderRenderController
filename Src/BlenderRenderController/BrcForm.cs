using BlenderRenderController.Properties;
using BRClib;
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
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace BlenderRenderController
{
    public partial class BrcForm : Form
    {
        private static readonly string _baseDir = Environment.CurrentDirectory;
        private readonly string _scriptsFolderPath;
        private static Logger logger = LogManager.GetLogger("BRC");
        private AppSettings _settings;
        private ProjectSettings pData;
        private ObservableCollection<Chunk> _currentChunks;


        public BrcForm()
        {
            InitializeComponent();

            pData = new ProjectSettings();
            _currentChunks = new ObservableCollection<Chunk>();
            _scriptsFolderPath = Path.Combine(_baseDir, AppInfo.ScriptsSubfolder);
            _settings = new AppSettings();
            // temp settings
            _settings.BlenderProgram = @"C:\Program Files\Blender Foundation\Blender\blender.exe";
            _settings.FFmpegProgram = @"E:\Programas\FFmpeg\Snapshot\bin\ffmpeg.exe";
        }

        private void ProjectData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {


        }
        private void BlendData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(pData.BlendData.Start):
                case nameof(pData.BlendData.End):
                    infoDuration.Text = string.Format("{0:%h}h {0:%m}m {0:%s}s {0:%f}ms", pData.Duration.GetValueOrDefault());
                    infoFramesTotal.Text = pData.TotalFrames.ToString();
                    break;
                default:
                    break;
            }
        }
        private void BrcForm_Load(object sender, EventArgs e)
        {
            pData.PropertyChanged += ProjectData_PropertyChanged;
            pData.BlendData.PropertyChanged += BlendData_PropertyChanged;
            _settings.State = AppStates.AFTER_START;

            processCountNumericUpDown.Value = Environment.ProcessorCount;
            infoActiveScene.DataBindings.Add("Text", pData, "BlendData.ActiveScene", false, DataSourceUpdateMode.OnValidation, "...");
            infoResolution.DataBindings.Add("Text", pData.BlendData, "BlendData.Resolution", false, DataSourceUpdateMode.OnValidation, "...");
            infoFramerate.DataBindings.Add("Text", pData.BlendData, "BlendData.Fps", true, DataSourceUpdateMode.OnValidation, "...", "###.##");
            infoFramesTotal.DataBindings.Add("Text", pData, "TotalFrames", false, DataSourceUpdateMode.OnPropertyChanged, "...");

            totalStartNumericUpDown.DataBindings.Add("Value", pData.BlendData, "Start");
            totalStartNumericUpDown.DataBindings.Add("Enabled", startEndCustomRadio, "Checked");
            totalEndNumericUpDown.DataBindings.Add("Value", pData.BlendData, "End");
            totalEndNumericUpDown.DataBindings.Add("Enabled", startEndCustomRadio, "Checked");

            chunkLengthNumericUpDown.DataBindings.Add("Enabled", renderOptionsCustomRadio, "Checked");
            processCountNumericUpDown.DataBindings.Add("Enabled", renderOptionsCustomRadio, "Checked");


        }



        private void PData_Chunks_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var cList = sender as IList<Chunk>;
            if (cList.Count > 0)
            {
                var len = cList.First().Length;
                chunkLengthNumericUpDown.Value = len;
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

        private void GetBlendInfo(string blendFile, string scriptName = AppInfo.PyGetInfo)
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
            if (!Directory.Exists(_scriptsFolderPath))
            {
                // Error scriptsfolder not found
                MessageBox.Show("Scripts folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            getBlendInfoCom.StartInfo.FileName = _settings.BlenderProgram;
            getBlendInfoCom.StartInfo.WorkingDirectory = new FileInfo(_settings.BlenderProgram).Directory.FullName;
            getBlendInfoCom.StartInfo.StandardOutputEncoding =
            getBlendInfoCom.StartInfo.StandardErrorEncoding = Encoding.UTF8;
            getBlendInfoCom.StartInfo.Arguments = string.Format(CommandARGS.GetInfoComARGS, 
                                                            blendFile, 
                                                            Path.Combine(_scriptsFolderPath, scriptName));

            try
            {
                getBlendInfoCom.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex);
                return;
            }

            // Get values from streams
            var streamOutput = new List<string>();
            var streamErrors = new List<string>();

            while (!getBlendInfoCom.StandardOutput.EndOfStream)
                streamOutput.Add(getBlendInfoCom.StandardOutput.ReadLine());

            while (!getBlendInfoCom.StandardError.EndOfStream)
                streamErrors.Add(getBlendInfoCom.StandardError.ReadLine());

            // log errors
            if (streamErrors.Count > 0)
                logger.Error(streamErrors);

            if (streamOutput.Count == 0)
            {
                var e = new Ui.ErrorBox("Could not open project, no information was received",
                                         streamErrors);
                e.Text = "Error";
                e.ShowDialog(this);
                //stopRender(false);
                return;
            }

            var blendData = Parsers.ParsePyOutput(streamOutput);
            pData.BlendData = blendData;
            pData.BlendPath = blendFile;
            var chunks = Chunk.CalcChunks(blendData.Start, blendData.End, 8);
            UpdateCurrentChunks(chunks);
        }

        private void UpdateCurrentChunks(params Chunk[] newChunks)
        {
            if (_currentChunks.Count > 0)
                _currentChunks.Clear();

            foreach (var chnk in newChunks)
            {
                _currentChunks.Add(chnk);
            }
        }

        private void UpdateRecentBlendsMenu()
        {
            //last blends
            recentBlendsMenu.Items.Clear();

            foreach (string item in _settings.RecentBlends)
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
            var blend = pData.BlendPath;
            if (!string.IsNullOrEmpty(blend))
            {
                GetBlendInfo(blend);
            }
        }

    }
}
