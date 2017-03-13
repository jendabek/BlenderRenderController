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
        public bool Verbose { get; set; }

        public enum LogType
        {
            INFO, ERROR, WARNING
        }

        public FileLogger(bool verbose = false)
        {
            this.Verbose = verbose;
        }

        private void Log(string message, LogType logType)
        {
            // Ignore 'INFO' logs if Verbose == false
            if ((!Verbose) && (logType != LogType.ERROR))
                return;

            string type = logType.ToString();

            using (StreamWriter sw = new StreamWriter(LogsConstants.LOG_FILE_NAME, true))
            {
                var logLine = $"{type}: {message} -- [{_time}]\n";
                sw.WriteLine(logLine); 
            }

        }

        public void LogInfo(string message)
        {
            Log(message, LogType.INFO);
        }

        public void LogError(string message)
        {
            Log(message, LogType.ERROR);
        }

        public void LogWarn(string message)
        {
            Log(message, LogType.WARNING);
        }
    }

    public class ConsoleLogger : ILogger
    {
        private readonly DateTime _time = DateTime.Now;

        public bool Verbose{ get; set; }

        public enum LogType
        {
            INFO, ERROR, WARNING
        }

        private void Log(string message, LogType logType)
        {
            if ((!Verbose) && (logType == LogType.INFO))
                return;

            string type = logType.ToString();
            var logLine = $"{type}: {message} -- [{_time}]\n";
            Console.WriteLine(logLine);
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(message, LogType.ERROR);
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Log(message, LogType.INFO);
        }

        public void LogWarn(string message)
        {
            throw new NotImplementedException();
        }
    }

}

//if (logMode == 0)
//{
//    var logTitle = $"\n{LogConstants.LOG_TITLE} - {AppDomain.CurrentDomain.FriendlyName}\n";
//    sw.WriteLine(logTitle);
//    logMode++;
//}
