using System;
using System.Linq;
using BRClib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class CalcChunkTests
    {
        // How many times the tests w/ random values
        // will loop
        const int RANDOM_TEST_LOOPS = 10_000;

        // the max value for random frame numbers
        const int MAX_FRAME = 99_999;

        // the max value for random core counts
        const int MAX_CORES = 17;

        // TODO: Enforce this?
        // avoids attempting calculations w/ 
        // unrealistic small lenghts
        const int MIN_CHUNK_LEN = 50;

        [TestMethod]
        public void CalcChunks_randomGen_tests()
        {
            for (int i = 0; i < RANDOM_TEST_LOOPS; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, MAX_FRAME);

                var cores = RandomParams.GetNumber(1, MAX_CORES);
                var start = randPair[0];
                var end = randPair[1];

                var totalLen = end - start + 1;

                if (totalLen < MIN_CHUNK_LEN)
                {
                    continue;
                }

                var calcResult = new List<Chunk>(Chunk.CalcChunks(start, end, cores));
                //var calcResult = Chunk.CalcChunks(start, end, cores);

                Console.WriteLine();
                Console.WriteLine("{0}: Number of chunks: {1}", i, calcResult.Count);
                Console.WriteLine("Start: {0}, End: {1}, Total lenght: {2}, CoreCount: {3}", start, end, totalLen, cores);

                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "\tLenght: " + res.Length);
                }

                Assert.AreEqual(start, calcResult.First().Start, "First calcResult.Start was diferent then param start");
                Assert.AreEqual(end, calcResult.Last().End, "Last calcResult.End was diferent then param end");
            }
        }

        [TestMethod]
        public void CalcChunksByLength_randomGen_tests()
        {
            for (int i = 0; i < RANDOM_TEST_LOOPS; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, MAX_FRAME);

                int start = randPair[0];
                int end = randPair[1];
                int totalLen = end - start + 1;

                // ignore small spans
                if (totalLen < MIN_CHUNK_LEN)
                {
                    continue;
                }

                // range of random lengths:
                var minLen = totalLen * 0.05;
                var maxLen = totalLen * 0.5;

                int len = RandomParams.GetNumber((int)minLen, (int)maxLen);

                // ignore small spans
                if (len < MIN_CHUNK_LEN)
                {
                    continue;
                }

                var calcResult = new List<Chunk>(Chunk.CalcChunksByLength(start, end, len));
                //var calcResult = Chunk.CalcChunksByLength(start, end, len);

                Console.WriteLine();
                Console.WriteLine("{0}: Number of chunks: {1}", i, calcResult.Count);
                Console.WriteLine("Start: {0}, End: {1}, Leght: {2}", start, end, len);
                foreach (var res in calcResult)
                {
                    Console.WriteLine(res + "\tLenght: " + res.Length);
                }

                var actualLen = calcResult.First().Length;
                Assert.AreEqual(len, actualLen, "Lenght and generated chunk's lenght don't match");

                Assert.AreEqual(start, calcResult.First().Start, "First calcResult.Start was diferent then param start");
                Assert.AreEqual(end, calcResult.Last().End, "Last calcResult.End was diferent then param end");
            }
        }

        [TestMethod]
        public void CalcChunks_TotalLenght_is_accurate()
        {
            int[] randPair = RandomParams.GetNumberPair(1, MAX_FRAME);

            var start = randPair[0];
            var end = randPair[1];
            int totalLen = end - start + 1;
            int cores = RandomParams.GetNumber(1, MAX_CORES);

            var chunks = Chunk.CalcChunks(start, end, cores);

            Console.WriteLine("Total lenght = " + totalLen);
            Console.WriteLine("Chunks total length = " + chunks.TotalLength());

            Assert.AreEqual(totalLen, chunks.TotalLength(), "Lenghts are not equal");
        }

        [TestMethod]
        public void CalcChunks_ThrowOn_invalid_arguments()
        {
            // start cannot be greater then end
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunksByLength(2000, 50, 550);
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunks(2000, 50, 550);
            });

            // div cannot 0
            Assert.ThrowsException<DivideByZeroException>(() =>
            {
                var chunks = Chunk.CalcChunks(50, 2000, 0);
            });

            // invalid chunk lenght
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunksByLength(2000, 50, 0);
            });
        }

    }
}
