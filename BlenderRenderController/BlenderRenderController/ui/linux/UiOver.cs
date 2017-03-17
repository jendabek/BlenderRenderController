using System;

namespace BlenderRenderController
{
    /*
     Ui override
   */

    partial class MainForm
    {
		void PlatAdjust(PlatformID os)
		{
			// exec ajustments for the selected platform
			switch (os)
			{
                case PlatformID.Win32NT:
                    WinIconFix();
                    break;
			    case PlatformID.MacOSX:
				    // future Mac Ajustments?
				    //break;
			    case PlatformID.Unix:
				    UiUnix();
				    break;
			    default:
				    break;
			}
		}

        private void UiUnix()
        {
			this.ClientSize = 
			this.MaximumSize = new System.Drawing.Size (780, 640);

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
        void PlatAdjust(PlatformID os)
        {
            // exec ajustments for the selected platform
            switch (os)
            {
                case PlatformID.Win32NT:
                    break;
			case PlatformID.MacOSX:
                // future Mac Ajustments?
                //break;
			case PlatformID.Unix:
				UiUnix ();
				//SetUnixIcon ();
                    break;
                default:
                    break;
            }
        }

        private void UiUnix()
        {
            blenderPathTextBox.BackColor = 
            ffmpegPathTextBox.BackColor = System.Drawing.Color.White;

			this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            blenderChangePathButton.BackColor = 
            ffmpegChangePathButton.BackColor = 
            okButton.BackColor = System.Drawing.Color.FromArgb(225, 225, 225);
        }

        private void SetUnixIcon()
        {
			Type type = this.GetType();
			System.Resources.ResourceManager resources =
				new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);

			//this.Icon = (System.Drawing.Icon)resources.GetObject("blender_icon");
			this.Icon = (System.Drawing.Icon)resources.GetObject("blender_ic_linux");
        }

        
    }
}