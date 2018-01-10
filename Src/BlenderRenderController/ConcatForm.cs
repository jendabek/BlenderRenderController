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
using static BRClib.RenderFormats;

namespace BlenderRenderController
{
    public partial class ConcatForm : Form
    {
        string _dialogBasePath, _chunksFolder;

        public string ChunksTextFile { get; set; }

        public string OutputFile { get; set; }

        public string MixdownAudioFile { get; set; }


        public ConcatForm()
        {
            InitializeComponent();
        }

        public ConcatForm(string outputPath, string projName) : this()
        {
            _dialogBasePath = outputPath;

            var chunksFolder = Path.Combine(_dialogBasePath, Constants.ChunksSubfolder);
            var chunkFiles = Directory.GetFiles(chunksFolder);
            var ext = Path.GetExtension(chunkFiles.FirstOrDefault() ?? "");

            ChunksTextFile = Path.Combine(chunksFolder, Constants.ChunksTxtFileName);
            OutputFile = Path.Combine(_dialogBasePath, projName + ext);
        }

        private void ConcatForm_Shown(object sender, EventArgs e)
        {
            // run validation on start
            ValidateFields(changeChunksFolderButton);
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
                    MessageBox.Show("One or more fields are not valid", "Error", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            var btn = sender as Button;

            var openDialog = new OpenFileDialog()
            {
                Title = "Select FFmpeg's concatenation script file",
                CheckFileExists = true
            };

            var res = openDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                chunksTxtFileTextBox.Text = openDialog.FileName;
                _chunksFolder = Path.GetDirectoryName(openDialog.FileName);
            }

            ValidateFields(btn);
        }

        private void changeOutputFileButton_Click(object sender, EventArgs e)
        {
            var exts = VideoFileExts;
            var btn = sender as Button;

            var saveDialog = new SaveFileDialog()
            {
                Title = "Set final video name and location",
                Filter = MakeDiagFilter(exts),
                FilterIndex = GetPreferedExtIndex(exts),
                CheckPathExists = true,
            };

            var res = saveDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                outputFileTextBox.Text = saveDialog.FileName;
            }

            ValidateFields(btn);
        }

        private void changeMixdownFileButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;

            var openDialog = new OpenFileDialog()
            {
                Title = "Select mixdown audio file",
                Filter = MakeDiagFilter(AudioFileExts),
                FilterIndex = 0,
                CheckFileExists = true,
            };

            var res = openDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                mixdownFileTextBox.Text = openDialog.FileName;
            }

            ValidateFields(btn);
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
            var filter = "All files (*.*)|*.*|";

            // build filter entries from extentions
            var fExts = extentions.Select(s => s.Where(c => char.IsLetterOrDigit(c)))
                                  .Select(iec => new string(iec.ToArray()))
                                  .Select(a => new { desc = a.ToUpper(), ext = a })
                                  .Select(a => $"{a.desc} files (*.{a.ext})|*.{a.ext}");

            filter += string.Join("|", fExts);

            return filter;
        }

        private int GetPreferedExtIndex(string[] extentions)
        {
            if (string.IsNullOrEmpty(_chunksFolder))
            {
                return 0;
            }

            var afe = extentions.ToList();

            var cFileExts = Directory.GetFiles(_chunksFolder)
                .Select(f => Path.GetExtension(f));

            var idx = afe.FindIndex(f => cFileExts.Contains(f));

            // +2 to compensate for 'all files' entry and beeing 1 based
            return idx + 2;
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

        private void ValidateFields(Control original)
        {
            chunksTxtFileTextBox.Focus();
            outputFileTextBox.Focus();
            mixdownFileTextBox.Focus();

            original.Focus();
        }
    }
}
