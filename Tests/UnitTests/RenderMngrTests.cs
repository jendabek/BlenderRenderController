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
        public void RenderManager_BaseTest()
        {
            ClearFolder(OUT_PATH);

            var run = RunManager();

            Assert.AreEqual(run.chunks.TotalLength(), run.renderMngr.NumberOfFramesRendered);
        }

        [TestMethod]
        public void RenderManager_BaseTest_progress_report()
        {
            ClearFolder(OUT_PATH);
            var progress = new Progress<RenderProgressInfo>();
            progress.ProgressChanged += Progress_ProgressChanged;

            var run = RunManager(progress);

            Assert.AreEqual(run.chunks.TotalLength(), run.renderMngr.NumberOfFramesRendered);
        }

        [TestMethod]
        public void RenderManager_ThrowOn_Properties_changed_while_in_progress()
        {
            ClearFolder(OUT_PATH);

            var bData = GetTestBlendData();
            var chunks = Chunk.CalcChunksByLenght(bData.Start, 5000, 300);
            var renderMngr = new RenderManager(chunks, MockSettings)
            {
                BlendFilePath = BLEND_PATH,
                ChunksFolderPath = Path.Combine(OUT_PATH, "chunks"),
                MaxConcurrency = 5,
                BaseFileName = "UnitTest"
            };

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                renderMngr.Start();

                Thread.Sleep(TimeSpan.FromSeconds(5));

                renderMngr.MaxConcurrency = 99;
            }, 
            "Property value changed while a render was in progress");

            //ClearFolder(OUT_PATH);
            renderMngr.Abort();
        }

        [TestMethod]
        public void RenderManager_ThrowOn_Start_called_while_in_progress()
        {
            ClearFolder(OUT_PATH);

            var bData = GetBlendData(BLEND_PATH);
            var chunks = Chunk.CalcChunksByLenght(bData.Start, 5000, 300);
            var renderMngr = new RenderManager(chunks, MockSettings)
            {
                BlendFilePath = BLEND_PATH,
                ChunksFolderPath = Path.Combine(OUT_PATH, "chunks"),
                MaxConcurrency = 5,
                BaseFileName = "UnitTest"
            };

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                renderMngr.Start();
                Thread.Sleep(TimeSpan.FromSeconds(5));

                renderMngr.Start();
            }, 
            "Start() was called twice");

            //ClearFolder(OUT_PATH);
            renderMngr.Abort();
        }

        private void Progress_ProgressChanged(object sender, RenderProgressInfo e)
        {
            Console.WriteLine("Progress report: {0} frames rendered, {1} parts completed", e.FramesRendered, e.PartsCompleted);
        }

        private (Chunk[] chunks, RenderManager renderMngr) RunManager(IProgress<RenderProgressInfo> progress = null)
        {
            var blendData = GetBlendData(BLEND_PATH);
            var chunks = Chunk.CalcChunksByLenght(blendData.Start, 3000, 300);
            var renderMngr = new RenderManager(chunks, MockSettings)
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
