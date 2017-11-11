using System;
using System.Linq;
using BRClib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UnitTests
{
    [TestClass]
    public class CalcChunkTests
    {
        const int RANDOM_TEST_LOOPS = 50;

        [TestMethod]
        public void CalcChunks_randomGen_tests()
        {
            for (int i = 0; i < RANDOM_TEST_LOOPS; i++)
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

                Console.WriteLine();
                Console.WriteLine("Number of chunks: " + calcResult.Length);
                Console.WriteLine("Start: {0}, End: {1}, Total lenght: {2}, CoreCount: {3}", start, end, totalLen, cores);
                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "   Lenght: " + res.Length);
                }

                Assert.IsFalse(calcResult.Last().End != end, "Last calcResult.End was diferent then param end");
                Assert.IsTrue(calcResult.Length == cores, "Number of chunks does not match core count");

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalcChunks_ThrowOn_start_greater_then_end()
        {
            int[] rnd = RandomParams.GetNumberPair(1, 9999, invert: true);
            int cores = RandomParams.GetNumber(1, 17);

            int start = rnd[0];
            int end = rnd[1];

            Console.WriteLine("Start: {0}, End: {1}", start, end);

            var calcResult = Chunk.CalcChunks(start, end, 8);

        }

        [TestMethod]
        public void CalcChunksByLength_randomGen_tests()
        {
            for (int i = 0; i < RANDOM_TEST_LOOPS; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, 99999);

                int start = randPair[0];
                int end = randPair[1];
                int totalLen = end - start + 1;

                // range of random lengths:
                var minLen = totalLen * 0.05;
                var maxLen = totalLen * 0.4;

                int len = RandomParams.GetNumber((int)minLen, (int)maxLen);
                if (len > maxLen || len < minLen)
                {
                    // ignore unrealistic lenghts
                    continue;
                }

                var calcResult = Chunk.CalcChunksByLenght(start, end, len);
                Console.WriteLine();
                Console.WriteLine("Number of chunks: " + calcResult.Length);
                Console.WriteLine("Start: {0}, End: {1}, Leght: {2}", start, end, len);
                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "   Lenght: " + res.Length);
                }

                Assert.IsFalse(calcResult.Last().End != end, "Last calcResult.End was different then param end");
                var actualLen = calcResult.First().Length;
                Assert.AreEqual(len, actualLen, "Lenght and generated chunk's lenght don't match");
            }

        }

        [TestMethod]
        public void CalcChunks_TotalLenght_is_accuerate()
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CalcChunksByLength_ThrowOn_invalid_start_end()
        {
            // start is greater then end
            var start = 2000;
            var end = 50;
            var lenght = 550;

            var chunks = Chunk.CalcChunksByLenght(start, end, lenght);
        }

    }
}
