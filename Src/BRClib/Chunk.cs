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
        /// The <see cref="Chunk"/>'s length in N# of frames
        /// </summary>
        public int Length => End - Start;
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
        /// Calculates an even divided array of chunks
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="div">Number of chunks desired</param>
        /// <returns></returns>
        public static Chunk[] CalcChunks(int start, int end, int div)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            if (div == 0)
                throw new ArgumentException("Divider cannot be 0", nameof(div));

            if (div == 1)
                // return a single chunk
                return new Chunk[]{ new Chunk(start, end) };

            return MakeChunks(start, end, div);
        }
        /// <summary>
        /// Calculates an even divided array of chunks, based on desired lenght
        /// </summary>
        /// <param name="start">Project's start frame</param>
        /// <param name="end">Project's end frame</param>
        /// <param name="chunkLenght">Desired chunk lenght</param>
        /// <returns></returns>
        public static Chunk[] CalcChunksByLenght(int start, int end, int chunkLenght)
        {
            if (chunkLenght <= 0)
                throw new ArgumentException("Invalid chunk lenght", nameof(chunkLenght));

            int div = (int)Math.Ceiling((decimal)(end - start + 1) / chunkLenght);

            return CalcChunks(start, end, div);
        }

        private static Chunk[] MakeChunks(int start, int end, int div)
        {
            int cStart = start;
            int cEnd;
            decimal totalLen = end - start + 1;
            var lenght = (int)Math.Ceiling(totalLen / div);
            List<Chunk> chunkList = new List<Chunk>(div);

            for (int i = 0; i < div; i++)
            {
                cEnd = cStart + lenght;

                var chunk = new Chunk(cStart, cEnd);

                if ((chunk.End + 1 < end))
                {
                    chunkList.Add(chunk);
                    cStart = cEnd + 1;
                }
                else
                {
                    // decide final chunk, the one that matches the project's end
                    var secondLast = chunkList.Last();

                    if (secondLast.End == end)
                        break;

                    var finalChunk = new Chunk(secondLast.End + 1, end);
                    chunkList.Add(finalChunk);
                }
            }

            return chunkList.ToArray();
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }

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
    }

}
