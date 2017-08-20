using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Controls;

namespace BlenderRenderController.Ui
{
    class Dialogs
    {
        public static CommonOpenFileDialog FolderPickerDialog
        {
            get
            {
                return new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                };
            }
        }

        public static TaskDialog ErrorBox(string textBody, string mainText, string caption, string details)
        {
            var td = new TaskDialog();
            td.Text = textBody;
            td.InstructionText = mainText;
            td.Caption = caption;

            td.DetailsExpanded = false;
            td.DetailsExpandedLabel = "Show details";
            td.DetailsExpandedText = details;
            td.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;

            td.Icon = TaskDialogStandardIcon.Error;
            td.FooterIcon = TaskDialogStandardIcon.Information;
            td.StandardButtons = TaskDialogStandardButtons.Ok;

            return td;
        }
        public static TaskDialog ErrorBox(string textBody, string mainText, string details)
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

    }
}
