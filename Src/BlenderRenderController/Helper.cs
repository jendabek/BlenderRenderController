using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlenderRenderController
{
    static class Helper
    {
        private static Logger logger = LogManager.GetLogger("Helper");

        static public void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }
        
        static public void ShowErrors(MessageBoxIcon icon, string fmt, params AppErrorCode[] errorCodes)
        {
            var errorText = "";

            foreach (var errorCode in errorCodes)
                errorText += SetErrorText(errorCode, fmt);

            MessageBox.Show(
                    errorText,
                    "",
                    MessageBoxButtons.OK,
                    icon);

            logger.Warn(errorText);
        }
        static public void ShowErrors(MessageBoxIcon icon, params AppErrorCode[] errorCodes)
        {
            var errorText = "";

            foreach (var errorCode in errorCodes)
                errorText += SetErrorText(errorCode);

            MessageBox.Show(
                    errorText,
                    "",
                    MessageBoxButtons.OK,
                    icon);

            logger.Warn(errorText);
        }
        static public void ShowErrors(MessageBoxIcon icon, AppErrorCode errorCode)
        {
            var errorText = SetErrorText(errorCode);

            MessageBox.Show(
                errorText,
                "",
                MessageBoxButtons.OK,
                icon);

            logger.Warn(errorText);
        }
        static public void ShowErrors(MessageBoxIcon icon, string fmt, AppErrorCode errorCode)
        {
            var errorText = SetErrorText(errorCode, fmt);

            MessageBox.Show(
                errorText,
                "",
                MessageBoxButtons.OK,
                icon);

            logger.Warn(errorText);
        }

        static private string SetErrorText(AppErrorCode code, string arg1 = "")
        {
            var sb = new StringBuilder();

            switch (code)
            {
                case AppErrorCode.BLENDER_PATH_NOT_SET:
                    sb.AppendLine("Please set correct path to Blender.");
                    break;
                case AppErrorCode.FFMPEG_PATH_NOT_SET:
                    sb.AppendLine("Please set correct path to FFmpeg.");
                    break;
                case AppErrorCode.BLEND_FILE_NOT_EXISTS:
                    sb.AppendLine("File does not exists anymore.");
                    sb.AppendLine("It was removed from the list of recent blends.");
                    break;
                case AppErrorCode.RENDER_FORMAT_IS_IMAGE:
                    sb.AppendLine("The render format is " + arg1 + " image.");
                    sb.AppendLine("You can render an image sequence with this tool but you will need to make a video with other SW.");
                    break;
                case AppErrorCode.BLEND_OUTPUT_INVALID:
                    sb.AppendLine("Unable to read output path, using project location.");
                    break;
                case AppErrorCode.UNKNOWN_OS:
                    sb.AppendLine("Could not identify operating system, BRC might not work properly.");
                    break;
            }

            return sb.ToString();
        }

        static public string FixPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                logger.Warn("Failed to fixPath");
                return null;
            }

            return path.Trim().TrimEnd('\\');
        }

        static public string ParseChunksFolder(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath))
                return string.Empty;

            return Path.Combine(outputPath, Constants.ChunksSubfolder);
        }
        static public string ParseChunksTxt(string outputPath)
        {
            if (string.IsNullOrEmpty(outputPath))
                return string.Empty;

            return Path.Combine(ParseChunksFolder(outputPath), Constants.ChunksTxtFileName);
        }

        static public string SecondsToString(double seconds, bool digital = false)
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

        static public IEnumerable<Control> FindControlsByTag(Control.ControlCollection controls, string key)
        {
            List<Control> controlsWithTags = new List<Control>();

            foreach (Control c in controls)
            {
                if (c.Tag != null)
                {
                    // splits tag content into string array
                    string[] tags = c.Tag.ToString().Split(';');

                    // if key maches, add to list
                    if (tags.Contains(key))
                        controlsWithTags.Add(c);
                }

                if (c.HasChildren)
                {
                    //Recursively check all children controls as well; ie groupboxes or tabpages
                    controlsWithTags.AddRange(FindControlsByTag(c.Controls, key));
                }
            }

            return controlsWithTags;
        }

    }
}
