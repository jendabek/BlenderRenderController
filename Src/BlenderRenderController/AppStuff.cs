using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController
{
    class AppInfo
    {
        public const string PyGetInfo = "get_project_info.py";
        public const string PyMixdown = "mixdown_audio.py";
        public const string ChunksSubfolder = "chunks";
        public const string ScriptsSubfolder = "Scripts";
        public const string APP_TITLE = "Blender Render Controller";
        public const string ChunksTxtFileName = "chunklist.txt";
    }

    class CommandARGS
    {
        /// <summary>
        /// 0=chucksTxt, 1=audioFile, 2=Project, 3=videoExt
        /// </summary>
        public const string ConcatenateComARGS = 
            "-f concat -safe 0 -i \"{0}\" -i {1} -map0:v -map1:a -c:v copy \"{2}.{3}\" -y";

        /// <summary>
        /// 0=Blend file, 1=get_project_info.py
        /// </summary>
        public const string GetInfoComARGS = "-b \"{0}\" -P \"{1}\"";

        /// <summary>
        /// 0=Blend file, 1=start, 2=end, 3=mixdown_audio.py, 4=output
        /// </summary>
        public const string MixdownComARGS = "-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- \"{4}\"";

        /// <summary>
        /// 0=Blend file, 1=output, 2=Renderer, 3=Frame start, 4=Frame end
        /// </summary>
        public const string RenderComARGS = "-b \"{0}\" -o \"{1}\" -E {2}-# -s {3} -e {4} -a";
    }

    enum AppStates
    {
        RENDERING_ALL, READY_FOR_RENDER, AFTER_START, NOT_CONFIGURED
    }
    enum AppErrorCode
    {
        BLENDER_PATH_NOT_SET, FFMPEG_PATH_NOT_SET, BLEND_FILE_NOT_EXISTS,
        RENDER_FORMAT_IS_IMAGE, BLEND_OUTPUT_INVALID, UNKNOWN_OS
    }

    static class RenderFormats
    {
        public static readonly string[] IMAGES = 
            { "PNG", "BMP", "IRIS", "JPEG", "JPEG2000", "TARGA", "TARGA_RAW",
            "CINEON", "DPX", "OPEN_EXR_MULTILAYER", "OPEN_EXR", "HDR", "TIFF" };

        public static readonly string[] VIDEOS = 
            { "AVI_JPEG", "AVI_RAW", "H264", "FFMPEG", "THEORA", "XVID" };


        public enum Videos
        {
            AVI_JPEG, AVI_RAW, H264, FFMPEG, THEORA, XVID
        }

    }

    public enum BlenderRenderes
    {
        BLENDER_RENDER, CYCLES
    }

}
