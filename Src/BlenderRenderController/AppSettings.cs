using BlenderRenderController.Infra;
using BRClib;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace BlenderRenderController
{
    [JsonObject(Description = "Brc settings")]
    public class AppSettings
    {
        private static readonly string _baseDir = Environment.CurrentDirectory;
        private string _scriptsFolderPath;
        private const int RECENT_BLENDS_MAX_COUNT = 10;
        const string SETTINGS_FILE = "brc_settings.json";
        //bool blenderFound, ffmpegFound;


        private static string DefBlenderFolder
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        return @"C:\Program Files\Blender Foundation\Blender\";
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                    default:
                        return "/usr/bin/";
                }
            }
        }

        private static string DefFFmpegFolder
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    // ffmpeg exe is in base dir
                    return string.Empty;
                }
                else
                {
                    return "/usr/bin/";
                }
            }
        }


        private static AppSettings _instance;

        /// <summary>
        /// Settings singleton
        /// </summary>
        public static AppSettings Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(AppSettings))
                    {
                        if (_instance == null)
                        {
                            //_instance = new AppSettings();
                            _instance = Load();
                        }
                    }
                }

                return _instance;
            }
        }

        [JsonProperty("RecentBlends")]
        public RecentBlendsCollection RecentProjects { get; set; }

        public string BlenderProgram { get; set; }
        public string FFmpegProgram { get; set; }
        public bool Verbose { get; set; }
        public bool DisplayToolTips { get; set; }
        public AfterRenderAction AfterRender { get; set; }
        public Renderer Renderer { get; set; }


        public string ScriptsFolder
        {
            get => _scriptsFolderPath;
            set => _scriptsFolderPath = value;
        }

        public string BlenderExeName { get; private set; }

        public string FFmpegExeName { get; private set; }



        [JsonIgnore]
        private static AppSettings Defaults
        {
            get
            {
                var blenderExe = GetProgramFileName("blender");
                var ffmpegExe = GetProgramFileName("ffmpeg");

                return new AppSettings()
                {
                    BlenderExeName = blenderExe,
                    BlenderProgram = DefBlenderFolder + blenderExe,
                    FFmpegExeName = ffmpegExe,
                    FFmpegProgram = DefFFmpegFolder + ffmpegExe,
                    Verbose = false,
                    DisplayToolTips = true,
                    AfterRender = AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN,
                    Renderer = Renderer.BLENDER_RENDER,
                    ScriptsFolder = Path.Combine(_baseDir, Constants.ScriptsSubfolder),
                    RecentProjects = new RecentBlendsCollection()
                };
            }
        }


        public bool CheckCorrectConfig()
        {
            if (!File.Exists(BlenderProgram))
            {
                if (VerifyLocation(BlenderExeName, "-v", "Blender"))
                {
                    BlenderProgram = GetPathFromEnv(BlenderExeName);
                    return CheckCorrectConfig();
                }
                else return false;
            }

            if (!File.Exists(FFmpegProgram))
            {
                if (VerifyLocation(FFmpegExeName, "-version", "ffmpeg version"))
                {
                    FFmpegProgram = GetPathFromEnv(FFmpegExeName);
                    return CheckCorrectConfig();
                }
                else return false;
            }

            return true;
        }

        bool VerifyLocation(string exe, string args, string match)
        {
            string fullExe = GetPathFromEnv(exe);
            if (fullExe == null)
            {
                return false;
            }

            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = fullExe,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            p.StartInfo = info;
            p.Start();
            var line = p.StandardOutput.ReadLine();

            return line.IndexOf(match, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        string GetPathFromEnv(string exe)
        {
            var PATH = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator);

            var exePath = PATH.Select(x => Path.Combine(x, exe))
                                  .Where(x => File.Exists(x))
                                  .FirstOrDefault();

            return exePath;
        }

        private static string GetProgramFileName(string prog)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return prog + ".exe";
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                default:
                    return prog;
            }
        }

        public void Save()
        {
            var jsonStr = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(Path.Combine(_baseDir, SETTINGS_FILE), jsonStr);
        }

        static AppSettings Load()
        {
            return Load(Path.Combine(_baseDir, SETTINGS_FILE));
        }

        public static AppSettings Load(string settingsPath)
        {
            return File.Exists(settingsPath) 
                        ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(settingsPath))
                        : AppSettings.Defaults;


        }
    }
}
