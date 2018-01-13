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

        public ErrorBox()
        {
            InitializeComponent();

            ErrorIcon.Image = SystemIcons.Hand.ToBitmap();
            ErrorIcon.SizeMode = PictureBoxSizeMode.CenterImage;

        }

        public ErrorBox(string label, string title, IEnumerable<string> contents, MessageBoxButtons bnts = MessageBoxButtons.OK)
            : this()
        {

            ErrorBoxLabel.Text = label;
            this.Text = title;
            ErrorContentBox.Lines = contents.ToArray();
            DefineButtons(bnts);
        }

        private void DefineButtons(MessageBoxButtons bnts)
        {
            // TODO: Localize text
            switch (bnts)
            {
                case MessageBoxButtons.OK:
                    BtnLeft.Visible =
                    BtnRight.Visible = false;
                    BtnMiddle.Text = "OK";

                    BtnMiddle.Tag = DialogResult.OK;
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    BtnLeft.Text = "Retry";
                    BtnMiddle.Text = "Abort";
                    BtnRight.Text = "Ignore";

                    BtnLeft.Tag = DialogResult.Retry;
                    BtnMiddle.Tag = DialogResult.Abort;
                    BtnRight.Tag = DialogResult.Ignore;
                    break;
                case MessageBoxButtons.YesNo:
                    BtnLeft.Text = "Yes";
                    BtnRight.Text = "No";

                    BtnLeft.Tag = DialogResult.Yes;
                    BtnRight.Tag = DialogResult.No;
                    break;
                case MessageBoxButtons.OKCancel:
                    BtnMiddle.Visible = false;

                    BtnLeft.Text = "OK";
                    BtnRight.Text = "Cancel";

                    BtnLeft.Tag = DialogResult.OK;
                    BtnRight.Tag = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    BtnLeft.Text = "Yes";
                    BtnMiddle.Text = "No";
                    BtnRight.Text = "Cancel";

                    BtnLeft.Tag = DialogResult.Yes;
                    BtnMiddle.Tag = DialogResult.No;
                    BtnRight.Tag = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.RetryCancel:
                    BtnMiddle.Visible = false;

                    BtnLeft.Text = "Retry";
                    BtnRight.Text = "Cancel";

                    BtnLeft.Tag = DialogResult.Retry;
                    BtnRight.Tag = DialogResult.Cancel;
                    break;
                default:
                    throw new Exception("Unknown buttons");
            }
        }

        public ErrorBox(string message, string title, string contents, MessageBoxButtons bnts = MessageBoxButtons.OK)
            :this(message, title, new List<string> { contents }, bnts)
        {

        }

        private void Bnt_Click(object sender, EventArgs e)
        {
            var b = (sender as Button);
            this.DialogResult = (DialogResult)b.Tag;
            this.Close();
        }

        private void ErrorBox_Shown(object sender, EventArgs e)
        {
            // play error sound
            System.Media.SystemSounds.Hand.Play();
        }

    }
}
