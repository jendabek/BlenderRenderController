using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController.newLogger
{
    public class FileLogger : ILogger
    {
        private readonly DateTime _time = DateTime.Now;
        private bool _verbose;
        //private string _lastLog;
        //private string _repeatLine;
        //private int _numLastLogs;

        public FileLogger()
        {
            var appSettings = new AppSettings();
            appSettings.RemoteLoadJsonSettings();
            this._verbose = appSettings.verboseLog;
            //this._repeatLine = $"Last message repeted {_numLastLogs} times";
        }

        private void Log(string message, LogType logType)
        {
            // Ignore 'INFO' and 'WARN' logs if Verbose == false
            if ((!_verbose) && (logType != LogType.ERROR))
                return;

            string type = logType.ToString();

            using (StreamWriter sw = new StreamWriter(AppStrings.LOG_FILE_NAME, true))
            {
                var logLine = $"{type} [{_time}]: {message}";
                sw.WriteLine(logLine); 
            }

        }

        public void Info(string message)
        {
            Log(message, LogType.INFO);
        }

        public void Error(string message)
        {
            Log(message, LogType.ERROR);
        }

        public void Warn(string message)
        {
            Log(message, LogType.WARNING);
        }
    }

    public class ConsoleLogger : ILogger
    {
        private readonly DateTime _time = DateTime.Now;
        private bool _verbose;


        public ConsoleLogger()
        {
            var appSettings = new AppSettings();
            appSettings.RemoteLoadJsonSettings();
            this._verbose = appSettings.verboseLog;
        }

        private void Log(string message, LogType logType)
        {
            if ((!_verbose) && (logType == LogType.INFO))
                return;

            string type = logType.ToString();
            var logLine = $"{type}: {message} -- [{_time}]";
            Console.WriteLine(logLine);
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(message, LogType.ERROR);
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Log(message, LogType.INFO);
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(message, LogType.WARNING);
        }
    }

}

//if (logMode == 0)
//{
//    var logTitle = $"\n{LogConstants.LOG_TITLE} - {AppDomain.CurrentDomain.FriendlyName}\n";
//    sw.WriteLine(logTitle);
//    logMode++;
//}
