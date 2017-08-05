using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController.Ui
{
    static class Dialogs
    {
        public static OpenFileDialog FolderSelectDialog
        {
            get
            {
                var op = new OpenFileDialog();
                op.ValidateNames = false;
                op.CheckFileExists = false;
                op.CheckPathExists = true;
                op.FileName = "Folder Selection";
                return op;
            }
        }

        public static CommonOpenFileDialog FolderSelectDialogWin
        {
            get
            {
                return new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    EnsurePathExists = true,
                };
            }
        }
    }

}
