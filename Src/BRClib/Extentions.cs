using System.Collections.Generic;

namespace BRClib
{
    public static class Extentions
    {
        public static int TotalLength(this IEnumerable<Chunk> chunks)
        {
            int len = 0;

            foreach (var chunk in chunks)
            {
                len += (int)chunk.Length + 1;
            }

            return len;
        }

    }
}
