// For Mono compatible Unix builds compile with /d:UNIX

using System;
using System.Text;
using System.Windows.Forms;

#if WIN
using Microsoft.WindowsAPICodePack.Dialogs;
#endif

namespace BlenderRenderController.Ui
{
    class Dialogs
    {
#if WIN
        public static DialogResult ShowErrorBox(string textBody, string mainText, string caption, string details)
        {
            var td = new TaskDialog();
            td.Text = textBody;
            td.InstructionText = mainText;
            td.Caption = caption;

            td.DetailsExpanded = false;
            td.DetailsExpandedText = details;
            td.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;

            td.Icon = TaskDialogStandardIcon.Error;
            td.FooterIcon = TaskDialogStandardIcon.Information;
            td.StandardButtons = TaskDialogStandardButtons.Ok;

            return ToDR(td.Show());

            //return td;
        }
        public static DialogResult ShowErrorBox(string textBody, string mainText, string details)
        {
            var td = new TaskDialog();
            td.Text = textBody;
            td.InstructionText = mainText;

            td.DetailsExpanded = false;
            td.DetailsExpandedLabel = "Show details";
            td.DetailsExpandedText = details;
            td.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;

            td.Icon = TaskDialogStandardIcon.Error;
            td.FooterIcon = TaskDialogStandardIcon.Information;
            td.StandardButtons = TaskDialogStandardButtons.Ok;

            return ToDR(td.Show());

            //return td;
        }

        static DialogResult ToDR(TaskDialogResult tdr)
        {
            switch (tdr)
            {
                case TaskDialogResult.None:
                    return DialogResult.None;
                case TaskDialogResult.Ok:
                    return DialogResult.OK;
                case TaskDialogResult.Yes:
                    return DialogResult.Yes;
                case TaskDialogResult.No:
                    return DialogResult.No;
                case TaskDialogResult.Cancel:
                    return DialogResult.Cancel;
                case TaskDialogResult.Retry:
                    return DialogResult.Retry;
                case TaskDialogResult.Close:
                    return DialogResult.Abort;
                case TaskDialogResult.CustomButtonClicked:
                default:
                    throw new NotSupportedException("Task result not supported");
            }
        }
#else
        public static DialogResult ShowErrorBox(string textBody, string mainText, string caption, string details)
        {
            string msg = mainText + "\n\n" + textBody;
            ErrorBox eb = new ErrorBox(msg, caption, details);
            return eb.ShowDialog();
        }
        public static DialogResult ShowErrorBox(string textBody, string mainText, string details)
        {
            ErrorBox eb = new ErrorBox(textBody, mainText, details);
            return eb.ShowDialog();
        }
#endif
    }
}
