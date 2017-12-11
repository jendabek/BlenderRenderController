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

            var blendData = GetBlendData(BLEND_PATH);
            var chunks = Chunk.CalcChunksByLength(blendData.Start, 3000, 300);

            var mockProj = new ProjectSettings
            {
                BlendData = new BlendData
                {
                    OutputPath = OUT_PATH,
                    ProjectName = "UnitTest"
                },
                BlendPath = BLEND_PATH,
                MaxConcurrency = 5,
            };

            var renderMngr = new RenderManager(MockSettings);
            renderMngr.Setup(mockProj);

            renderMngr.Finished += (s, e) => finishedEvent.Set();

            renderMngr.StartAsync();
            finishedEvent.Wait();

            Assert.AreEqual(chunks.TotalLength(), renderMngr.NumberOfFramesRendered);
        }

        [TestMethod]
        public void RenderManager_BaseTest_progress_report()
        {
            ClearFolder(OUT_PATH);
            var progress = new Progress<RenderProgressInfo>();
            progress.ProgressChanged += Progress_ProgressChanged;

            var blendData = GetBlendData(BLEND_PATH);
            var chunks = Chunk.CalcChunksByLength(blendData.Start, 3000, 300);
            var mockProj = new ProjectSettings
            {
                BlendData = new BlendData
                {
                    OutputPath = OUT_PATH,
                    ProjectName = "UnitTest"
                },
                BlendPath = BLEND_PATH,
                MaxConcurrency = 5,
            };

            var renderMngr = new RenderManager(MockSettings);
            renderMngr.Setup(mockProj);
            renderMngr.Finished += (s, e) => finishedEvent.Set();

            renderMngr.StartAsync(progress);
            finishedEvent.Wait();

            Assert.AreEqual(chunks.TotalLength(), renderMngr.NumberOfFramesRendered);
        }
        
        [TestMethod]
        public void RenderManager_ThrowOn_Properties_changed_while_in_progress()
        {
            ClearFolder(OUT_PATH);

            var bData = GetTestBlendData();
            var chunks = Chunk.CalcChunksByLength(bData.Start, 5000, 300);

            var mockProj = new ProjectSettings
            {
                BlendData = new BlendData
                {
                    OutputPath = OUT_PATH,
                    ProjectName = "UnitTest"
                },
                BlendPath = BLEND_PATH,
                MaxConcurrency = 5,
            };

            var renderMngr = new RenderManager(MockSettings);
            renderMngr.Setup(mockProj);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                renderMngr.StartAsync();

                Thread.Sleep(TimeSpan.FromSeconds(5));

                renderMngr.Setup(new ProjectSettings());
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
            var chunks = Chunk.CalcChunksByLength(bData.Start, 5000, 300);
            var mockProj = new ProjectSettings
            {
                BlendData = new BlendData
                {
                    OutputPath = OUT_PATH,
                    ProjectName = "UnitTest"
                },
                BlendPath = BLEND_PATH,
                MaxConcurrency = 5,
            };

            var renderMngr = new RenderManager(MockSettings);
            renderMngr.Setup(mockProj);

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                renderMngr.StartAsync();
                Thread.Sleep(TimeSpan.FromSeconds(5));

                renderMngr.StartAsync();
            }, 
            "Start() was called twice");

            //ClearFolder(OUT_PATH);
            renderMngr.Abort();
        }

        private void Progress_ProgressChanged(object sender, RenderProgressInfo e)
        {
            Console.WriteLine("Progress report: {0} frames rendered, {1} parts completed", e.FramesRendered, e.PartsCompleted);
        }
    }
}
