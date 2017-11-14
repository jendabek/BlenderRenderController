using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Common;
using System.Windows.Forms;
using System.IO;

namespace BlenderRenderController
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            NlogSetup();

            Application.Run(new BrcForm());
        }


        static void NlogSetup()
        {
            if (AppSettings.Current.Verbose)
            {
                foreach (var rule in LogManager.Configuration.LoggingRules)
                {
                    rule.EnableLoggingForLevel(LogLevel.Info);
                }

                LogManager.ReconfigExistingLoggers();
            }
        }
    }
}
