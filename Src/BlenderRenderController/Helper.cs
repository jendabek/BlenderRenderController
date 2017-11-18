using BlenderRenderController.Properties;
using BRClib;
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

        static public bool ClearOutputFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                DirectoryInfo[] subDirs = dir.GetDirectories();

                // clear files in the output
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                // clear files in the 'chunks' subdir
                var chunkSDir = subDirs.FirstOrDefault(di => di.Name == Constants.ChunksSubfolder);
                if (chunkSDir != null)
                {
                    Directory.Delete(chunkSDir.FullName, true);
                }

                return true;
            }
            catch (IOException)
            {
                string msg = "Can't clear output folder, there're files are in use";
                logger.Error(msg);
                MessageBox.Show(msg);
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Trace(ex.StackTrace);
                MessageBox.Show("An unexpected error ocurred, sorry.\n\n" + ex.Message);
                return false;
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
            ShowErrors(icon, string.Empty, errorCodes);
        }
        static public void ShowErrors(MessageBoxIcon icon, AppErrorCode errorCode)
        {
            ShowErrors(icon, string.Empty, errorCode);
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

        static public void ReportProcFail(string name, int exitCode, string logFilePath, string stdErr = null)
        {
            var content = string.Format(Resources.BadProcResult_Report, exitCode, stdErr);
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.Write("\n\n");
                sw.Write(name + ' ');
                sw.WriteLine(content);
            }


        }

        //static public string SecondsToString(double seconds, bool digital = false)
        //{
        //    TimeSpan t = TimeSpan.FromSeconds(seconds);
        //    string timeString;
        //    if (!digital) {
        //        timeString = string.Format("{0:D1}h {1:D1}m {2:D1}s {3:D1}ms",
        //                        t.Hours,
        //                        t.Minutes,
        //                        t.Seconds,
        //                        t.Milliseconds);
        //    }
        //    else
        //    {
        //        timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
        //                        t.Hours,
        //                        t.Minutes,
        //                        t.Seconds);
        //    }
        //    return timeString;

        //}

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

        //public static string GetChunksFolder(string blendOutput)
        //{
        //    return Path.Combine(blendOutput, "chunks");
        //}
        //public static string GetChunksFolder(BlendData blendData)
        //{
        //    return GetChunksFolder(blendData.OutputPath);
        //}
        //public static string GetChunkTxt(string blendOutput)
        //{
        //    return Path.Combine(GetChunksFolder(blendOutput), "chunklist.txt");
        //}
        //public static string GetChunkTxt(BlendData blendData)
        //{
        //    return GetChunkTxt(blendData.OutputPath);
        //}

    }
}
