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
        /// <param name="output">Standard output lines</param>
        /// <returns>A <see cref="BlendData"/> object</returns>
        /// <remarks>
        /// When executing get_project_info script, Blender may also print errors
        /// alongside the project info (a commun case if there're custom plugins installed) 
        /// this method will filter out those errors.
        /// </remarks>
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
        /// Parses the output of get_project_info
        /// </summary>
        /// <param name="outputString">Standard output</param>
        /// <returns>A <see cref="BlendData"/> object</returns>
        /// <remarks>
        /// When executing get_project_info script, Blender may also print errors
        /// alongside the Json containig the project info (a commun case if there're
        /// custom plugins installed) this method will filter out those errors.
        /// </remarks>
        public static BlendData ParsePyOutput(string outputString)
        {
            string[] outLineSplit = outputString.Split('\n');
            return ParsePyOutput(outLineSplit);
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
            string[] exts = RenderFormats.AllowedFileExts;

            var fileList = files
                .Where(f => 
                {
                    // skip '.' in ext
                    var ext = Path.GetExtension(f);
                    var split = f.Split('-');

                    // format: FILENAME-fStart-fEnd.ext
                    if (split.Length > 2 && exts.Contains(ext))
                    {
                        // only add files with frame range
                        string numStr = split[split.Length - 2];
                        return int.TryParse(numStr, out int x);
                    }

                    return false;
                })
                .OrderBy(s =>
                {
                    //sort files in list by starting frame
                    var split = s.Split('-');
                    string numStr = split[split.Length - 2];
                    return int.Parse(numStr);
                });
            
            return fileList.ToList();
        }

    }

    /// <summary>
    /// Reference info of possible Blender render formats
    /// </summary>
    public static class RenderFormats
    {
        /// <summary>
        /// Image render formats
        /// </summary>
        public static readonly string[] IMAGES =
            { "PNG", "BMP", "IRIS", "JPEG", "JPEG2000", "TARGA", "TARGA_RAW",
            "CINEON", "DPX", "OPEN_EXR_MULTILAYER", "OPEN_EXR", "HDR", "TIFF" };

        //public static readonly string[] VideoFormats =
        //    { "AVI_JPEG", "AVI_RAW", "H264", "FFMPEG", "THEORA", "XVID" };


        /// <summary>
        /// Allowed video file extentions
        /// </summary>
        public static readonly string[] AllowedFileExts = { ".avi", ".mp4", ".mov", ".mkv", ".mpg", ".flv", ".dv", ".dvd", ".ogv" };

        public static readonly string[] AllowedAudioFileExts = { ".mp3", ".ac3", ".aac", ".ogg", ".flac", ".wav" };

        // TODO maybe: Make a list that relates format property to output file format
        //public static readonly Dictionary<string, string> ExtForEncoding = 
        //    new Dictionary<string, string>
        //    {
        //        { "AVI", "avi" }, {"XVID", "avi"}, {"H264", "avi"},
        //        { "MPEG4", "mp4" }, { "MPEG1", "mpg" }, {"MPEG2", "dvd"},
        //        {"QUICKTIME", "mov"}, {"DV", "dv"}, {"OGG", "ogv"},
        //        {"MKV", "mkv"}, {"FLASH", "flv"}
        //    };

    }

}
