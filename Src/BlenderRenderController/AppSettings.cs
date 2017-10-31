using NLog;
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
        [JsonProperty("RecentBlends")]
        private ObservableCollection<string> _recentBlends = new ObservableCollection<string>();

        private static readonly string _baseDir = Environment.CurrentDirectory;
        private string _blenderExeName, _scriptsFolderPath, _ffmpegExeName;
        private const int RECENT_BLENDS_MAX_COUNT = 10;
        const string SETTINGS_FILE = "brc_settings.json";
        //bool blenderFound, ffmpegFound;

        private static AppSettings _instance;

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

        public event EventHandler<NotifyCollectionChangedEventArgs> RecentBlends_Changed;

        public string BlenderProgram { get; set; }
        public string FFmpegProgram { get; set; }
        public bool Verbose { get; set; }
        public bool DisplayToolTips { get; set; }
        public AfterRenderAction AfterRender { get; set; }
        public BlenderRenderes Renderer { get; set; }

        public IList<string> GetRecentBlends()
        {
            return _recentBlends;
        }

        [JsonIgnore]
        public int RecentBlendsCount => _recentBlends.Count;

        [JsonIgnore]
        public string ScriptsFolder
        {
            get => _scriptsFolderPath;
            private set => _scriptsFolderPath = value;
        }

        public string BlenderExeName { get => _blenderExeName; }

        public string FFmpegExeName { get => _ffmpegExeName; }

        //public string AutoMixdownExt { get; set; }

        public AppSettings()
        {
            _ffmpegExeName = GetProgramFileName("ffmpeg");
            _blenderExeName = GetProgramFileName("blender");
            BlenderProgram = DefBlenderFolder + _blenderExeName;
            FFmpegProgram = DefFFmpegFolder + _ffmpegExeName;

            _recentBlends.CollectionChanged += RecentBlends_CollectionChanged;
            _scriptsFolderPath = Path.Combine(_baseDir, Constants.ScriptsSubfolder);
        }

        [JsonIgnore]
        private static AppSettings Defaults
        {
            get
            {
                var settings = new AppSettings()
                {
                    BlenderProgram = DefBlenderFolder + GetProgramFileName("blender"),
                    FFmpegProgram = DefFFmpegFolder + GetProgramFileName("ffmpeg"),
                    Verbose = false,
                    DisplayToolTips = true,
                    AfterRender = AfterRenderAction.JOIN | AfterRenderAction.MIXDOWN,
                    Renderer = BlenderRenderes.BLENDER_RENDER
                };

                return settings;
            }
        }

        private void RecentBlends_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecentBlends_Changed?.Invoke(sender, e);
        }

        public void AddRecentBlend(string blendFilePath)
        {
            // dont want to show one file many times
            if (_recentBlends.Contains(blendFilePath))
            {
                var oldIdx = _recentBlends.IndexOf(blendFilePath);
                _recentBlends.Move(oldIdx, 0);
                return;
            }

            // delete last if the list count hits max
            if (_recentBlends.Count == RECENT_BLENDS_MAX_COUNT)
            {
                _recentBlends.RemoveAt(RECENT_BLENDS_MAX_COUNT - 1);
            }

            _recentBlends.Insert(0, blendFilePath);
        }
        public void RemoveRecentBlend(string blendFilePath)
        {
            if (_recentBlends.Contains(blendFilePath))
            {
                _recentBlends.Remove(blendFilePath);
            }
        }

        public void ClearRecentBlend()
        {
            if (_recentBlends.Count > 0)
            {
                var response = MessageBox.Show(
                                 "This will clear all files in the recent blends list, are you sure?", 
                                 "Clear recent blends?",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                switch (response)
                {
                    case DialogResult.Yes:
                        _recentBlends.Clear();
                        break;
                    case DialogResult.No:
                    default:
                        break;
                }
            }
        }

        public bool CheckCorrectConfig()
        {
            List<AppErrorCode> errors = new List<AppErrorCode>();

            if (!File.Exists(BlenderProgram))
            {
                errors.Add(AppErrorCode.BLENDER_PATH_NOT_SET);
            }

            if (!File.Exists(FFmpegProgram))
            {
                errors.Add(AppErrorCode.FFMPEG_PATH_NOT_SET);
            }

            if (errors.Count == 0)
            {
                return true;
            }

            //if (showErrors)
            //{
            //    Helper.ShowErrors(MessageBoxIcon.Exclamation, errors.ToArray());
            //}

            return false;
        }

        private static string GetProgramFileName(string prog)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return prog + ".exe";
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return prog;
                default:
                    return prog;
            }
        }



        public void Save()
        {
            var jsonStr = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(Path.Combine(_baseDir, SETTINGS_FILE), jsonStr);
        }

        public void ReLoad()
        {
            _instance = Load();
        }

        static AppSettings Load()
        {
            return Load(Path.Combine(_baseDir, SETTINGS_FILE));
        }

        public static AppSettings Load(string settingsPath)
        {
            //var settingsPath = Path.Combine(_baseDir, SETTINGS_FILE);

            var appSet = File.Exists(settingsPath) 
                        ? JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(settingsPath))
                        : AppSettings.Defaults;

            // apply log
            if (appSet.Verbose)
            {
                //string[] names = { typeof(BrcForm).FullName, typeof(Helper).FullName };
                foreach (var rule in LogManager.Configuration.LoggingRules)
                {
                    if (rule.NameMatches(typeof(BrcForm).FullName))
                        rule.EnableLoggingForLevel(LogLevel.Info);
                }
            }

            return appSet;
        }
    }
}
