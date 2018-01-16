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

            return Equals((Chunk)obj);
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



        /// <summary>
        /// Calculates an even divided collection of chunks
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="chunkNum">Number of chunks desired</param>
        public static IEnumerable<Chunk> CalcChunks(int start, int end, int chunkNum)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            if (chunkNum <= 0)
                throw new ArgumentException("Invalid N# of Chunks", nameof(chunkNum));

            if (chunkNum == 1)
            {
                // return a single chunk
                return Enumerable.Repeat(new Chunk(start, end), 1);
            }

            var lenght = Math.Ceiling((end - start + 1) / (double)chunkNum);

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

        private static IEnumerable<Chunk> GenChunks(int start, int end, int chunkLen)
        {
            int cStart = start;
            int cEnd = 0;
            int cDiff = chunkLen - 1;

            while (true)
            {
                cEnd = cStart + cDiff;

                if (cEnd + 1 < end)
                {
                    yield return new Chunk(cStart, cEnd);
                    cStart = cEnd + 1;
                }
                else
                {
                    // last chunk, matches the 'end' param
                    yield return new Chunk(cStart, end);
                    break;
                }
            }
        }

    }

}
