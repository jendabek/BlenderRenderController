using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace BlenderRenderController
{

    public class AppSettings
    {

        //these properties are stored (saved/loaded) in external JSON file
        //they overrides default values of this class
        private const int _LAST_BLENDS_MAX_COUNT = 10;
        public const string BLENDER_EXE_NAME = "blender.exe";
        public const string FFMPEG_EXE_NAME = "ffmpeg.exe";

        private string[] _jsonProperties = { "lastBlends", "processCount", "blenderPath", "ffmpegPath"};

        private string _blenderPathDefault = "C:\\Program Files\\Blender Foundation\\Blender";
        private string _ffmpegPathDefault = AppDomain.CurrentDomain.BaseDirectory;

        private List<string> _lastBlends = new List<string>();
        private string _jsonFileName = "settings.json";
        private string _chunksSubfolder = "chunks";
        private string _scriptsSubfolder = "scripts";
        private string _audioFormat = "ac3";
        private string _chunksTxtFileName = "partList.txt";
        private decimal _processCount = 4;
        private string[] _allowedFormats = { "avi", "mp4", "mov", "mkv", "mpg", "flv" };

        private string _scriptsPath, _blenderPath, _ffmpegPath;

        private int _processCheckInterval = 100;
        private bool _appConfigured = false;
        private SettingsForm _settingsForm;


        public void init()
        {
            //LOADing data from JSON and set it to properties
            //-----------------------
            _scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _scriptsSubfolder);
            _blenderPath = _blenderPathDefault;
            _ffmpegPath = _ffmpegPathDefault;
            loadJsonSettings();
            checkRequiredSettings();
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
                    catch (Exception)
                    {
                        return;
                    }

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

        public void lastBlendsAdd(string blendFilePath)
        {
            //dont want to show one file many times
            if (_lastBlends.IndexOf(blendFilePath) != -1) return;

            //delete last if the list is larger than _LAST_BLENDS_MAX_COUNT
            if (_lastBlends.Count == _LAST_BLENDS_MAX_COUNT)
            {
                _lastBlends.RemoveAt(_LAST_BLENDS_MAX_COUNT - 1);
            }
            _lastBlends.Insert(0, blendFilePath);
        }
        public void checkRequiredSettings()
        {
            List<string> errors = new List<string>();
            
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
            _appConfigured = false;
            Helper.showErrors(errors);
        }

        public bool checkBlenderPath()
        {
            if (File.Exists(Path.Combine(_blenderPath, BLENDER_EXE_NAME))) {
                return true;
            }
            if (File.Exists(Path.Combine(_blenderPathDefault, BLENDER_EXE_NAME))) {
                _blenderPath = _blenderPathDefault;
                save();
                return true;
            }
            return false;
        }

        public bool checkFFmpegPath()
        {
            if(File.Exists(Path.Combine(_ffmpegPath, FFMPEG_EXE_NAME)))
            {
                return true;
            }
            if (File.Exists(Path.Combine(_ffmpegPathDefault, FFMPEG_EXE_NAME)))
            {
                _ffmpegPath = _ffmpegPathDefault;
                save();
                return true;
            }
            return false;
        }
        public List<string> lastBlends
        {
            get {
                return _lastBlends;
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
    }
}
