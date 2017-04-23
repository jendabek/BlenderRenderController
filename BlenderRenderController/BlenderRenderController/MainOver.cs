using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BlenderRenderController
{
    public partial class MainOver : BlenderRenderController.MainForm
    {
        public MainOver()
        {
            InitializeComponent();
        }

        private void PlatAdjust(PlatformID os)
        {
            // diferenciate between MacOs and Linux
            if (os == PlatformID.Unix)
            {
                var detectOS = new OsDetection();
                os = detectOS.LinuxOrMac();
            }

            // exec ajustments for the selected platform
            switch (os)
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
}
