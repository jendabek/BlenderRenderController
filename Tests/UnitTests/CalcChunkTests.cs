using System;
using System.Linq;
using BRClib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UnitTestProject1
{
    [TestClass]
    public class CalcChunkTests
    {
        [TestMethod]
        public void CalcChunks_random_genarations()
        {
            for (int i = 0; i < 10; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, 99999);

                var cores = RandomParams.GetNumber(1, 16);
                var start = randPair[0];
                var end = randPair[1];

                var totalLen = end - start + 1;
                var minLen = totalLen * 0.05;
                var maxLen = totalLen * 0.4;

                var calcResult = Chunk.CalcChunks(start, end, cores);
                if (calcResult.First().Length > maxLen || calcResult.First().Length < minLen)
                {
                    // ignore unrealistic lenghts
                    continue;
                }

                Console.WriteLine("Start: {0}, End: {1}, Total lenght: {2}, CoreCount: {3}", start, end, totalLen, cores);
                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "   Lenght: " + res.Length);
                }

                Console.WriteLine();
                Assert.IsFalse(calcResult.Last().End != end, "Last calcResult.End was diferent then param end");
                Assert.IsTrue(calcResult.Length == cores, "Number of chunks does not match core count");

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalcChunks_start_cannot_be_greater_then_end()
        {
            int[] rnd = RandomParams.GetNumberPair(1, 9999, invert: true);
            int cores = RandomParams.GetNumber(1, 17);

            int start = rnd[0];
            int end = rnd[1];

            Console.WriteLine("Start: {0}, End: {1}", start, end);

            var calcResult = Chunk.CalcChunks(start, end, 8);

        }

        [TestMethod]
        public void CalcChunks_custom_length()
        {

            for (int i = 0; i < 10; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, 99999);

                int start = randPair[0];
                int end = randPair[1];
                int totalLen = end - start + 1;

                // range of random lengths:
                var minLen = totalLen * 0.05;
                var maxLen = totalLen * 0.4;

                int len = RandomParams.GetNumber((int)minLen, (int)maxLen);

                Console.WriteLine("Start: {0}, End: {1}, Total legth: {2}", start, end, totalLen);

                var calcResult = Chunk.CalcChunksByLenght(start, end, len);
                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "   Lenght: " + res.Length);
                }
                Console.WriteLine();
                Console.WriteLine("Number of chunks: " + calcResult.Length);
                Assert.IsFalse(calcResult.Last().End != end, "Last calcResult.End was different then param end");
            }

        }

        [TestMethod]
        public void Chunks_TotalLenght_is_accuerate()
        {
            var start = 1;
            var end = 24000;
            var totalLen = end - start + 1;
            int cores = RandomParams.GetNumber(1, 17);

            var chunks = Chunk.CalcChunks(start, end, cores);

            Console.WriteLine("Total lenght = " + totalLen);
            Console.WriteLine("Chunks total length = " + chunks.TotalLength());

            Assert.IsTrue(totalLen == chunks.TotalLength(), "Lenghts are not equal");
        }
    }

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
