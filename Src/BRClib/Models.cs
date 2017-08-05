using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BRClib
{
    public class BlendData : BindingBase
    {
        private int _start, _end, _sceneNum;
        private double _fps, _fpsBase;
        private string _res, _outPath, _projName, 
            _activeScene, _renderFmt;

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
        [JsonProperty("scenesNum")]
        public int NumberOfScenes
        {
            get => _sceneNum;
            set => SetProperty(ref _sceneNum, value);
        }
        [JsonProperty("fps")]
        public double FpsSource
        {
            get => _fps;
            set => SetProperty(ref _fps, value);
        }
        [JsonProperty("fpsBase")]
        public double FpsBase
        {
            get => _fpsBase;
            set => SetProperty(ref _fpsBase, value);
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
        [JsonProperty("renderFormat")]
        public string RenderFormat
        {
            get => _renderFmt;
            set => SetProperty(ref _renderFmt, value);
        }

    }

    public class ProjectData : BlendData
    {
        private readonly List<Chunk> _chunkList;
        private string _blendPath;

        public List<Chunk> ChunkList { get => _chunkList; }

        public string BlendPath
        {
            get => _blendPath;
            set => SetProperty(ref _blendPath, value);
        }

        public TimeSpan Duration
        {
            get
            {
                var durationSeconds = TotalFrames / FpsSource;
                return TimeSpan.FromSeconds(durationSeconds);
            }
        }

        public int TotalFrames
        {
            get
            {
                return End - Start + 1;
            }
        }

        public double Fps
        {
            get => FpsSource / FpsBase;
        }

        public ProjectData()
        {
            _chunkList = new List<Chunk>();
        }
    }

    /// <summary>
    /// Represents a range of frames to be rendered
    /// </summary>
    public struct Chunk
    {
        /// <summary>
        /// <see cref="Chunk"/>'s start frame
        /// </summary>
        public decimal Start { get; set; }
        /// <summary>
        /// <see cref="Chunk"/>'s end frame
        /// </summary>
        public decimal End { get; set; }
        /// <summary>
        /// The <see cref="Chunk"/>'s length
        /// </summary>
        public decimal Length
        {
            get => End - Start;
        }

        public Chunk(decimal start, decimal end)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them end frame",
                                            nameof(start));

            Start = start;
            End = end;

        }

        /// <summary>
        /// Calculates an even divided array of chunks, based on
        /// the provided divisor
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="div"></param>
        /// <returns></returns>
        public static Chunk[] CalcChunks(decimal start, decimal end, int div)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them end frame",
                                            nameof(start));

            if (div == 0)
                throw new ArgumentException("Divider cannot be 0", nameof(div));

            // if div is 1, return a single chunk
            if (div == 1)
                return new Chunk[]{ new Chunk(start, end) };

            var lenght = Math.Ceiling((end - start + 1) / div);
            List<Chunk> chunkList = new List<Chunk>();

            decimal cStart, cEnd;

            // makes even chunks
            for (int i = 0; i != div; i++)
            {
                cStart = start;
                cEnd = start + lenght;

                var chunk = new Chunk(cStart, cEnd);

                if ((chunk.End + 1 < end))
                {
                    chunkList.Add(chunk);
                    start = cEnd + 1;
                }
                else
                {
                    var last = chunkList.Last();
                    var finalChunk = new Chunk(last.End + 1, end);
                    chunkList.Add(finalChunk);
                }
            }

            return chunkList.ToArray();
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }

    public class BindingBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string pName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(pName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName]string pName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pName));
        }
    }

}
