using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BRClib
{
    /// <summary>
    /// Contains usefull methods
    /// </summary>
    public class Utilities
    {
        /// <summary>
        /// Parses the output of get_project_info
        /// </summary>
        /// <param name="output"></param>
        /// <returns>A <see cref="BlendData"/> object</returns>
        public static BlendData ParsePyOutput(IEnumerable<string> output)
        {
            StringBuilder jsonInfo = new StringBuilder();
            bool jsonStarted = false;
            int curlyStack = 0;

            // Filter out errors and create data
            foreach (string line in output)
            {
                if (line.Contains("{"))
                {
                    jsonStarted = true;
                    curlyStack++;
                }
                if (jsonStarted)
                {
                    if (!line.ToLower().Contains("blender quit") && curlyStack > 0)
                    {
                        jsonInfo.AppendLine(line);
                    }
                    if (line.Contains("}"))
                    {
                        curlyStack--;
                        if (curlyStack == 0)
                        {
                            jsonStarted = false;
                        }
                    }
                }
            }

            var json = jsonInfo.ToString();
            var bData = JsonConvert.DeserializeObject<BlendData>(json);

            return bData;
        }

        /// <summary>
        /// Analizes the files in the specified folder and returns
        /// a list of valid chunks, ordered by frame-range
        /// </summary>
        /// <param name="chunkFolderPath"></param>
        /// <returns></returns>
        public static IList<string> GetChunkFiles(string chunkFolderPath)
        {
            var dirFiles = Directory.GetFiles(chunkFolderPath, "*.*", SearchOption.TopDirectoryOnly);

            return GetChunkFiles(dirFiles);
        }

        /// <summary>
        /// Analizes the files and returns a list of valid chunks, 
        /// ordered by frame-range
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IList<string> GetChunkFiles(params string[] files)
        {
            string[] exts = RenderFormats.AllowedFormats;

            var fileList = files
                .Where(f => 
                {
                    try
                    {
                        // skip '.' in ext
                        var ext = Path.GetExtension(f).Substring(1);
                        if (exts.Contains(ext))
                        {
                            // only add files with frame range
                            string numStr = f.Split('-')[f.Split('-').Length - 2];
                            return int.TryParse(numStr, out int x);
                        }
                        else
                            return false;
                    }
                    catch (IndexOutOfRangeException)
                    { return false; }
                })
                .OrderBy(s =>
                {
                    //sort files in list by starting frame
                    string numStr = s.Split('-')[s.Split('-').Length - 2];
                    return int.Parse(numStr);
                });
            
            return fileList.ToList();
        }

    }

    public static class RenderFormats
    {
        public static readonly string[] IMAGES =
            { "PNG", "BMP", "IRIS", "JPEG", "JPEG2000", "TARGA", "TARGA_RAW",
            "CINEON", "DPX", "OPEN_EXR_MULTILAYER", "OPEN_EXR", "HDR", "TIFF" };

        public static readonly string[] VIDEOS =
            { "AVI_JPEG", "AVI_RAW", "H264", "FFMPEG", "THEORA", "XVID" };

        public static readonly string[] AllowedFormats = { "avi", "mp4", "mov", "mkv", "mpg", "flv" };

    }

    public static class Extentions
    {
        /// <summary>
        /// Starts the process asynchronously.
        /// </summary>
        /// <param name="process">The process to wait for cancellation.</param>
        /// <param name="cancellationToken">A cancellation token. If invoked, the task will return 
        /// immediately as canceled.</param>
        /// <returns>A Task representing waiting for the process to end.</returns>
        public static Task StartAsync(this Process process,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);

            process.Start();

            return tcs.Task;
        }
    }
}
