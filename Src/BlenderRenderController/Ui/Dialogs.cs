// For Mono compatible Unix builds compile with /d:UNIX


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WIN
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Controls;
#endif

namespace BlenderRenderController.Ui
{
    class Dialogs
    {
#if WIN
        public static TaskDialog ShowErrorBox(string textBody, string mainText, string caption, string details)
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

            return td;
        }
        public static TaskDialog ShowErrorBox(string textBody, string mainText, string details)
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

            return td;
        }
#else
        public static ErrorBox ShowErrorBox(string textBody, string mainText, string caption, string details)
        {
            string msg = mainText + "\n\n" + textBody;
            ErrorBox eb = new ErrorBox(msg, caption, details);
            return eb;
        }
        public static ErrorBox ShowErrorBox(string textBody, string mainText, string details)
        {
            ErrorBox eb = new ErrorBox(textBody, mainText, details);
            return eb;
        }
#endif
    }
}
