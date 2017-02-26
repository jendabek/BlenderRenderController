using System.Collections.Generic;
using System.IO;
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
