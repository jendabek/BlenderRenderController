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
            set
            {
                if (SetProperty(ref _start, value))
                {
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(TotalFrames));
                }
            }
        }

        [JsonProperty("end")]
        public int End
        {
            get => _end;
            set
            {
                if (SetProperty(ref _end, value))
                {
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(TotalFrames));
                }
            }
        }
        
        [JsonProperty("fps")]
        public double Fps
        {
            get => _fps;
            private set => SetProperty(ref _fps, value);
        }

        [JsonProperty("resolution")]
        public string Resolution
        {
            get => _res;
            private set => SetProperty(ref _res, value);
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
            private set => SetProperty(ref _activeScene, value);
        }

        // scene.render.image_settings.file_format
        [JsonProperty("imgFormat")]
        public string FileFormat { get; private set; }

        // scene.render.ffmpeg.format
        [JsonProperty("ffmpegFmt")]
        public string FFmpegVideoFormat { get; private set; }

        // scene.render.ffmpeg.audio_codec
        [JsonProperty("ffmpegAudio")]
        public string FFmpegAudioCodec { get; private set; }

        public TimeSpan? Duration
        {
            get
            {
                var duration = (End - Start + 1) / Fps;
                if (!double.IsNaN(duration) && !double.IsInfinity(duration))
                    return TimeSpan.FromSeconds(duration);
                else
                    return null;
            }
        }

        public int? TotalFrames
        {
            get
            {
                if (End <= Start)
                {
                    return null;
                }
                return End - Start + 1;
            }
        }

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

    /// <summary>
    /// Holds settings to the BRC render process
    /// </summary>
    public class ProjectSettings : BindingBase
    {
        private readonly ObservableCollection<Chunk> _chunkList;
        private string _blendPath;
        private int _chunkLen, _processCount;
        private BlendData _bData;

        public BlendData BlendData
        {
            get { return _bData; }
            set { SetProperty(ref _bData, value); }
        }

        public string BlendPath
        {
            get => _blendPath;
            set => SetProperty(ref _blendPath, value);
        }

        public ObservableCollection<Chunk> ChunkList => _chunkList;

        public int ChunkLenght
        {
            get => _chunkLen;
            set => SetProperty(ref _chunkLen, value);
        }

        public int ProcessesCount
        {
            get => _processCount;
            set => SetProperty(ref _processCount, value);
        }

        public string ChunkSubdirPath
        {
            get
            {
                if (BlendData == null || BlendData.OutputPath == null)
                    return null;

                return Path.Combine(BlendData.OutputPath, "chunks");
            }
        }


        public ProjectSettings() : base()
        {
            _chunkList = new ObservableCollection<Chunk>();
            ChunkLenght = 50;
        }


    }

    public enum BlenderRenderes
    {
        BLENDER_RENDER, CYCLES
    }

    [Flags]
    public enum AfterRenderAction
    {
        JOIN = 0x2,
        MIXDOWN = 0x1,
        NOTHING = 0x0
    }


}
