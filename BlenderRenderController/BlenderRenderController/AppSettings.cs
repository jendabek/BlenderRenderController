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

namespace BlenderRenderController
{

    public class AppSettings
    {

        private List<string> _lastBlends = new List<string>();
        private int _LAST_BLENDS_MAX_COUNT = 10;

        private string _jsonFileName = "settings.json";
        private string _chunksSubfolder = "chunks";
        private string _scriptsSubfolder = "scripts";
        private string _audioFormat = "ac3";
        private string _chunksTxtFileName = "partList.txt";
        private decimal _processCount = 4;
        private string[] _allowedFormats = { "avi", "mp4", "mov", "mkv", "mpg", "flv" };
        private string[] _jsonProperties = { "lastBlends", "processCount" };
        private string _scriptsPath;
        private int _processCheckInterval = 100;


        public AppSettings()
        {
            //LOADing data from JSON and set it to properties
            //-----------------------
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _jsonFileName);
            _scriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _scriptsSubfolder);

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
                    catch(Exception) {
                        return;
                    }

                    //converting ArrayList object from json to List
                    if (jsonSettings[propertyName] is ArrayList)
                    {

                        ArrayList arrayList = (ArrayList) jsonSettings[propertyName];
                        List<string> listValue = arrayList.Cast<string>().ToList();
                        GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, listValue);
                    }
                    //converting Int32 to decimal
                    else if (jsonSettings[propertyName] is Int32)
                    {
                        decimal decimalValue = (decimal)(int)jsonSettings[propertyName];
                        GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, decimalValue);
                    }
                }

            }
        }
        public List<string> getRequirementsResult()
        {
            List<string> errors = new List<string> {};
            //List<string> errors = new List<string> { Requirements.BLENDER_PATH_NOT_SET, Requirements.FFMPEG_PATH_NOT_SET };
            return errors;
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

        public void addToLastBlends(string blendFilePath)
        {
            if(_lastBlends.Count == _LAST_BLENDS_MAX_COUNT)
            {
                _lastBlends.RemoveAt(_LAST_BLENDS_MAX_COUNT - 1);
            }
            _lastBlends.Insert(0, blendFilePath);
        }
    }
}
