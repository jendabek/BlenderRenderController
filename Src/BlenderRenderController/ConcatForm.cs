using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using BRClib;


namespace BlenderRenderController
{
    public partial class ConcatForm : Form
    {

        public string ChunksTextFile { get; set; }
        public string OutputFile { get; set; }
        public string MixdownAudioFile { get; set; }


        public ConcatForm()
        {
            InitializeComponent();
        }

        private void joinCancelButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;

            if (btn.Name == joinButton.Name)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;

            Close();
        }


        private void changeChunksTextFileButton_Click(object sender, EventArgs e)
        {
            var saveDialog = new OpenFileDialog()
            {
                Title = "Select FFmpeg's concatenation file (chunklist.txt)",
                CheckFileExists = true
            };

            var res = saveDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                chunksTxtFileTextBox.Text = saveDialog.FileName;
            }

        }

        private void changeOutputFileButton_Click(object sender, EventArgs e)
        {
            var exts = RenderFormats.AllowedFileExts;

            var saveDialog = new SaveFileDialog()
            {
                Title = "Set final video name and location",
                Filter = MakeDiagFilter(exts),
                CheckPathExists = true,
            };

            var res = saveDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                outputFileTextBox.Text = saveDialog.FileName;
            }
        }

        private void changeMixdownFileButton_Click(object sender, EventArgs e)
        {
            var exts = RenderFormats.AllowedAudioFileExts;

            var saveDialog = new OpenFileDialog()
            {
                Title = "Select mixdown audio file",
                Filter = MakeDiagFilter(exts),
                CheckFileExists = true,
            };

            var res = saveDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                mixdownFileTextBox.Text = saveDialog.FileName;
            }

        }

        private string MakeDiagFilter(string[] extentions)
        {
            var sb = new StringBuilder();

            foreach (var ext in extentions)
            {
                sb.AppendFormat("{0} files (*.{0})|*.{0}", ext);
                sb.Append('|');
            }

            sb.Append("All files (*.*)|*.*");

            return sb.ToString();
        }

        private void ConcatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MixdownAudioFile = mixdownFileTextBox.Text;
            OutputFile = outputFileTextBox.Text;
            ChunksTextFile = chunksTxtFileTextBox.Text;


        }
    }
}
