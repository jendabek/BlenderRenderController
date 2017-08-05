using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController
{
    class AppSettings
    {
        private List<string> _recentBlends = new List<string>();
        private PlatformID Os = Environment.OSVersion.Platform;

        private string _blenderExeName;
        private string _ffmpegExeName;

        public List<string> RecentBlends
        {
            get => _recentBlends;
            private set => _recentBlends = value;
        }

        public string BlenderProgram { get; set; }
        public string FFmpegProgram { get; set; }

        public AppSettings()
        {
            _ffmpegExeName = GetProgramFileName("ffmpeg");
            _blenderExeName = GetProgramFileName("blender");
        }

        public void ClearRecentBlend()
        {
            if (_recentBlends.Count > 0)
            {
                var response = MessageBox.Show(
                                 "This will clear all files in the recent blends list, are you sure?", 
                                 "Clear recent blends?",
                                 MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                switch (response)
                {
                    case DialogResult.OK:
                        _recentBlends.Clear();
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        MessageBox.Show("Something wrong happend.");
                        break;
                }
            }
        }

        private string GetProgramFileName(string prog)
        {
            switch (Os)
            {
                case PlatformID.Win32NT:
                    return prog + ".exe";
                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return prog;
                default:
                    return prog;
            }
        }
    }
}
