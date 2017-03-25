using BlenderRenderController.newLogger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController.ui
{
    public partial class ErrorBox : Form
    {
        private string _result;

        private LogService _log = new LogService();

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

            _log.RegisterLogSevice(new FileLogger());
            _log.RegisterLogSevice(new ConsoleLogger());
        }

        public ErrorBox(string label, List<string> contents, Buttons bnts = Buttons.Ok)
            : this()
        {
            _log.Error(contents);

            ErrorBoxLabel.Text = label;
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
    }
}
