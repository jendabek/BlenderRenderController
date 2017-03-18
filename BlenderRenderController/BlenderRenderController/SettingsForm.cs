using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using BlenderRenderController.newLogger;

namespace BlenderRenderController
{
    public partial class SettingsForm : Form
    {
        private AppSettings _appSettings;
        private OpenFileDialog _changePathDialog;
        private LogService _log = new LogService();
		PlatformID Os = Environment.OSVersion.Platform;

        public SettingsForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

			PlatAdjust (Os);
        }
        public void init(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        private void onFormLoad(object sender, EventArgs e)
        {
            if(_appSettings.checkBlenderPath()) blenderPathTextBox.Text = _appSettings.blenderPath;
            if (_appSettings.checkFFmpegPath())
            {
                ffmpegPathTextBox.Text = _appSettings.ffmpegPath;
                
                //ffmpeg exe is in app directory == FULL VERSION
                //if(_appSettings.ffmpegPath == AppSettings.FFMPEG_PATH_DEFAULT)
				if(_appSettings.ffmpegPath == _appSettings.FFmpegPathDefault)
                {
                    ffmpegPathTextBox.Visible = true;
					ffmpegPathTextBox.Enabled = false;
                    ffmpegChangePathButton.Visible = true;
					ffmpegChangePathButton.Enabled = false;
                    ffmpegLabel.Visible = true;
                    ffmpegDownloadLabel.Visible = false;
                }
            }

            chkBoxVerboseLog.Checked = _appSettings.verboseLog;
        }

        private void blenderChangePathButton_Click(object sender, EventArgs e)
        {
            _changePathDialog = new OpenFileDialog();
            //_changePathDialog.Filter = "Blender|" + AppSettings.BLENDER_EXE_NAME;
            //_changePathDialog.Title = "Find " + AppSettings.BLENDER_EXE_NAME;
			_changePathDialog.Filter = "Blender|" + _appSettings.BlenderExeName;
			_changePathDialog.Title = "Find " + _appSettings.BlenderExeName;
            _changePathDialog.InitialDirectory = blenderPathTextBox.Text.Trim();

            var result = _changePathDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                blenderPathTextBox.Text = Path.GetDirectoryName(_changePathDialog.FileName);
            }
        }

        private void ffmpegChangePathButton_Click(object sender, EventArgs e)
        {
            _changePathDialog = new OpenFileDialog();
            //_changePathDialog.Filter = "FFmpeg|" + AppSettings.FFMPEG_EXE_NAME;
            //_changePathDialog.Title = "Find " + AppSettings.FFMPEG_EXE_NAME;
			_changePathDialog.Filter = "FFmpeg|" + _appSettings.FFmpegExeName;
			_changePathDialog.Title = "Find " + _appSettings.FFmpegExeName;
            _changePathDialog.InitialDirectory = ffmpegPathTextBox.Text.Trim();

            var result = _changePathDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ffmpegPathTextBox.Text = Path.GetDirectoryName(_changePathDialog.FileName);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string oldBlenderPath = _appSettings.blenderPath;
            string oldFFmpegPath = _appSettings.ffmpegPath;
            _appSettings.blenderPath = blenderPathTextBox.Text.Trim();
            _appSettings.ffmpegPath = ffmpegPathTextBox.Text.Trim();

            _appSettings.checkCorrectConfig();

            if (!_appSettings.checkBlenderPath()) _appSettings.blenderPath = oldBlenderPath;
            if (!_appSettings.checkFFmpegPath()) _appSettings.ffmpegPath = oldFFmpegPath;

            if(_appSettings.appConfigured)
            {
                _appSettings.save(); _log.Info("Settings saved");
                Close();
            }
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _appSettings.checkCorrectConfig(false);
            if (!_appSettings.appConfigured)
            {
                Application.Exit();
            }
        }

        private void ffmpegDownloadLabel_Click(object sender, EventArgs e)
        {
            Process.Start(AppStrings.FFMPEG_DOWNLOAD_URL);
        }

        private void clearRecentBnt_Click(object sender, EventArgs e)
        {
            // does clear the recent blends list, but only takes effect 
            // after closing and re-opening
            _appSettings.clearRecentBlend();
        }

        private void chkBoxVerboseLog_Click(object sender, EventArgs e)
        {
            _appSettings.verboseLog = (sender as CheckBox).Checked;
            _appSettings.save();
        }
    }
}
