using System;
using System.Diagnostics;
using System.Windows.Forms;

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
                    Os = PlatformID.Win32NT;
                    break;
                case PlatformID.MacOSX:
                    UiColors();
                    Os = PlatformID.MacOSX;
                    break;
                case PlatformID.Unix:
                    this.ClientSize =
                    this.MaximumSize = new System.Drawing.Size(780, 640);
                    UiColors();
                    BlendBrowseUnix();
                    Os = PlatformID.Unix;
                    break;
                default:
                    break;
            }

            logger.Info($"OS is {Os}");
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

            blendBrowseOver.BackColor =
            renderAllButton.BackColor =
            concatenatePartsButton.BackColor =
            blendFileBrowseButton.BackColor =
            reloadBlenderDataButton.BackColor =
            outputFolderBrowseButton.BackColor =
            mixDownButton.BackColor =
            openOutputFolderButton.BackColor =
            donateButton.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);


            renderAllButton.FlatStyle =
            concatenatePartsButton.FlatStyle =
            blendFileBrowseButton.FlatStyle =
            reloadBlenderDataButton.FlatStyle =
            outputFolderBrowseButton.FlatStyle =
            mixDownButton.FlatStyle =
            openOutputFolderButton.FlatStyle =
            donateButton.FlatStyle = FlatStyle.Flat;

        }

        void WinIconFix()
        {
            // sets the icon for Windows systems outside of MainFormDesigner
            // required to make project build on Mono Develop
            Type type = this.GetType();
            System.Resources.ResourceManager resources =
            new System.Resources.ResourceManager(type.Namespace + ".Properties.Resources", this.GetType().Assembly);

            this.Icon = (System.Drawing.Icon)resources.GetObject("blender_icon");
            this.ShowIcon = true;
        }

        void BlendBrowseUnix()
        {
            // alternative layout for linux

            this.blendFileBrowseButton.Visible = false;

            this.blendBrowseOver.Visible = true;
            blendBrowseOver.ButtonClicked += BlendBrowseFiles_Clicked;
        }

        private void BlendBrowseFiles_Clicked(object sender, EventArgs e)
        {
            blendFileBrowseButton_Click(sender, e);
        }

        void BlendBrowseOverBUILD()
        {
            // blendBrowseOver
            // getting infos from blendFileBrowseButton
            Button b = blendFileBrowseButton;

            this.blendBrowseOver.AutoSizeMode = b.AutoSizeMode;
            this.blendBrowseOver.BackColor = b.BackColor;
            this.blendBrowseOver.Location = b.Location;
            this.blendBrowseOver.MainBnt = "  Open Blend";
            this.blendBrowseOver.MainImg = blendFileBrowseButton.Image;
            this.blendBrowseOver.Margin = new System.Windows.Forms.Padding(0);
            this.blendBrowseOver.Name = "blendBrowseOver";
            this.blendBrowseOver.SecondaryBnt = "˅";
            this.blendBrowseOver.Size = new System.Drawing.Size(172, 47);
            this.blendBrowseOver.TabIndex = 1;

        }
    }
}

