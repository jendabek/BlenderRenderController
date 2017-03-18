using System;
using System.Diagnostics;

namespace BlenderRenderController
{
    /*
     Ui override
   */

    partial class MainForm
    {
		private void PlatAdjust(PlatformID os)
		{
            // diferenciate between MacOs and Linux
            PlatformID _plat;
            if (os != PlatformID.Win32NT)
            {
                var detectOS = new OsDetection();
                _plat = detectOS.LinuxOrMac();
            }
            else
                _plat = os;

			// exec ajustments for the selected platform
			switch (_plat)
			{
                case PlatformID.Win32NT:
                    WinIconFix();
                    break;
			    case PlatformID.MacOSX:
                    UiColors();
                    break;
			    case PlatformID.Unix:
                    this.ClientSize =
                    this.MaximumSize = new System.Drawing.Size(780, 640);
                    UiColors();
				    break;
			    default:
				    break;
			}
		}

        private void UiColors()
        {

            totalEndNumericUpDown.BackColor = 
            totalStartNumericUpDown.BackColor =
            chunkLengthNumericUpDown.BackColor =
            processCountNumericUpDown.BackColor =
            outputFolderTextBox.BackColor = System.Drawing.Color.White;

            this.BackColor = 
            infoActiveScene.BackColor =
            infoDuration.BackColor =
            infoFramerate.BackColor =
            infoFramesTotal.BackColor =
            infoResolution.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

			renderAllButton.BackColor = 
			concatenatePartsButton.BackColor = 
            blendFileBrowseButton.BackColor =
            reloadBlenderDataButton.BackColor =
            outputFolderBrowseButton.BackColor =
            mixDownButton.BackColor =
            openOutputFolderButton.BackColor =
            donateButton.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);

        }

        void WinIconFix()
        {
            // sets the icon for Windows systems outside of MainFormDesigner
            Type type = this.GetType();
            System.Resources.ResourceManager resources =
            new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);

            this.Icon = (System.Drawing.Icon)resources.GetObject("blender_icon");
            this.ShowIcon = true;
        }
    }

    partial class SettingsForm
    {
        private void PlatAdjust(PlatformID os)
        {
            // exec ajustments for the selected platform
            switch (os)
            {
                case PlatformID.Win32NT:
                    break;
		    	case PlatformID.MacOSX:
			    case PlatformID.Unix:
			    	UiColors();
                    break;
                default:
                    break;
            }
        }

        private void UiColors()
        {
            blenderPathTextBox.BackColor = 
            ffmpegPathTextBox.BackColor = System.Drawing.Color.White;

			this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            blenderChangePathButton.BackColor = 
            ffmpegChangePathButton.BackColor = 
            okButton.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
        }

    }

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