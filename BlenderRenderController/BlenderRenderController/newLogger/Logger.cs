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
        public bool IsActive { get; set; }

        public enum LogType
        {
            INFO, ERROR, WARNING
        }

        public FileLogger(bool isActive = false)
        {
            this.IsActive = isActive;
        }

        private void Log(string message, LogType logType)
        {
            if (!IsActive)
                return;

            string type = logType.ToString();

            using (StreamWriter sw = new StreamWriter(LogConstants.LOG_FILE_NAME, true))
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
    }

    public class ConsoleLogger : ILogger
    {
        private readonly DateTime _time = DateTime.Now;

        public bool IsActive{ get; set; }

        public enum LogType
        {
            INFO, ERROR, WARNING
        }

        private void Log(string message, LogType logType)
        {
            if (!IsActive)
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
    }

    static class LogConstants
    {
        public const string LOG_FILE_NAME = "log.txt";

        public const string LOG_TITLE = "Red's Logger";
        public const string STOPWATCH = "[STOPWATCH]";
        public const string MENU_CHOICES = "[MENU_CHOICES]";
    }
}

//if (logMode == 0)
//{
//    var logTitle = $"\n{LogConstants.LOG_TITLE} - {AppDomain.CurrentDomain.FriendlyName}\n";
//    sw.WriteLine(logTitle);
//    logMode++;
//}
