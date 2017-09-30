# Blender Render Controller 

## Download &#8615;
Latest release [HERE](https://github.com/jendabek/BlenderRenderController/releases/latest).

BRC is a tool to speed up the rendering process of videos in Blender's Video Sequence editor by rendering multiple diferent chunks of the video at the same time and joining them toguether after.

Check the wiki for more details about using and installing / building BRC.

Any questions, bug reports or suggestions? [Let us know](https://github.com/jendabek/BlenderRenderController/issues)!


### Known Issues

- *"Fatal python error"* when opening projects in Windows 7
	- This is caused some incompatibility with Win7 UAC, runnig as administrator won't work either.
	- This will probably affect you if one of the required programs is installed in a protected folder (like "Program Files")
	- Work-around: Launch _BlenderRenderController.exe_ from the command line, you can download a convenient _.bat_ file [here](https://github.com/jendabek/BlenderRenderController/blob/master/BlenderRenderController/utilities/runWin7.bat).

- Older versions of FFmpeg may fail to join chunks if AAC audio is used ([link](https://trac.ffmpeg.org/wiki/Encode/AAC#NativeFFmpegAACencoder))

## CREDITS

- Isti115
- meTwentyFive
- RedRaptor93
- jendabek

## Support the development &#9829;
<a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=jendabek@gmail.com&lc=CZE&item_name=Donation%20for%20Blender Render%20Controller&currency_code=USD&bn=PP%2dDonationsBF">
<img align="left" src="https://github.com/jendabek/BlenderRenderController/blob/master/extras/imgs/donate-github.png" width="110"/>
</a>
