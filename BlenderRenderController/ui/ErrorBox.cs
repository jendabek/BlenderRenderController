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
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

            //LogService.Log.RegisterLogSevice(new FileLogger());
            //LogService.Log.RegisterLogSevice(new ConsoleLogger());
        }

        public ErrorBox(string label, List<string> contents, Buttons bnts = Buttons.Ok)
            : this()
        {
            logger.Error(contents);

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

        public ErrorBox(string message, string contents, Buttons bnts = Buttons.Ok)
            :this(message, new List<string> { contents }, bnts)
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
            logger.Error(".blend was NOT loaded");

            if (String.IsNullOrEmpty(_result))
                return;

            logger.Error($"-ErrorBox- {_result} was pressed");
        }
    }
}
