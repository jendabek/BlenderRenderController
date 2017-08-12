# Blender Render Controller - Beta branch

#### Beta notes:
- Massive code revision/rewrite to make it easier to work w/ in the future
- Core logic now resides in BRClib
- No Linux/Mono support, yet

## What is this?
<img align="right" src="https://github.com/jendabek/BlenderRenderController/blob/master/BlenderRenderController/extras/blender-render-controller.png" width="480"/>
Blender Render Controller is a tool to help speed up the rendering of Blender's Video Sequence Editor (VSE) projects.

VSE is pretty good for editing videos, it's precise and relatively easy to learn, making it a compelling choice next to other free video editing tools. There are some downsides too, main of which been that the renderer is **single threaded**. Meaning that it won't take full advantage of all logical cores in your system, so rendering your finished project is **super slow** compared to other video editors.

This tool offers a work-around for this limitation until the Blender developers make a better renderer for VSE. 

It renders different segments (chunks) of the project at the same time by calling multiple blender.exe instances, **making use of full processing power of your PC**.
After all parts are rendered, they're joined together in FFmpeg, and your **video is ready much faster** then previously possible.

### Video demonstration
[<img src="https://github.com/jendabek/BlenderRenderController/blob/master/BlenderRenderController/extras/intro-720.png" width="480"/>](https://www.youtube.com/watch?v=Kdvq1CzOPfM)

## How much difference does it make?
**Quite a lot!** I did some testing shown below (Blender Render Controller shown in orange):

![Test3](https://github.com/RedRaptor93/BlenderRenderController/blob/multiPlat/BlenderRenderController/extras/brc%20graph%201080p.png)

![Test1](https://github.com/RedRaptor93/BlenderRenderController/blob/multiPlat/BlenderRenderController/extras/brc%20graph%20720p.png)

PC used: i7 4790, 16GB DDR3 RAM @ 1600Mhz

Really shows the importance of those extra cores huh? And of course, more processor cores = bigger difference.
Even if you don't use Blender VSE often, that’s a LOT of time saved. And the time added by joining the videos together is negligible (less then 1min).

## HOW TO USE

### Dependencies
- Blender, obviously.
- [FFmpeg](https://ffmpeg.zeranoe.com/builds/), required for joining the parts together. You don't need to worry about it if you download the Full version which has FFmpeg already included.
- .NET framework 4.5

### Steps
1. Create your Blender VSE project normally within Blender.
 
2. Open BlenderRenderController, browse for the .blend file.
 
3. BRC will automatically calculate the *Start Frame*, *End Frame* and *Chunk Size* according to the length of the project and number of logical cores in your CPU respectively, you can change these values manually if you want.

	- Tip: For optimum performance, the N# of processes should match the N# of logical cores in you system.
 
4. Choose the render method:

	- *Automatically join chunks & use mixdown audio* - renders chunks, makes a separated audio file and joins it all together, recommended if you have audio tracks in your project.

	- *Automatically join chunks* - same as above, minus audio mixdown.

	- *Render just chunks* - just renders the Chunks.
 
5. Click *Start Render* and wait for the render to be done.

### Known Issues

- *"Fatal python error"* when opening projects in Windows 7
	- This is caused some incompatibility with Win7 UAC, runnig as administrator won't work either.
	- This will probably affect you if one of the required programs is installed in a protected folder (like "Program Files")
	- Work-around: Launch _BlenderRenderController.exe_ from the command line, you can download a convenient _.bat_ file [here](https://github.com/jendabek/BlenderRenderController/blob/master/BlenderRenderController/utilities/runWin7.bat).

- If the output of your blend project is set to a location one folder below the project's location AND your using RELATIVE paths, BRC won't be able to parse it and it'll set the output to the blend file location. Use absolute paths to avoid this issue or change the path once loaded.

## CREDITS

- Isti115
- meTwentyFive
- RedRaptor93

## Support the development &#9829;
<a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=jendabek@gmail.com&lc=CZE&item_name=Donation%20for%20Blender Render%20Controller&currency_code=USD&bn=PP%2dDonationsBF">
<img align="left" src="https://github.com/jendabek/BlenderRenderController/blob/master/BlenderRenderController/extras/donate-github.png" width="110"/>
</a>
