using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController.newLogger
{
    public class FileLogger : LoggerBase, ILogger
    {
        //private string _lastLog;
        //private string _repeatLine;
        //private int _numLastLogs;

        public FileLogger()
        {
            Name = nameof(FileLogger);
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

    public class ConsoleLogger : LoggerBase, ILogger
    {
        public ConsoleLogger()
        {
            Name = nameof(ConsoleLogger);
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

    public class LoggerBase
    {
        protected readonly DateTime _time = DateTime.Now;
        protected bool _verbose;
        protected AppSettings appSettings = new AppSettings();
        public string Name { get; set; }

        public LoggerBase()
        {
            appSettings.RemoteLoadJsonSettings();
            this._verbose = appSettings.verboseLog;
        }
    }
}

//if (logMode == 0)
//{
//    var logTitle = $"\n{LogConstants.LOG_TITLE} - {AppDomain.CurrentDomain.FriendlyName}\n";
//    sw.WriteLine(logTitle);
//    logMode++;
//}
