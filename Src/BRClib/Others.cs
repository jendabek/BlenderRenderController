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
                    var ext = Path.GetExtension(f).Substring(1);
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
        public static readonly string[] AllowedFileExts = { "avi", "mp4", "mov", "mkv", "mpg", "flv", "dv", "dvd", "ogv" };

        public static readonly string[] AllowedAudioFileExts = { "mp3", "ac3", "aac", "ogg", "flac", "wav" };

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

    /// <summary>
    /// Command line arguments for the various processes
    /// </summary>
    public static class CommandARGS
    {
        // 0=ChunkTxtPath, 1=MixdownInclude, 2=Optional duration, 3=Final file path + .EXT
        //const string _concatBase = "-f concat -safe 0 -i \"{0}\" {1} -c:v copy {2} \"{3}\" -y";


        public static string GetConcatenationArgs(string chunksTxt, TimeSpan? duration, string output, string mixdown = null)
        {
            if (duration == null) return GetConcatenationArgs(chunksTxt, output, mixdown);

            string args = "-f concat -safe 0 ";

            // chunkTxt
            args += "-i \"" + chunksTxt + "\" ";
            // mixdown audio
            if (!string.IsNullOrWhiteSpace(mixdown)) args += "-i \"" + mixdown + "\" -map 0:v -map 1:a ";
            // encoder
            args += "-c:v copy ";
            // duration
            args += "-t " + duration.Value.ToString(@"hh\:mm\:ss") + ' ';
            // output
            args += "\"" + output + "\" -y";

            return args;
        }

        public static string GetConcatenationArgs(string chunksTxt, string output, string mixdown = null)
        {
            string args = "-f concat -safe 0 ";

            // chunkTxt
            args += "-i \"" + chunksTxt + "\" ";
            // mixdown audio
            if (!string.IsNullOrWhiteSpace(mixdown)) args += "-i \"" + mixdown + "\" -map 0:v -map 1:a ";
            // encoder
            args += "-c:v copy ";
            // output
            args += "\"" + output + "\" -y";

            return args;
        }


        /*
        /// <summary>
        /// Concatenate command ARGS, join chunks with mixdown
        /// <para>0=ChunkTxtPath, 1=Mixdown audio, 2=Final file path + .EXT, </para>
        /// </summary>
        public const string ConcatenateMixdown = "-f concat -safe 0 -i \"{0}\" -i \"{1}\" -map 0:v -map 1:a -c:v copy \"{2}\" -y";

        /// <summary>
        /// Concatenate command ARGS, join chunks
        /// <para>0=ChunkTxtPath, 1=Final file path + .EXT</para>
        /// </summary>
        public const string ConcatenateOnly = "-f concat -safe 0 -i \"{0}\" -c:v copy \"{1}\" -y";
        */

        /// <summary>
        /// Get info command ARGS
        /// <para>0=Blend file, 1=get_project_info.py</para>
        /// </summary>
        public const string GetInfoComARGS = "-b \"{0}\" -P \"{1}\"";

        /// <summary>
        /// Mixdown command ARGS
        /// <para>0=Blend file, 1=start, 2=end, 3=mixdown_audio.py, 4=output</para>
        /// </summary>
        public const string MixdownComARGS = "-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- \"{4}\"";

        /// <summary>
        /// Render command ARGS
        /// <para>0=Blend file, 1=output, 2=Renderer, 3=Frame start, 4=Frame end</para>
        /// </summary>
        public const string RenderComARGS = "-b \"{0}\" -o \"{1}\" -E {2} -s {3} -e {4} -a";
    }
}
