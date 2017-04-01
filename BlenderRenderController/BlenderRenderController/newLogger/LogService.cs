using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController.newLogger
{

    public class LogService : ILogger
    {
        private readonly IList<ILogger> _loggerServices = new List<ILogger>();

        // Singleton implementation
        private static readonly Lazy<LogService> lazy = new Lazy<LogService>(() => new LogService());
        public static LogService Log { get => lazy.Value; }

        public void RegisterLogSevice(ILogger service)
        {
            if (service == null)
                throw new ArgumentNullException(service.ToString() ,"Log service passed is null.");

            if (!_loggerServices.Contains(service))
                // avoid duplicates services
                _loggerServices.Add(service);
        }

        // Interface stuff
        public void Error(string message)
        {
            foreach (var service in _loggerServices)
                service.Error(message);
        }
        public void Error(List<string> messages)
        {
            foreach (var msg in messages)
                this.Error(msg);
        }

        public void Info(string message)
        {
            foreach (var service in _loggerServices)
                service.Info(message);
        }
        public void Info(List<string> messages)
        {
            foreach (var msg in messages)
                this.Info(msg);
        }

        public void Warn(string message)
        {
            foreach (var service in _loggerServices)
                service.Warn(message);
        }
        public void Warn(List<string> messages)
        {
            foreach (var msg in messages)
                this.Warn(msg);
        }
    }
}
