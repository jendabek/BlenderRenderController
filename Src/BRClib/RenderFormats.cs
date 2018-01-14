using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BRClib
{
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

        public static readonly string[] VideoFormats =
            { "AVI_JPEG", "AVI_RAW", "H264", "FFMPEG", "THEORA", "XVID" };


        /// <summary>
        /// Allowed video file extentions
        /// </summary>
        public static readonly string[] VideoFileExts = 
            { ".avi", ".mp4", ".mov", ".mkv", ".mpg", ".flv", ".dv", ".dvd", ".ogv" };

        public static readonly string[] AudioFileExts = 
            { ".mp3", ".ac3", ".aac", ".ogg", ".flac", ".wav" };

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
