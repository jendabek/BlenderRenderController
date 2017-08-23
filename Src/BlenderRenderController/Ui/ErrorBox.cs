using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController.Ui
{
    public partial class ErrorBox : Form
    {
        private string _result;

        public string Result
        {
            get { return _result; }
        }

        public enum Buttons
        {
            Ok, RetryAbortIgnore, YesNo
        }

        public ErrorBox()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            ErrorIcon.Image = SystemIcons.Hand.ToBitmap();
            ErrorIcon.SizeMode = PictureBoxSizeMode.CenterImage;

        }

        public ErrorBox(string label, string title, IEnumerable<string> contents, Buttons bnts = Buttons.Ok)
            : this()
        {

            ErrorBoxLabel.Text = label;
            this.Text = title;
            ErrorContentBox.Lines = contents.ToArray();

            switch (bnts)
            {
                case Buttons.Ok:
                    BntLeft.Visible =
                    BntRight.Visible = false;
                    BntMiddle.Visible = true;
                    BntMiddle.Text = "Ok";
                    break;
                case Buttons.RetryAbortIgnore:
                    BntMiddle.Visible = true;
                    BntLeft.Text = "Retry";
                    BntMiddle.Text = "Abort";
                    BntRight.Text = "Ignore";
                    break;
                case Buttons.YesNo:
                    BntLeft.Text = "Yes";
                    BntRight.Text = "No";
                    break;
                default:
                    break;
            }
        }

        public ErrorBox(string message, string title, string contents, Buttons bnts = Buttons.Ok)
            :this(message, title, new List<string> { contents }, bnts)
        {

        }

        private void Bnt_Click(object sender, EventArgs e)
        {
            var b = (sender as Button);
            _result = b.Text;
            this.Close();
        }

        private void ErrorBox_Shown(object sender, EventArgs e)
        {
            // play error sound
            System.Media.SystemSounds.Hand.Play();
        }

        private void ErrorBox_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (String.IsNullOrEmpty(_result))
                return;

        }
    }
}
