using System;
using System.Linq;
using BRClib;
using BRClib.Extentions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class CalcChunkTests
    {
        // How many times the tests w/ random values
        // will loop
        const int RANDOM_TEST_LOOPS = 1_000;

        // the max value for random frame numbers
        const int MAX_FRAME = 99_999;

        // the max value for random core counts
        const int MAX_CORES = 17;

        // avoids attempting calculations w/ 
        // unrealistic small lenghts
        const int MIN_CHUNK_LEN = 50;

        [TestMethod]
        public void CalcChunks_randomGen_tests()
        {
            StringBuilder sb = new StringBuilder();
            int skips = 0;
            int i = 0;

            for (;i < RANDOM_TEST_LOOPS; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, MAX_FRAME);

                var cores = RandomParams.GetNumber(1, MAX_CORES);
                var start = randPair[0];
                var end = randPair[1];

                var totalLen = end - start + 1;

                if (totalLen < MIN_CHUNK_LEN)
                {
                    skips++;
                    continue;
                }

                var calcResult = new List<Chunk>(Chunk.CalcChunks(start, end, cores));

                sb.AppendFormat("\n{0}: Number of chunks: {1}\n", i, calcResult.Count);
                sb.AppendFormat("Start: {0}, End: {1}, Total lenght: {2}, CoreCount: {3}\n", start, end, totalLen, cores);
                foreach (var res in calcResult)
                {
                    sb.AppendLine(res + "\tLenght: " + res.Length);
                }

                Assert.AreEqual(start, calcResult.First().Start, "First calcResult.Start was diferent then param start");
                Assert.AreEqual(end, calcResult.Last().End, "Last calcResult.End was diferent then param end");
                Assert.IsTrue(CheckStartAndEnd(calcResult), "There are unexpected gaps between Chunks");
                Assert.AreEqual(totalLen, calcResult.TotalLength(), "Lenghts don't match");
            }

            Console.WriteLine("CalcChunks Random test. {0} runs", i - skips);
            Console.Write(sb.ToString());
        }

        [TestMethod]
        public void CalcChunksByLength_randomGen_tests()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int skips = 0;

            for (; i < RANDOM_TEST_LOOPS; i++)
            {
                int[] randPair = RandomParams.GetNumberPair(1, MAX_FRAME);

                int start = randPair[0];
                int end = randPair[1];
                int totalLen = end - start + 1;

                // ignore small spans
                if (totalLen < MIN_CHUNK_LEN)
                {
                    skips++;
                    continue;
                }

                // range of random lengths:
                var minLen = totalLen * 0.05;
                var maxLen = totalLen * 0.5;

                int len = RandomParams.GetNumber((int)minLen, (int)maxLen);

                // ignore small spans
                if (len < MIN_CHUNK_LEN)
                {
                    skips++;
                    continue;
                }

                var calcResult = new List<Chunk>(Chunk.CalcChunksByLength(start, end, len));

                sb.AppendFormat("\n{0}: Number of chunks: {1}, Chunk lenght: {2}\n", i, calcResult.Count, len);
                sb.AppendFormat("Start frame: {0}, End frame: {1}, Total lenght: {2}\n", start, end, totalLen);
                foreach (var res in calcResult)
                {
                    sb.AppendLine(res + "\tLenght: " + res.Length);
                }

                var actualLen = calcResult.First().Length;
                Assert.AreEqual(len, actualLen, "Lenght and generated chunk's lenght don't match");

                Assert.AreEqual(start, calcResult.First().Start, "First calcResult.Start was diferent then param start");
                Assert.AreEqual(end, calcResult.Last().End, "Last calcResult.End was diferent then param end");
                Assert.IsTrue(CheckStartAndEnd(calcResult), "There are unexpected gaps between Chunks");
                Assert.AreEqual(totalLen, calcResult.TotalLength(), "Total Lenghts don't match");
            }

            Console.WriteLine("CalcChunksByLength Random test. {0} runs", i - skips);
            Console.Write(sb.ToString());
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

            // start cannot be equal to end
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunks(2000, 2000, 550);
            });

            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunksByLength(2000, 2000, 550);
            });

            // div cannot 0
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunks(50, 2000, 0);
            });

            // invalid chunk lenght
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var chunks = Chunk.CalcChunksByLength(2000, 50, 0);
            });
        }



        // check if the list of chunks is sequential
        bool CheckStartAndEnd(List<Chunk> chunks)
        {
            using (var cEnum = chunks.GetEnumerator())
            {
                while (cEnum.MoveNext())
                {
                    Chunk c1, c2;

                    c1 = cEnum.Current;

                    if (cEnum.MoveNext())
                    {
                        c2 = cEnum.Current;
                    }
                    else break;

                    // there cannot be wider gaps between items
                    if (c2.Start != c1.End + 1) return false;
                }

                return true;
            }
        }
    }
}
