using BlenderRenderController;
using BRClib;
using BRClib.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    static class TestHelpers
    {
        public const string BLEND_PATH = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\stuff\test project2.blend";
        public const string OUT_PATH = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\expo";
        const string SCRIPTS_DIR = @"C:\Users\Pedro\Source\Repos\BlenderRenderController\Src\BRClib\Scripts";
        public static BrcSettings MockSettings = new BrcSettings
        {
            BlenderProgram = "C:\\Program Files\\Blender Foundation\\Blender\\blender.exe",
            FFmpegProgram = "E:\\Programas\\FFmpeg\\Snapshot\\bin\\ffmpeg.exe",
            Renderer = Renderer.BLENDER_RENDER
        };

        public static BlendData GetTestBlendData()
        {
            return GetBlendData(BLEND_PATH);
        }

        public static BlendData GetBlendData(string blendPath)
        {
            var proc = GetBlendDataProc(blendPath);

            proc.Start();
            var output = proc.StandardOutput.ReadToEnd();

            return Utilities.ParsePyOutput(output);
        }

        public static Process GetBlendDataProc(string path)
        {
            var giScript = Path.Combine(SCRIPTS_DIR, "get_project_info.py");

            var giProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = MockSettings.BlenderProgram,
                    Arguments = $"-b \"{path}\" -P \"{giScript}\"",

                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },

                EnableRaisingEvents = true,
            };

            return giProc;
        }

        public static void ClearFolder(string path)
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
