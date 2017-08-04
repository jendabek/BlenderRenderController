using System;

namespace BlenderRenderController
{

    /*
      Ui override
   */


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
}
