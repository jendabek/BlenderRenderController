import os
import json
import bpy
import re
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
fps  = bpy.data.scenes[sceneActive].render.fps / bpy.data.scenes[sceneActive].render.fps_base
outputPath = bpy.data.scenes[sceneActive].render.filepath

#altDir = str(outputPath).rpartition('\\')[:-1][0]

data = {'projectName': projectName,
		'start': start,
		'end': end,
		'fps': fps,
		'outputPath': outputPath,
        'scenesNum': scenesNum,
		'sceneActive': sceneActive
};

jsonData = json.dumps(data, indent=4, skipkeys=True, sort_keys=True);

#with open('settings.json', 'w') as f:
#    print(jsonData, file=f)

print(jsonData);

