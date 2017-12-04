using System;
using System.Collections.Generic;
using System.Linq;

namespace BRClib
{
    /// <summary>
    /// Represents a range of frames to be rendered
    /// </summary>
    public struct Chunk
    {
        /// <summary>
        /// <see cref="Chunk"/>'s start frame
        /// </summary>
        public int Start { get; set; }
        /// <summary>
        /// <see cref="Chunk"/>'s end frame
        /// </summary>
        public int End { get; set; }
        /// <summary>
        /// The <see cref="Chunk"/>'s frame length
        /// </summary>
        /// <remarks>
        /// A Chunk's Lenght is measured inclusively, 
        /// so the lenght of: {1-2400} is 2400, not 2399.
        /// </remarks>
        public int Length => End - Start + 1;
        /// <summary>
        /// Returns if current <see cref="Chunk"/> is valid
        /// </summary>
        public bool IsValid
        {
            get => Start < End && Length > 0;
        }


        /// <summary>
        /// Create a new chunk
        /// </summary>
        /// <param name="start">Chunk's start frame</param>
        /// <param name="end">Chunk's end frame</param>
        public Chunk(int start, int end)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            Start = start;
            End = end;

        }

        /// <summary>
        /// Calculates an even divided collection of chunks
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="div">Number of chunks desired</param>
        public static IEnumerable<Chunk> CalcChunks(int start, int end, int div)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            if (div == 1)
                // return a single chunk
                return new Chunk[]{ new Chunk(start, end) };

            decimal lenght = Math.Ceiling((end - start + 1) / (decimal)div);
            return GenChunks(start, end, (int)lenght);
        }
        /// <summary>
        /// Calculates an even divided collection of chunks, based on desired lenght
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="chunkLenght">Desired chunk lenght</param>
        public static IEnumerable<Chunk> CalcChunksByLength(int start, int end, int chunkLenght)
        {
            if (chunkLenght <= 1)
                throw new ArgumentException("Invalid chunk lenght", nameof(chunkLenght));

            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            return GenChunks(start, end, chunkLenght);
        }

        private static List<Chunk> GenChunks(int start, int end, int chunkLen)
        {
            int cStart = start,
                cEnd,
                cDiff = chunkLen - 1;

            int totalLen = end - start + 1;

            List<Chunk> chunkList = new List<Chunk>();

            while (chunkList.TotalLength() < totalLen)
            {
                cEnd = cStart + cDiff;

                if (cEnd + 1 < end)
                {
                    chunkList.Add(new Chunk(cStart, cEnd));
                    cStart = cEnd + 1;
                }
                else
                {
                    // this should be the last chunk
                    // to be added
                    chunkList.Add(new Chunk(cStart, end));
                }
            }

            return chunkList;
        }

        // For SOME REASON the code below causes outofmemory exceptions on my unit tests,
        // affects CalcChunksByLength, but not CalcChunks
        /*
        private static IEnumerable<Chunk> GenChunks(int start, int end, int chunkLen)
        {
            int cStart = start,
                cEnd = 0,
                cDiff = chunkLen - 1;

            while (cEnd < end)
            {
                cEnd = cStart + cDiff;

                if (cEnd + 1 < end)
                {
                    var chunk = new Chunk(cStart, cEnd);
                    cStart = cEnd + 1;
                    yield return chunk;
                }
                else
                {
                    yield return new Chunk(cStart, end);
                }
            }
        }
        */

        public override string ToString()
        {
            return $"{Start}-{End}";
        }

        #region Equallity
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var chunk = (Chunk)obj;

            return Equals(chunk);
        }

        public bool Equals(Chunk c)
        {
            return Start == c.Start
                && End == c.End;
        }

        public static bool operator ==(Chunk c1, Chunk c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(Chunk c1, Chunk c2)
        {
            return !(c1.Equals(c2));
        }


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
        #endregion

    }

}
