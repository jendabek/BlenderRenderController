using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlenderRenderController
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
}
