using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using BlenderRenderController.newLogger;

namespace BlenderRenderController
{
    static class Helper
    {
        /* public static IEnumerable<string> AlphanumericSort(this IEnumerable<string> me)
         {
             string[] Separator = new string[] { "-" };
         }*/
        static LogService _log = new LogService();

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
        
        static public void showErrors(List<string> errorCodes, MessageBoxIcon icon = MessageBoxIcon.Asterisk, string arg1 = "")
        {
            RegsterLog();

            var errorText = "";

            foreach (var errorCode in errorCodes)
                errorText += SetErrorText(errorCode, arg1);

            MessageBox.Show(
                    errorText,
                    "",
                    MessageBoxButtons.OK,
                    icon);

            _log.Warn("-Helper- " + errorText);
        }

        static public void showErrors(string errorCode, MessageBoxIcon icon = MessageBoxIcon.Asterisk, string arg1 = "")
        {
            RegsterLog();

            var errorText = "";

            errorText += SetErrorText(errorCode, arg1);

            MessageBox.Show(
                errorText,
                "",
                MessageBoxButtons.OK,
                icon);

            _log.Warn("-Helper- " + errorText);
        }

        static private string SetErrorText(string code, string arg1)
        {
            var errorText = "";
            var appSettings = new AppSettings();

            if (code == AppErrorCodes.BLENDER_PATH_NOT_SET)
            {
                errorText += $"Please set correct path to Blender ({appSettings.BlenderExeName}).\n";
            }
            if (code == AppErrorCodes.FFMPEG_PATH_NOT_SET)
            {
                errorText += $"Please set correct path to FFmpeg ({appSettings.FFmpegExeName}).\n";
            }
            if (code == AppErrorCodes.BLEND_FILE_NOT_EXISTS)
            {
                errorText += "File does not exists anymore.\n";
                errorText += "It was removed from the list of recent blends.\n";
            }
            if (code == AppErrorCodes.RENDER_FORMAT_IS_IMAGE)
            {
                errorText += "The render format is " + arg1 + " image.\n";
                errorText += "You can render an image sequence with this tool but you will need to make a video with other SW.\n";
            }
            if (code == AppErrorCodes.BLEND_OUTPUT_INVALID)
            {
                errorText += "Unable to read output path, using project location.\n";
            }
            if (code == AppErrorCodes.UNKNOWN_OS)
            {
                errorText += "Could not identify operating system, BRC might not work properly.\n";
            }

            return errorText;
        }

        static public string fixPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var ex = new ArgumentNullException(path, "Path passed is null and cannot be fixed.");
                _log.Error(ex.Message);
                throw ex;
            }

            return path.Trim().TrimEnd('\\');
        }


        static void RegsterLog()
        {
            _log.RegisterLogSevice(new ConsoleLogger());
            _log.RegisterLogSevice(new FileLogger());
        }
      
        static public string secondsToString(double seconds, bool digital = false)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            string timeString;
            if (!digital) {
                timeString = string.Format("{0:D1}h {1:D1}m {2:D1}s {3:D1}ms",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);
            }
            else
            {
                timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds);
            }
            return timeString;

        }
    }
}
