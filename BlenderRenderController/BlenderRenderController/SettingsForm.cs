﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BlenderRenderController
{
    public partial class SettingsForm : Form
    {
        private AppSettings _appSettings;
        private OpenFileDialog _changePathDialog;

        public SettingsForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
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
                if(_appSettings.ffmpegPath == AppSettings.FFMPEG_PATH_DEFAULT)
                {
                    ffmpegPathTextBox.Visible = false;
                    ffmpegChangePathButton.Visible = false;
                    ffmpegLabel.Visible = false;
                    ffmpegDownloadLabel.Visible = false;
                }
            }
        }

        private void blenderChangePathButton_Click(object sender, EventArgs e)
        {
            _changePathDialog = new OpenFileDialog();
            _changePathDialog.Filter = "Blender|" + AppSettings.BLENDER_EXE_NAME;
            _changePathDialog.Title = "Find " + AppSettings.BLENDER_EXE_NAME;
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
            _changePathDialog.Filter = "FFmpeg|" + AppSettings.FFMPEG_EXE_NAME;
            _changePathDialog.Title = "Find " + AppSettings.FFMPEG_EXE_NAME;
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
            int oldDefChunkLen = _appSettings.processCheckInterval;

            _appSettings.blenderPath = blenderPathTextBox.Text.Trim();
            _appSettings.ffmpegPath = ffmpegPathTextBox.Text.Trim();
            _appSettings.processCheckInterval = Convert.ToInt32(DefChunkLen.Value);

            _appSettings.checkCorrectConfig();

            if (!_appSettings.checkBlenderPath()) _appSettings.blenderPath = oldBlenderPath;
            if (!_appSettings.checkFFmpegPath()) _appSettings.ffmpegPath = oldFFmpegPath;

            if(_appSettings.appConfigured)
            {
                _appSettings.save();
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

        private void DefChunkLen_ValueChanged(object sender, EventArgs e)
        {
            // make Default ChunkLen user setting...
        }
    }
}
