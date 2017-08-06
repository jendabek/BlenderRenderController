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
        private int RECENT_BLENDS_MAX_COUNT = 10;

        public string BlenderProgram { get; set; }
        public string FFmpegProgram { get; set; }
        public AppStates State { get; set; }
        public List<string> RecentBlends { get => _recentBlends; }

        public AppSettings()
        {
            _ffmpegExeName = GetProgramFileName("ffmpeg");
            _blenderExeName = GetProgramFileName("blender");
        }

        public void AddRecentBlend(string blendFilePath)
        {
            //dont want to show one file many times
            if (_recentBlends.Contains(blendFilePath))
            {
                _recentBlends.Remove(blendFilePath);
            }

            //delete last if the list is larger than _LAST_BLENDS_MAX_COUNT
            if (_recentBlends.Count == RECENT_BLENDS_MAX_COUNT)
            {
                _recentBlends.RemoveAt(RECENT_BLENDS_MAX_COUNT - 1);
            }
            _recentBlends.Insert(0, blendFilePath);
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
