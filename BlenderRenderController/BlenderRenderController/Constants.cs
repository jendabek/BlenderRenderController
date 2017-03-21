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
        public const string BLEND_FILE_NOT_EXISTS = "blendFileNotExists";
        public const string RENDER_FORMAT_IS_IMAGE = "formatIsImage";
        public const string BLEND_OUTPUT_INVALID = "blendOutputInvalid";
        public const string UNKNOWN_OS = "unknownOS";
    }
    static class AppStrings
    {
        public const string RENDERER_BLENDER = "BLENDER_RENDER";
        public const string RENDERER_CYCLES = "CYCLES";
        public const string AFTER_RENDER_JOIN_MIXDOWN = "afterRenderJoinMixdown";
        public const string AFTER_RENDER_JOIN = "afterRenderJoin";
        public const string AFTER_RENDER_NOTHING = "afterRenderDoNothing";
        public const string FFMPEG_DOWNLOAD_URL = "https:\\//ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-3.2.2-win64-static.zip";
    }
    static class RenderFormats
    {
        public static readonly string[] IMAGES = { "PNG", "BMP", "IRIS", "JPEG", "JPEG2000", "TARGA", "TARGA_RAW", "CINEON", "DPX", "OPEN_EXR_MULTILAYER", "OPEN_EXR", "HDR", "TIFF" };
        public static readonly string[] VIDEOS = { "AVI_JPEG", "AVI_RAW", "H264", "FFMPEG", "THEORA", "XVID" };
    }
    static class LogFormats
    {
        public const string LOG_FILE_NAME = "log.txt";

    }

}
