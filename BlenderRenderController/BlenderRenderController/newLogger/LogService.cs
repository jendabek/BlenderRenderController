using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController.newLogger
{
    static class oldLogger
    {
        private static AppSettings appSettings;
        private const string LOG_FILE_PATH = "log.txt";

        public static void init(AppSettings appSettingsInstance)
        {
            appSettings = appSettingsInstance;
        }
        public static void add(string line)
        {
            using (StreamWriter sw = File.AppendText(LOG_FILE_PATH))
            {
                sw.WriteLine(line);
            }
        }
    }

    public class LogService
    {
        private readonly IList<ILogger> _loggerServices = new List<ILogger>();

        public enum LogType
        {
            INFO, ERROR, WARNING
        }

        public void RegisterLogSevice(ILogger service)
        {
            _loggerServices.Add(service);
        }

        public void Error(string message)
        {
            foreach (var service in _loggerServices)
                service.LogError(message);
        }

        public void Warning(string message)
        {
            foreach (var service in _loggerServices)
                service.LogWarn(message);
        }

        public void Info(string message)
        {
            foreach (var service in _loggerServices)
                service.LogInfo(message);
        }

    }
}
