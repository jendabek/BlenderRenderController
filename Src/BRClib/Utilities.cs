using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            return JsonConvert.DeserializeObject<BlendData>(json);
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
            return ParsePyOutput(outputString.Split('\n'));
        }

        /// <summary>
        /// Analizes the files in the specified folder and returns
        /// a list of valid chunks, ordered by frame-range
        /// </summary>
        /// <param name="chunkFolderPath"></param>
        /// <returns></returns>
        public static IList<string> GetChunkFiles(string chunkFolderPath)
        {
            var dirFiles = Directory.EnumerateFiles(chunkFolderPath, "*.*", 
                                            SearchOption.TopDirectoryOnly);
            return GetChunkFiles(dirFiles);
        }

        /// <summary>
        /// Analizes the files and returns a list of valid chunks, 
        /// ordered by frame-range
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static IList<string> GetChunkFiles(IEnumerable<string> files)
        {
            string[] exts = RenderFormats.VideoFileExts;

            var fileList = files
                .Where(f => 
                {
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

}
