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
                int[] randPair = MakeRandomParams.GetRandomNumberPair(1, 99999);
                //Console.WriteLine("randPair: r1={0}, r2={1}", randPair[0], randPair[1]);

                var cores = MakeRandomParams.GetRandomNumber(1, 16);
                var start = randPair[0];
                var end = randPair[1];
                Console.WriteLine("Start: {0}, End: {1}, Total lenght: {2}, CoreCount: {3}", start, end, end - start, cores);

                var calcResult = Chunk.CalcChunks(start, end, cores);
                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "   Lenght: " + res.Length);
                }

                Console.WriteLine();
                Assert.IsFalse(calcResult.Last().End != end, "Last calcResult.End was diferent then param end");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalcChunks_start_cannot_be_greater_then_end()
        {
            int[] rnd = MakeRandomParams.GetRandomNumberPair(1, 9999, invert: true);
            int cores = MakeRandomParams.GetRandomNumber(1, 17);

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
                int[] randPair = MakeRandomParams.GetRandomNumberPair(1, 99999);

                int start = randPair[0];
                int end = randPair[1];
                int len = 3500;

                Console.WriteLine("Start: {0}, End: {1}, Legth: {2}", start, end, end - start + 1);

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
    }

    static class MakeRandomParams
    {
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }
        public static int[] GetRandomNumberPair(int min, int max, bool invert = false)
        {
            var r1 = GetRandomNumber(min, max);
            var r2 = GetRandomNumber(min, max);

            int[] rnd = { r1, r2 };

            return invert 
                    ? rnd.OrderByDescending(r => r).ToArray() 
                    : rnd.OrderBy(r => r).ToArray();
        }
    }
}
