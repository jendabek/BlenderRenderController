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
        string dialogBasePath;

        public string ChunksTextFile { get; set; }
        public string OutputFile { get; set; }
        public string MixdownAudioFile { get; set; }


        public ConcatForm()
        {
            InitializeComponent();
        }

        public ConcatForm(string outputPath, string projName) : this()
        {
            dialogBasePath = outputPath;

            var chunksFolder = Path.Combine(dialogBasePath, Constants.ChunksSubfolder);
            var chunkFiles = Directory.GetFiles(chunksFolder);
            var ext = Path.GetExtension(chunkFiles.FirstOrDefault() ?? "");

            ChunksTextFile = Path.Combine(chunksFolder, Constants.ChunksTxtFileName);
            OutputFile = Path.Combine(dialogBasePath, projName + ext);
        }

        private void ConcatForm_Shown(object sender, EventArgs e)
        {
            // run validation on start
            Entries_Validating(chunksTxtFileTextBox, new CancelEventArgs());
            Entries_Validating(outputFileTextBox, new CancelEventArgs());
        }

        private void ConcatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // set properties on close
            MixdownAudioFile = mixdownFileTextBox.Text;
            OutputFile = outputFileTextBox.Text;
            ChunksTextFile = chunksTxtFileTextBox.Text;
        }

        private void joinCancelButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;

            if (btn.Name == joinButton.Name)
            {
                if (!CheckEntries())
                {
                    MessageBox.Show("One or more fields are not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult = DialogResult.OK;
            }
            else
                DialogResult = DialogResult.Cancel;

            Close();
        }


        private void changeChunksTextFileButton_Click(object sender, EventArgs e)
        {
            var saveDialog = new OpenFileDialog()
            {
                Title = "Select FFmpeg's concatenation file",
                CheckFileExists = true
            };

            var res = saveDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                chunksTxtFileTextBox.Text = saveDialog.FileName;
            }

            Entries_Validating(chunksTxtFileTextBox, new CancelEventArgs());
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

            Entries_Validating(outputFileTextBox, new CancelEventArgs());
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

            Entries_Validating(mixdownFileTextBox, new CancelEventArgs());
        }

        private void Entries_Validating(object sender, CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            var content = textBox.Text;
            bool hasErrors = false;

            if (textBox.Name == chunksTxtFileTextBox.Name)
            {
                if (string.IsNullOrWhiteSpace(content) || !File.Exists(content))
                {
                    errorProvider.SetError(textBox, "Select the concat text file");
                    hasErrors = true;
                }
            }
            else if (textBox.Name == outputFileTextBox.Name)
            {
                if (string.IsNullOrWhiteSpace(content) 
                || !Directory.Exists(Path.GetDirectoryName(content)))
                {
                    errorProvider.SetError(textBox, "Set the output file name and location");
                    hasErrors = true;
                }
            }
            else if (textBox.Name == mixdownFileTextBox.Name)
            {
                if (content.Length > 0 && !File.Exists(content))
                {
                    errorProvider.SetError(textBox, "Select the mixdown audio file, or leave empty");
                    hasErrors = true;
                }
            }

            if (!hasErrors)
            {
                errorProvider.SetError(textBox, string.Empty);
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

        private bool CheckEntries()
        {
            string[] allErrors =
            {
                errorProvider.GetError(chunksTxtFileTextBox),
                errorProvider.GetError(outputFileTextBox),
                errorProvider.GetError(mixdownFileTextBox)
            };

            return allErrors.All(e => string.IsNullOrEmpty(e));
        }

    }
}
