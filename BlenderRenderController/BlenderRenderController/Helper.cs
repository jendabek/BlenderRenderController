using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController
{

    static class Helper
    {

        /* public static IEnumerable<string> AlphanumericSort(this IEnumerable<string> me)
         {
             string[] Separator = new string[] { "-" };
         }*/
        static public void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
        static public void print(string text)
        {
            Trace.WriteLine(text);
        }
        static public void showErrors(List<string> errorCodes)
        {
            var errorText = "";
            foreach (var errorCode in errorCodes)
            {
                if(errorCode == AppErrorCodes.BLENDER_PATH_NOT_SET)
                {
                    errorText += "Please set correct path to Blender (blender.exe).\n";
                }
                if (errorCode == AppErrorCodes.FFMPEG_PATH_NOT_SET)
                {
                    errorText += "Please set correct path to FFmpeg (ffmpeg.exe).\n";
                }
            }
            MessageBox.Show(
                    errorText,
                    "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);

        }
    }
}
