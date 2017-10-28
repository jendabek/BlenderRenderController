using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlenderRenderController;
using BRClib;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Threading;

namespace UnitTests
{
    [TestClass]
    public class RenderMngrTests
    {
        const string BLEND_PATH = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\stuff\test_project.blend";
        const string SETTINGS_JSON = @"C:\Users\Pedro\Source\Repos\BlenderRenderController\Src\BlenderRenderController\bin\Debug\brc_settings.json";
        const string OUT_PATH = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\expo";
        AppSettings settings = AppSettings.Load(SETTINGS_JSON);
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
            var renderMngr = new RenderManager(chunks, settings)
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

        BlendData GetBlendData(string blendPath)
        {
            var getBlendInfoCom = new Process();
            var info = new ProcessStartInfo()
            {
                FileName = settings.BlenderProgram,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Format(CommandARGS.GetInfoComARGS,
                                            blendPath,
                                            Path.Combine(settings.ScriptsFolder, "get_project_info.py"))
            };

            getBlendInfoCom.StartInfo = info;
            getBlendInfoCom.Start();

            var output = getBlendInfoCom.StandardOutput.ReadToEnd();

            return Utilities.ParsePyOutput(output);
        }

        void ClearFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

    }
}
