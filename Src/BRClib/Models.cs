using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.ObjectModel;

namespace BRClib
{
    public class BlendData : BindingBase
    {
        private int _start, _end, _sceneNum;
        private double _fps;
        private string _res, _outPath, _projName, 
            _activeScene, _renderFmt;

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
            get => _sceneNum;
            set => SetProperty(ref _sceneNum, value);
        }
        [JsonProperty("fps")]
        public double Fps
        {
            get => _fps;
            set
            {
                if(SetProperty(ref _fps, value))
                {
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(TotalFrames));
                }
            }
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

    public class ProjectSettings : BindingBase
    {
        private readonly ObservableCollection<Chunk> _chunkList;
        private string _blendPath, _chunkFolder;
        private BlendData _bData;
        private int _chunkLen, _processCount;

        public string BlendPath
        {
            get => _blendPath;
            set => SetProperty(ref _blendPath, value);
        }

        public BlendData BlendData
        {
            get => _bData;
            set
            {
                SetProperty(ref _bData, value);
            }
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


        //public BlenderRenderes Renderer { get; set; }

        public ProjectSettings()
        {
            _chunkList = new ObservableCollection<Chunk>();
            //_chunkList.CollectionChanged += ChunkList_CollectionChanged;
            BlendData = new BlendData();
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
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
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
        /// <param name="div">Number of chunks desired</param>
        /// <returns></returns>
        public static Chunk[] CalcChunks(decimal start, decimal end, int div)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            if (div == 0)
                throw new ArgumentException("Divider cannot be 0", nameof(div));

            // if div is 1, return a single chunk
            if (div == 1)
                return new Chunk[]{ new Chunk(start, end) };

            var lenght = Math.Ceiling((end - start + 1) / div);
            List<Chunk> chunkList = new List<Chunk>();

            decimal cStart, cEnd;

            // makes chunks
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
                    // the final chunk, the one that matches the project's end
                    var last = chunkList.Last();
                    var finalChunk = new Chunk(last.End + 1, end);
                    chunkList.Add(finalChunk);
                }
            }

            return chunkList.ToArray();
        }
        /// <summary>
        /// Calculates an even divided array of chunks, based on desired lenght
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="chunkLenght">Desired chunk lenght</param>
        /// <returns></returns>
        public static Chunk[] CalcChunksByLenght(decimal start, decimal end, int chunkLenght)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            var totalLenght = end - start + 1;

            if (chunkLenght <= 0)
                throw new ArgumentException("Invalid chunk lenght", nameof(chunkLenght));

            List<Chunk> chunkList = new List<Chunk>();
            decimal cStart, cEnd, totalChunksLen = 0;

            while (totalChunksLen <= totalLenght)
            {
                cStart = start;
                cEnd = start + chunkLenght;

                var chunk = new Chunk(cStart, cEnd);

                if ((chunk.End + 1 < end))
                {
                    chunkList.Add(chunk);
                    start = cEnd + 1;
                }
                else
                {
                    var last = chunkList.LastOrDefault();
                    cStart = last.End + 1;
                    if (cStart < end)
                    {
                        var finalChunk = new Chunk(last.End + 1, end);
                        chunkList.Add(finalChunk);
                    }
                }

                totalChunksLen += chunk.Length;
            }

            return chunkList.ToArray();
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var chunk = (Chunk)obj;

            return Start == chunk.Start
                && End == chunk.End;
        }
        public bool Equals(Chunk c)
        {
            return Start == c.Start
                && End == c.End;
        }
        // override object.GetHashCode
        public override int GetHashCode()
        {
            const int HashBase = 233;
            const int HashMulti = 13;

            unchecked
            {
                int hash = HashBase;
                hash = (hash * HashMulti) ^ Start.GetHashCode();
                hash = (hash * HashMulti) ^ End.GetHashCode();

                return hash;
            }
        }
    }

}
