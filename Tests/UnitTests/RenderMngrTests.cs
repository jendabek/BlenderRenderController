using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlenderRenderController;
using BRClib;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;
using static UnitTests.TestHelpers;

namespace UnitTests
{
    [TestClass]
    public class RenderMngrTests
    {
        ManualResetEventSlim finishedEvent = new ManualResetEventSlim();

        [TestMethod]
        public void Mngr_Test()
        {
            ClearFolder(OUT_PATH);

            var run = RunManager();

            Assert.AreEqual(run.chunks.TotalLength(), run.renderMngr.NumberOfFramesRendered);
        }

        [TestMethod]
        public void Mngr_test_progress_report()
        {
            ClearFolder(OUT_PATH);
            var progress = new Progress<RenderProgressInfo>();
            progress.ProgressChanged += Progress_ProgressChanged;

            var run = RunManager(progress);

            Assert.AreEqual(run.chunks.TotalLength(), run.renderMngr.NumberOfFramesRendered);
        }


        private void Progress_ProgressChanged(object sender, RenderProgressInfo e)
        {
            Console.WriteLine("Progress report: {0} frames rendered, {1} parts completed", e.FramesRendered, e.PartsCompleted);
        }

        private (Chunk[] chunks, RenderManager renderMngr) RunManager(IProgress<RenderProgressInfo> progress = null)
        {
            var blendData = GetBlendData(BLEND_PATH);
            var chunks = Chunk.CalcChunksByLenght(blendData.Start, 3000, 300);
            var renderMngr = new RenderManager(chunks, Settings)
            {
                BlendFilePath = BLEND_PATH,
                ChunksFolderPath = Path.Combine(OUT_PATH, "chunks"),
                MaxConcurrency = 5,
                BaseFileName = "UnitTest"
            };
            renderMngr.Finished += (s, e) => finishedEvent.Set();

            renderMngr.Start(progress);
            finishedEvent.Wait();

            return (chunks, renderMngr);
        }


    }
}
