using System;
using System.Diagnostics;

namespace BlenderRenderController
{
    public class OsDetection
    {
        const string MACOS = "Darwin";
        const string LINUX = "Linux";

        public PlatformID LinuxOrMac()
        {
            Process p = new Process();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "uname";

            p.Start();

            string ver = p.StandardOutput.ReadLine();
            p.WaitForExit();

            if (ver == MACOS)
                return PlatformID.MacOSX;

            return PlatformID.Unix;
        }
    }
}
