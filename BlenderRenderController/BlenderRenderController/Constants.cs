using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController
{
    static class AppStates
    {
        public const string RENDERING_ALL = "state_rendering_all";
        public const string RENDERING_CHUNK_ONLY = "state_rendering_chunk_only";
        public const string READY_FOR_RENDER = "state_ready";
        public const string AFTER_START = "state_after_start";
        public const string NOT_CONFIGURED = "state_not_configured";
    }
    static class AppErrorCodes
    {
        public const string BLENDER_PATH_NOT_SET = "blenderPathNotSet";
        public const string FFMPEG_PATH_NOT_SET = "ffmpegPathNotSet";
    }
    static class AppStrings
    {

        public const string RENDERER_BLENDER = "BLENDER_RENDER";
        public const string RENDERER_CYCLES = "CYCLES";
        public const string FFMPEG_DOWNLOAD_URL = "https:\\//ffmpeg.zeranoe.com/builds/";
        
    }
}
