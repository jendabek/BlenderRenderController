import os
import json
import bpy
import re
import math
from bpy import context
from bpy import ops
from bpy import data

blendPath = bpy.context.blend_data.filepath;
projectName  = bpy.path.display_name_from_filepath( blendPath );

# get number of Scenes and active scene name
scenes = bpy.data.scenes
scene = bpy.context.scene

# get values from strings
scenesNum = str(scenes).partition('[')[-1].rpartition(']')[0]
sceneActive = str(scene).partition('("')[-1].rpartition('")')[0]

# set infos acording to active Scene
start = bpy.data.scenes[sceneActive].frame_start
end   = bpy.data.scenes[sceneActive].frame_end
fps  = bpy.data.scenes[sceneActive].render.fps
fpsBase = bpy.data.scenes[sceneActive].render.fps_base
outputPath = bpy.data.scenes[sceneActive].render.filepath
renderFormat = bpy.data.scenes[sceneActive].render.image_settings.file_format
resolutionPercentage = bpy.data.scenes[sceneActive].render.resolution_percentage
resolution = "{0} x {1}".format(math.floor(bpy.data.scenes[sceneActive].render.resolution_x * resolutionPercentage / 100),
                              math.floor(bpy.data.scenes[sceneActive].render.resolution_y * resolutionPercentage / 100))

data = {'projectName': projectName,
		'start': start,
		'end': end,
		'fps': fps,
		'fpsBase': fpsBase,
        'resolution': resolution,
		'resolutionPercentage': resolutionPercentage,
		'outputPath': outputPath,
        'scenesNum': scenesNum,
		'sceneActive': sceneActive,
		'renderFormat': renderFormat
};

jsonData = json.dumps(data, indent=4, skipkeys=True, sort_keys=True);

#with open('settings.json', 'w') as f:
#    print(jsonData, file=f)

print(jsonData);