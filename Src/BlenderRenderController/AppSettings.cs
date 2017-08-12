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

namespace BlenderRenderController
{
    [JsonObject(Description = "Brc settings")]
    class AppSettings
    {
        private ObservableCollection<string> _recentBlends = new ObservableCollection<string>();
        private static readonly string _baseDir = Environment.CurrentDirectory;
        private string _blenderExeName, _scriptsFolderPath, _ffmpegExeName;
        private int RECENT_BLENDS_MAX_COUNT = 10;
        const string SETTINGS_FILE = "brc_settings.json";

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

        public event EventHandler<NotifyCollectionChangedEventArgs> RecentBlends_Changed;

        public string BlenderProgram { get; set; }
        public string FFmpegProgram { get; set; }
        public bool Verbose { get; set; }
        public bool DisplayToolTips { get; set; }
        public AfterRenderAction AfterRender { get; set; }
        public BlenderRenderes Renderer { get; set; }


        public ObservableCollection<string> RecentBlends { get => _recentBlends; }

        [JsonIgnore]
        public string ScriptsFolder
        {
            get => _scriptsFolderPath;
            private set => _scriptsFolderPath = value;
        }

        [JsonIgnore]
        public string BlenderExeName { get => _blenderExeName; }

        [JsonIgnore]
        public string FFmpegExeName { get => _ffmpegExeName; }


        public AppSettings()
        {
            _ffmpegExeName = GetProgramFileName("ffmpeg");
            _blenderExeName = GetProgramFileName("blender");
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
                    BlenderProgram = @"C:\Program Files\Blender Foundation\Blender\" + GetProgramFileName("blender"),
                    FFmpegProgram = Path.Combine(_baseDir, GetProgramFileName("ffmpeg")),
                    Verbose = false,
                    DisplayToolTips = true,
                    AfterRender = AfterRenderAction.JOIN_MIXDOWN,
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
            //dont want to show one file many times
            if (_recentBlends.Contains(blendFilePath))
            {
                _recentBlends.Remove(blendFilePath);
            }

            //delete last if the list is larger than _LAST_BLENDS_MAX_COUNT
            if (_recentBlends.Count == RECENT_BLENDS_MAX_COUNT)
            {
                _recentBlends.RemoveAt(RECENT_BLENDS_MAX_COUNT - 1);
            }
            _recentBlends.Insert(0, blendFilePath);
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
                    case DialogResult.OK:
                        _recentBlends.Clear();
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        MessageBox.Show("Something wrong happend.");
                        break;
                }
            }
        }

        public bool CheckCorrectConfig(bool showErrors = true)
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

            if (showErrors)
            {
                Helper.ShowErrors(MessageBoxIcon.Exclamation, errors.ToArray());
            }

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
        public static AppSettings Load()
        {
            var settingsPath = Path.Combine(_baseDir, SETTINGS_FILE);

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
