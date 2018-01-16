# Blender Render Controller 

## Download &#8615;
Latest release [HERE](https://github.com/jendabek/BlenderRenderController/releases/latest).

BRC is a tool to speed up the rendering process of videos in Blender's Video Sequence editor by rendering multiple diferent chunks of the video at the same time and joining them toguether after.

Check the wiki for more details about using and installing / building BRC.

Any questions, bug reports or suggestions? [Let us know](https://github.com/jendabek/BlenderRenderController/issues)!


### Known Issues

- *"Fatal python error"* when opening projects in Windows 7
	- Blender crashes when BRC requests project info.
	- Work-around: Launch _BlenderRenderController.exe_ from the command line, you can get a convenient _.bat_ file [here](https://github.com/jendabek/BlenderRenderController/blob/master/extras/utilities/runWin7.bat).

- Older versions of FFmpeg may fail to join chunks if AAC audio is used ([link](https://trac.ffmpeg.org/wiki/Encode/AAC#NativeFFmpegAACencoder))

- Ui may not update progress properly on Linux

## CREDITS

- Isti115
- meTwentyFive
- RedRaptor93
- jendabek


## Support the development &#9829;
<a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=9SGQVK6TK2UJG&lc=US&item_name=Donation%20for%20Blender%20Render%20Controller&item_number=BRC&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted">
<img align="left" src="https://github.com/jendabek/BlenderRenderController/blob/master/extras/imgs/donate-github.png" width="110"/>
</a>
