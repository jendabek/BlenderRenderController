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
        private int _start, _end, _scenesNum;
        private double _fps;
        private string _outPath, _projName, 
            _activeScene, _fileFmt, _res;



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
        
        [JsonProperty("scenesNum")]
        public int NumberOfScenes
        {
            get => _scenesNum;
            private set => SetProperty(ref _scenesNum, value);
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
        public string FileFormat
        {
            get => _fileFmt;
            private set => SetProperty(ref _fileFmt, value);
        }


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


    }

    /// <summary>
    /// Holds settings to the BRC render process
    /// </summary>
    public class ProjectSettings : BindingBase
    {
        private readonly ObservableCollection<Chunk> _chunkList;
        private string _blendPath;
        private int _chunkLen, _processCount;

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

        public ProjectSettings() : base()
        {
            _chunkList = new ObservableCollection<Chunk>();
            ChunkLenght = 1;
        }
    }

    public enum BlenderRenderes
    {
        BLENDER_RENDER, CYCLES
    }

    public enum AfterRenderAction
    {
        JOIN_MIXDOWN = 2, JOIN = 1, NOTHING = 0
    }

}
