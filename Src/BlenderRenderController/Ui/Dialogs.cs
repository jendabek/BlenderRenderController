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

    }
}
