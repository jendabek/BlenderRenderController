using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace BRClib
{
    /// <summary>
    /// Represents the settings from a Blender project file
    /// </summary>
    public class BlendData : BindingBase
    {
        private int _start, _end;
        private double _fps;
        private string _outPath, _projName, 
            _activeScene, _res;


        [JsonProperty("start")]
        public int Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        [JsonProperty("end")]
        public int End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        [JsonProperty("fps")]
        public double Fps
        {
            get => _fps;
            set => SetProperty(ref _fps, value);
        }

        [JsonProperty("resolution")]
        public string Resolution
        {
            get => _res;
            set => SetProperty(ref _res, value);
        }

        [JsonProperty("outputPath")]
        public string OutputPath
        {
            get => _outPath;
            set => SetProperty(ref _outPath, value);
        }

        [JsonProperty("projectName")]
        public string ProjectName
        {
            get => _projName;
            set => SetProperty(ref _projName, value);
        }

        [JsonProperty("sceneActive")]
        public string ActiveScene
        {
            get => _activeScene;
            set => SetProperty(ref _activeScene, value);
        }

        // scene.render.image_settings.file_format
        [JsonProperty("imgFormat")]
        public string FileFormat { get; set; }

        // scene.render.ffmpeg.format
        [JsonProperty("ffmpegFmt")]
        public string FFmpegVideoFormat { get; set; }

        // scene.render.ffmpeg.audio_codec
        [JsonProperty("ffmpegAudio")]
        public string FFmpegAudioCodec { get; set; }

      

        //public string AudioFileFormat
        //{
        //    get
        //    {
        //        if (FFmpegAudioCodec == null)
        //            return null;

        //        switch (FFmpegAudioCodec)
        //        {
        //            case "PCM":
        //                return "wav";
        //            case "VORBIS":
        //                return "ogg";
        //            case "NONE":
        //                return "ac3";
        //            default:
        //                return FFmpegAudioCodec.ToLower();
        //        }
        //    }
        //}
    }


    public class RenderProgressInfo
    {
        public int FramesRendered { get; }
        public int PartsCompleted { get; }

        public RenderProgressInfo(int framesRendered, int partsCompleted)
        {
            FramesRendered = framesRendered;
            PartsCompleted = partsCompleted;
        }
    }


    public enum Renderer
    {
        BLENDER_RENDER, CYCLES
    }

    [Flags]
    public enum AfterRenderAction
    {
        JOIN = 0x0002,
        MIXDOWN = 0x0001,
        NOTHING = 0x0000
    }


}
