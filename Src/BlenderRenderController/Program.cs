using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using BRClib.Scripts;
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
        static void Main(string[] args)
        {

            // parse args
            if (args.Contains("--gen"))
            {
                Console.WriteLine("Writing scripts to AppData folder...");
                string[] paths = 
                {
                    Shelf.EmbeddedScriptToDisk(Shelf.PyScript.GetProjectInfo),
                    Shelf.EmbeddedScriptToDisk(Shelf.PyScript.MixdownAudio),
                };

                Console.WriteLine(string.Join("\n", paths));

                return;
            }
            else if (args.Contains("--clear"))
            {
                Console.WriteLine("Clearing existing scripts...");
                Directory.Delete(Dirs.Scripts, true);
            }

            Services.Settings.InitSettings();

            NlogSetup();

            var cmdFile = args.Where(a => Path.GetExtension(a) == ".blend")
                              .FirstOrDefault();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            BrcForm form;

            if (cmdFile != null)
            {
                form = new BrcForm(cmdFile);
            }
            else
            {
                form = new BrcForm();
            }

            Application.Run(form);
        }


        static void NlogSetup()
        {
            var _sett = Services.Settings.Current;
            LogLevel lLvl;

            switch (_sett.LoggingLevel)
            {
                case 1: lLvl = LogLevel.Info; break;
                case 2: lLvl = LogLevel.Trace; break;
                default: return;
            }

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                rule.EnableLoggingForLevel(lLvl);
            }

            LogManager.ReconfigExistingLoggers();

        }

    }
}
