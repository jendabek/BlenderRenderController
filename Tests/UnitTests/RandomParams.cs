using System;
using System.Linq;

namespace UnitTestProject1
{
    static class RandomParams
    {
        private static readonly Random getRandom = new Random();
        private static readonly object syncLock = new object();

        public static int GetNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getRandom.Next(min, max);
            }
        }
        public static int[] GetNumberPair(int min, int max, bool invert = false)
        {
            var r1 = GetNumber(min, max);
            var r2 = GetNumber(min, max);

            int[] rnd = { r1, r2 };

            return invert 
                    ? rnd.OrderByDescending(r => r).ToArray() 
                    : rnd.OrderBy(r => r).ToArray();
        }

    }
}
