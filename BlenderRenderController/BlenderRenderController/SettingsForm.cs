using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            blenderPathTextBox.Text = _appSettings.blenderPath;
            ffmpegPathTextBox.Text = _appSettings.ffmpegPath;
        }

        private void blenderChangePathButton_Click(object sender, EventArgs e)
        {
            _changePathDialog = new OpenFileDialog();
            _changePathDialog.Filter = "Blender|" + AppSettings.BLENDER_EXE_NAME;
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
            _changePathDialog.InitialDirectory = ffmpegPathTextBox.Text.Trim();

            var result = _changePathDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ffmpegPathTextBox.Text = Path.GetDirectoryName(_changePathDialog.FileName);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _appSettings.blenderPath = blenderPathTextBox.Text.Trim();
            _appSettings.ffmpegPath = ffmpegPathTextBox.Text.Trim();
            _appSettings.checkRequiredSettings();

            if(_appSettings.appConfigured)
            {
                _appSettings.save();
                Close();
            }
        }
    }
}
