using BlenderRenderController;
using BRClib;
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
        public const string BLEND_PATH = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\stuff\test_project.blend";
        public const string OUT_PATH = @"E:\Bibliotecas E\_projetos\Blender\Video\teste\expo";
        const string SCRIPTS_DIR = @"C:\Users\Pedro\Source\Repos\BlenderRenderController\Src\BRClib\Scripts";
        public static AppSettings MockSettings = new AppSettings
        {
            BlenderProgram = "C:\\Program Files\\Blender Foundation\\Blender\\blender.exe",
            FFmpegProgram = "E:\\Programas\\FFmpeg\\Snapshot\\bin\\ffmpeg.exe",
            Renderer = BlenderRenderes.BLENDER_RENDER
        };

        public static BlendData GetTestBlendData()
        {
            return GetBlendData(BLEND_PATH);
        }

        public static BlendData GetBlendData(string blendPath)
        {
            var getBlendInfoCom = GetBlendDataProc(blendPath);
            getBlendInfoCom.Start();
            var output = getBlendInfoCom.StandardOutput.ReadToEnd();

            return Utilities.ParsePyOutput(output);
        }

        public static Process GetBlendDataProc(string blendPath)
        {
            var getBlendInfoCom = new Process();
            var info = new ProcessStartInfo()
            {
                FileName = MockSettings.BlenderProgram,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                //CreateNoWindow = true,
                Arguments = string.Format(CommandARGS.GetInfoComARGS,
                                            blendPath,
                                            Path.Combine(SCRIPTS_DIR, "get_project_info.py"))
            };

            getBlendInfoCom.StartInfo = info;

            return getBlendInfoCom;
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
