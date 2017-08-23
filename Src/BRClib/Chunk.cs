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
        /// Calculates an even divided array of chunks, based on
        /// the provided divisor
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


            // if div is 1, return a single chunk
            if (div == 1)
                return new Chunk[]{ new Chunk(start, end) };

            int lenght = (int)Math.Ceiling((decimal)(end - start + 1) / div);
            List<Chunk> chunkList = new List<Chunk>();

            int cStart, cEnd;
            cStart = start;

            // makes chunks
            for (int i = 0; i != div; i++)
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
                    // the final chunk, the one that matches the project's end
                    var secondLast = chunkList.Last();

                    if (secondLast.End == end)
                        break;

                    var finalChunk = new Chunk(secondLast.End + 1, end);
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
        public static Chunk[] CalcChunksByLenght(int start, int end, int chunkLenght)
        {
            if (end <= start)
                throw new ArgumentException("Start frame cannot be equal or greater them the end frame",
                                            nameof(start));

            if (chunkLenght <= 0)
                throw new ArgumentException("Invalid chunk lenght", nameof(chunkLenght));


            decimal totalLen = end - start + 1;
            int div = (int)Math.Ceiling(totalLen / chunkLenght);

            List<Chunk> chunkList = new List<Chunk>();
            int cStart, cEnd;

            cStart = start;

            for (int i = 0; i < div; i++)
            {
                cEnd = cStart + chunkLenght;

                var chunk = new Chunk(cStart, cEnd);

                if ((chunk.End + 1 < end))
                {
                    chunkList.Add(chunk);
                    cStart = cEnd + 1;
                }
                else
                {
                    // the final chunk, the one that matches the project's end
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
