// For Mono compatible Unix builds compile with /d:UNIX

using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace BlenderRenderController
{
    public partial class SettingsForm : Form
    {

        private AppSettings _setts;

        public SettingsForm()
        {
            InitializeComponent();
            _setts = AppSettings.Current;
        }

        private void onFormLoad(object sender, EventArgs e)
        {
            // bindings
            blenderPathTextBox.DataBindings.Add("Text", _setts, nameof(AppSettings.BlenderProgram));
            ffmpegPathTextBox.DataBindings.Add("Text", _setts, nameof(AppSettings.FFmpegProgram));
            chkBoxShowTooltips.DataBindings.Add("Checked", _setts, nameof(AppSettings.DisplayToolTips));
            chkBoxDelChunks.DataBindings.Add(nameof(CheckBox.Checked), _setts, nameof(AppSettings.DeleteChunksFolder));

            cbLoggingLvl.SelectedIndex = _setts.LoggingLevel;
            cbLoggingLvl.SelectedIndexChanged += CbLoggingLvl_SelectedIndexChanged;

            if (!File.Exists(_setts.BlenderProgram))
            {
                blenderPathTextBox.Text = string.Empty;
            }
            if (!File.Exists(_setts.FFmpegProgram))
            {
                ffmpegPathTextBox.Text = string.Empty;
            }

            findBlenderDialog.Filter = "Blender|" + _setts.BlenderExeName;
            findBlenderDialog.Title += _setts.BlenderExeName;

            findFFmpegDialog.Filter = "FFmpeg|" + _setts.FFmpegExeName;
            findFFmpegDialog.Title += _setts.FFmpegExeName;
#if UNIX
            ffmpegPathTextBox.BackColor =
            blenderPathTextBox.BackColor = System.Drawing.Color.White;
#endif
        }

        private void CbLoggingLvl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _setts.LoggingLevel = cbLoggingLvl.SelectedIndex;
        }

        private void blenderChangePathButton_Click(object sender, EventArgs e)
        {
            findBlenderDialog.InitialDirectory = blenderPathTextBox.Text.Trim();

            var result = findBlenderDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                blenderPathTextBox.Text = findBlenderDialog.FileName;
            }
        }

        private void ffmpegChangePathButton_Click(object sender, EventArgs e)
        {
            findFFmpegDialog.InitialDirectory = ffmpegPathTextBox.Text.Trim();

            var result = findFFmpegDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ffmpegPathTextBox.Text = findFFmpegDialog.FileName;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {

            Close();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_setts.CheckCorrectConfig())
            {
                this.DialogResult = DialogResult.Abort;
                //Application.Exit();
            }

        }

    }
}
