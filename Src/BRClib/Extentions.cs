using System.Collections.Generic;

namespace BRClib
{
    public static class Extentions
    {
        /// <summary>
        /// Gets the combined lenght of all <see cref="Chunk"/>s in a 
        /// collection
        /// </summary>
        /// <param name="chunks"></param>
        /// <returns></returns>
        public static int TotalLength(this IEnumerable<Chunk> chunks)
        {
            int len = 0;

            foreach (var chunk in chunks)
            {
                len += chunk.Length + 1;
            }

            return len;
        }

    }
}
