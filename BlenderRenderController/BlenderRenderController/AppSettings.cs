using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace BlenderRenderController
{

    public class AppSettings
    {
        //THESE PROPERTIES (in _jsonProperties) ARE STORED AND LOADED automatically from external JSON file
        private string[] _jsonProperties = { "recentBlends", "blenderPath", "ffmpegPath", "renderer", "afterRenderAction", "displayTooltips", "verboseLog"};

        private const int _RECENT_BLENDS_MAX_COUNT = 10;
        public const string BLENDER_EXE_NAME = "blender.exe";
        public const string FFMPEG_EXE_NAME = "ffmpeg.exe";

        public const string BLENDER_PATH_DEFAULT = "C:\\Program Files\\Blender Foundation\\Blender";
        public const string FFMPEG_PATH_DEFAULT = ""; //EXE dir

        private List<string> _recentBlends = new List<string>();
        private string _jsonFileName = "settings.json";
        private string _chunksSubfolder = "chunks";
        private string _scriptsSubfolder = "scripts";
        private string _audioFormat = "ac3";
        private string _chunksTxtFileName = "chunklist.txt";
        private string _afterRenderAction = AppStrings.AFTER_RENDER_JOIN_MIXDOWN;
        private string _renderer = AppStrings.RENDERER_BLENDER;
        private decimal _processCount;
        private decimal _chunkLength;
        private bool _displayTooltips = true;
        private bool _verboseLog = false;
        private string[] _allowedFormats = { "avi", "mp4", "mov", "mkv", "mpg", "flv" };

        private string _scriptsPath, _blenderPath, _ffmpegPath;

        private int _processCheckInterval = 20;
        private bool _appConfigured = false;
        private SettingsForm _settingsForm;

        public void init()
        {
            //LOADing data from JSON and set it to properties
            _scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _scriptsSubfolder);
            _blenderPath = BLENDER_PATH_DEFAULT;
            _ffmpegPath = FFMPEG_PATH_DEFAULT;
            loadJsonSettings();
            checkCorrectConfig();
        }


        private void loadJsonSettings()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _jsonFileName);
            if (File.Exists(jsonFilePath))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonText = File.ReadAllText(jsonFilePath);
                var jsonSettings = serializer.Deserialize<Dictionary<string, object>>(jsonText.ToString());

                foreach (var propertyName in _jsonProperties)
                {
                    //property not found in json
                    try
                    {
                        var test = jsonSettings[propertyName];
                    }
                    catch (Exception) {return;}

                    //converting ArrayList object from json to List
                    if (jsonSettings[propertyName] is ArrayList)
                    {
                        ArrayList arrayList = (ArrayList)jsonSettings[propertyName];
                        List<string> listValue = arrayList.Cast<string>().ToList();
                        GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, listValue);
                    }
                    //converting Int32 to decimal
                    else if (jsonSettings[propertyName] is Int32)
                    {
                        decimal decimalValue = (decimal)(int)jsonSettings[propertyName];
                        GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, decimalValue);
                    }
                    //just a string
                    else
                    {
                        GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, jsonSettings[propertyName]);
                    }
                }
            }
        }

        public bool save()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonObject = new Dictionary<string, object>();
            
            foreach (var propertyName in _jsonProperties)
            {
                jsonObject[propertyName] = GetType().GetProperty(propertyName).GetValue(this);
            }
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _jsonFileName), serializer.Serialize(jsonObject));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void addRecentBlend(string blendFilePath)
        {
            //dont want to show one file many times
            if (_recentBlends.Contains(blendFilePath)) {
                _recentBlends.Remove(blendFilePath);
            }

            //delete last if the list is larger than _LAST_BLENDS_MAX_COUNT
            if (_recentBlends.Count == _RECENT_BLENDS_MAX_COUNT)
            {
                _recentBlends.RemoveAt(_RECENT_BLENDS_MAX_COUNT - 1);
            }
            _recentBlends.Insert(0, blendFilePath);
        }

        public void clearRecentBlend()
        {
            if (_recentBlends.Count > 0)
            {
                var response = System.Windows.Forms.MessageBox.Show(
                                 "This will clear all files in the recent blends list, are you sure?", "Clear recent blends?",
                                 MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                switch (response)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        _recentBlends.Clear();
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show("Something wrong happend.");
                        break;
                }
            }
            else
                return;
        }

        public void checkCorrectConfig(bool showErrors = true)
        {
            List<string> errors = new List<string>();
            _appConfigured = false;

            if (!checkBlenderPath())
            {
                errors.Add(AppErrorCodes.BLENDER_PATH_NOT_SET);
            }

            if (!checkFFmpegPath())
            {
                errors.Add(AppErrorCodes.FFMPEG_PATH_NOT_SET);
            }

            if (errors.Count == 0)
            {
                _appConfigured = true;
                return;
            }

            if (showErrors)
            {
                Helper.showErrors(errors);
            }
        }

        public bool checkBlenderPath()
        {
            return File.Exists(Path.Combine(_blenderPath, BLENDER_EXE_NAME));
        }

        public bool checkFFmpegPath()
        {
            return File.Exists(Path.Combine(_ffmpegPath, FFMPEG_EXE_NAME));
        }
        public List<string> recentBlends
        {
            get {
                return _recentBlends;
            }
        }

        public string audioFormat
        {
            get
            {
                return _audioFormat;
            }
        }

        public string chunksTxtFileName
        {
            get
            {
                return _chunksTxtFileName;
            }
        }

        public string[] allowedFormats
        {
            get
            {
                return _allowedFormats;
            }
        }

        public int processCheckInterval
        {
            get
            {
                return _processCheckInterval;
            }
        }

        public string scriptsPath
        {
            get
            {
                return _scriptsPath;
            }
        }

        public string chunksSubfolder
        {
            get
            {
                return _chunksSubfolder;
            }
        }

        public decimal processCount
        {
            get
            {
                return _processCount;
            }

            set
            {
                _processCount = value;
            }
        }
        public bool appConfigured
        {
            get
            {
                return _appConfigured;
            }
        }
        public string blenderPath
        {
            get
            {
                return _blenderPath;
            }
            set
            {
                _blenderPath = value;
            }
        }
        public string renderer
        {
            get
            {
                return _renderer;
            }
            set
            {
                _renderer = value;
            }
        }
        public string ffmpegPath
        {
            get
            {
                return _ffmpegPath;
            }
            set
            {
                _ffmpegPath = value;
            }
        }
        public SettingsForm settingsForm
        {
            set
            {
                _settingsForm = value;
            }
        }
        public decimal chunkLength
        {
            get
            {
                return _chunkLength;
            }
            set
            {
                _chunkLength = value;
            }
        }

        public string afterRenderAction
        {
            get
            {
                return _afterRenderAction;
            }

            set
            {
                _afterRenderAction = value;
            }
        }

        public bool displayTooltips
        {
            get
            {
                return _displayTooltips;
            }

            set
            {
                _displayTooltips = value;
            }
        }

        public bool verboseLog
        {
            get { return _verboseLog; }
            set { _verboseLog = value; }
        }
    }
}
