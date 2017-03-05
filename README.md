# Blender Render Controller, jendabek ver.

## What is this?
Blender Render Controller is a tool to help speed up the render process in Blender's Video Sequence Editor (VSE).

VSE is pretty good for editing videos, it's precise and relatively easy to learn, making it a compelling choice next to other free video editing tools. There are some downsides too, main of which been that the renderer is SINGLE THREADED. Meaning that it won't take full advantage of all logical cores in your system, so rendering your finished project is SUPER SLOW compared to other video editors.

This tool offers a work-around until the Blender developers make a better renderer for VSE. 

This tool offers a work-around by calling multiple instances of `blender.exe`, each rendering a different segments (chunks) of the project at the same time, making use of processing power that would otherwise go unused. After all parts are rendered, they're joined together in FFmpeg and BAM, your video is ready much faster then previously possible.

## How much difference does it make?
Quite a lot! I did some testing shown below (Blender Render Controller shown in orange):

![Test3](https://app.box.com/representation/file_version_147671500287/image_2048/1.png?shared_name=u90snyjbzslz0zszwges1helzmyz6b8y)

![Test1](https://app.box.com/representation/file_version_147672318497/image_2048/1.png?shared_name=i1bwfn03tie6ieehwnz7mbp4lu700gzy)

PC used: i7 4790, 16GB DDR3 RAM @ 1600Mhz

Really shows the importance of those extra cores huh? Even if you don't use Blender VSE often, that’s a LOT of time saved. And the time added by joining the videos together is negligible (less then 1min).

## HOW TO USE

### Dependencies
- Blender, obviously
- FFmpeg, required for joining the parts together. You don't need to care about this if you download Full version which has FFmpeg already included.'

1. Save your .blend file with the settings you want (output path, resolution, etc)
 
2. Open BlenderRenderController, browse for the desired .blend file.
 
3. BRC will automatically calculate the Start frame, End frame and Chuck size according to the time length of the project and number of processes respectively, you can change these values manually if you want.

	- Tip: For optimum performance, the N# of processes should match the N# of logical cores in you system.
 
4. Choose the render method:

	- ´Automatically join chunks && use mixdown audio´ Renders chunks, makes a separated audio file and Joins it all together, recommended if you have audio tracks in your project

	- ´Automatically join chunks´ Same as above, minus audio mixdown.

	- ´Render just chunks´ Just renders the Chunks.
 
5. Click “Start Render” and wait for the render to be done.

## CREDITS

- Isti115
- meTwentyFive
- redRaptor93
- jendabek
