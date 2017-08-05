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


namespace BlenderRenderController
{
    public partial class BrcForm : Form
    {
        private static readonly string _baseDir = Environment.CurrentDirectory;
        private readonly string _scriptsFolderPath;
        private static Logger logger = LogManager.GetLogger("BRC");
        private AppSettings _settings;
        private ProjectData pData;

        public BrcForm()
        {
            _scriptsFolderPath = Path.Combine(_baseDir, AppInfo.ScriptsSubfolder);
            _settings = new AppSettings();
        }

        private void BrcForm_Load(object sender, EventArgs e)
        {

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
            projectDataBindingSource.Add(blendData);
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
                    //pData.blendFilePath = recentItem.ToolTipText;
                    //loadBlend();
                };
                recentBlendsMenu.Items.Add(menuItem);
            }
        }

        private void projectDataBindingSource_DataMemberChanged(object sender, EventArgs e)
        {

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
    }
}
