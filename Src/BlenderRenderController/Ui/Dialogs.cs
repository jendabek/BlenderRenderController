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
        public static DialogResult ShowErrorBox(string textBody, string mainText, string caption, string details)
        {
#if WIN
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
#else
            string msg = mainText + "\n\n" + textBody;
            ErrorBox eb = new ErrorBox(msg, caption, details);
            return eb.ShowDialog();
#endif
        }

        public static DialogResult ShowErrorBox(string textBody, string mainText, string details)
        {
#if WIN
            return ShowErrorBox(textBody, mainText, null, details);
#else
            var eb = new ErrorBox(textBody, mainText, details);
            return eb.ShowDialog();
#endif
        }


        public static string OutputFolderSelection(string title, string initialDir)
        {
#if WIN
            var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = initialDir,
                IsFolderPicker = true,
                Title = title,
            };

            var res = dialog.ShowDialog();
            string path = null;

            if (res == CommonFileDialogResult.Ok)
            {
                path = dialog.FileName;
            }

            //return ((DialogResult)res, path);
            return path;
#else
            var dialog = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = initialDir,
                ShowNewFolderButton = true,
                Description = title,
            };

            var res = dialog.ShowDialog();
            string path = null;

            if (res == DialogResult.OK)
            {
                path = dialog.SelectedPath;
            }

            //return (res, path);
            return path;
#endif
        }





#if WIN
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
#endif
    }
}
