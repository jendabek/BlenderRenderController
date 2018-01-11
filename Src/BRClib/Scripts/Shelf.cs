using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace BRClib.Scripts
{
    using Res = BRClib.Properties.Resources;

    public static class Shelf
    {
        const string PyGetProjInfo = "get_project_info.py";
        const string PyMixdownAudio = "mixdown_audio.py";

        private static Dictionary<PyScript, ScriptPaths> _internalData =
            new Dictionary<PyScript, ScriptPaths>
            {
                [PyScript.GetProjectInfo] = ScriptPaths.Empty,
                [PyScript.MixdownAudio] = ScriptPaths.Empty
            };


        public static string GetProjectInfo
        {
            get
            {
                return GetScriptPath(PyScript.GetProjectInfo);
            }
            set
            {
                SetCustomScriptPath(PyScript.GetProjectInfo, value);
            }
        }

        public static string MixdownAudio
        {
            get
            {
                return GetScriptPath(PyScript.MixdownAudio);
            }
            set
            {
                SetCustomScriptPath(PyScript.MixdownAudio, value);
            }
        }



        private static void SetCustomScriptPath(PyScript script, string newPath)
        {
            var sp = _internalData[script];
            sp.Custom = newPath;
            _internalData[script] = sp;
        }


        private static string GetScriptPath(PyScript script)
        {
            var path = GetCustomPath(script) ?? GetDefaultPath(script, false);
            return path;
        }

        private static string GetDefaultPath(PyScript script, bool allowNull)
        {
            var sp = _internalData[script];

            if (sp.Default == null && !allowNull)
            {
                sp.Default = EmbeddedScriptToDisk(script);
            }

            _internalData[script] = sp;

            return sp.Default;
        }

        private static string GetCustomPath(PyScript script)
        {
            var custom = _internalData[script].Custom;
            return File.Exists(custom) ? custom : null;
        }

        public static string EmbeddedScriptToDisk(PyScript script, string dir = null)
        {
            var rsName = GetPyScriptFileName(script);
            return EmbeddedScriptToDisk(rsName, dir);
        }

        private static string EmbeddedScriptToDisk(string resourceName, string dir = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resPath = assembly.GetName().Name + ".Scripts." + resourceName;

            if (dir == null)
                dir = Path.GetTempPath();

            var file = Path.Combine(dir, resourceName);

            if (File.Exists(file))
            {
                return file;
            }

            using (var resStream = assembly.GetManifestResourceStream(resPath))
            using (var fileStream = File.OpenWrite(file))
            {
                string genHeader = "# " + Res.GenFileHeader + "\n";
                byte[] header = Encoding.UTF8.GetBytes(genHeader);

                fileStream.Write(header, 0, header.Length);

                // copy contents to file
                resStream.Seek(0, SeekOrigin.Begin);
                resStream.CopyTo(fileStream);
            }

            return file;
        }

        public enum PyScript
        {
            GetProjectInfo,
            MixdownAudio
        }

        private static string GetPyScriptFileName(PyScript script)
        {
            switch (script)
            {
                case PyScript.GetProjectInfo:
                    return PyGetProjInfo;
                case PyScript.MixdownAudio:
                    return PyMixdownAudio;
                default:
                    return null;
            }
        }

        struct ScriptPaths
        {
            public string Default;
            public string Custom;

            public static ScriptPaths Empty
            {
                get
                {
                    ScriptPaths sp;
                    sp.Default = null;
                    sp.Custom = null;

                    return sp;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                var sp = (ScriptPaths)obj;

                return this.Equals(sp);
            }

            public bool Equals(ScriptPaths sp)
            {
                return Default == sp.Default
                    && Custom == sp.Custom;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }


}
