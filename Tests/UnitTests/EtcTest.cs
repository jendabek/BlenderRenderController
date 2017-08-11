using BRClib;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class EtcTest
    {
        string chunkFolderPath = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\expo\chunks";

        [TestMethod]
        public void GetChunksTest()
        {
            var dirFiles = Directory.GetFiles(chunkFolderPath, "*.*", SearchOption.TopDirectoryOnly);

            var files = Utilities.GetChunkFiles(dirFiles);

            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
        }
    }
}
