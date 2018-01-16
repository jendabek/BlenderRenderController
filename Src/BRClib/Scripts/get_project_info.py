# Part of Blender Render Controller
# https://github.com/jendabek/BlenderRenderController
# Copyright 2017-present Pedro Oliva Rodrigues

import os
import json
import bpy
import math


class ProjectInfo:
    

    # get number of Scenes and active scene name
    scene = bpy.context.scene
    #scenes = bpy.data.scenes


    # requests info from blender
    def get_info(self):

        print("Requesting infos...")

        blendPath = bpy.context.blend_data.filepath;
        projectName  = bpy.path.display_name_from_filepath( blendPath );
        
        scene = self.scene

        # get ActiveScene
        sceneActive = str(scene).partition('("')[-1].rpartition('")')[0]

        # resolution
        res_p = scene.render.resolution_percentage
        resolution = "{0} x {1}".format(math.floor(scene.render.resolution_x * res_p / 100),
                                        math.floor(scene.render.resolution_y * res_p / 100))
        
        # calc real fps
        fps = scene.render.fps / scene.render.fps_base

        # convert output path to absolute
        output = bpy.path.abspath(scene.render.filepath)
        outputPath = ""

        # see if path needs fixing (one of the elements is '\..\')
        updir = os.sep + '..' + os.sep
        
        if output.find(updir) != -1:
            print("Path has relative folders '/../'")
            print("Before: " + output)
            outputPath = self.fix_path(output)
            print("After: " + outputPath, end='\n\n')

        print("Building data...")

        return {
                'projectName': projectName,
		        'sceneActive': sceneActive,
		        'start': scene.frame_start,
		        'end': scene.frame_end,
		        'fps': fps,
                'resolution': resolution,
		        'outputPath': outputPath,
		        'fileFormat': scene.render.image_settings.file_format,
                'ffmpegFmt': scene.render.ffmpeg.format,
                'ffmpegCodec': scene.render.ffmpeg.codec,
                'ffmpegAudio': scene.render.ffmpeg.audio_codec
            };


    # removes 'up' directories from a path str
    def fix_path(self, path):

        # create collection (pos, value)
        p_split = path.split(os.sep)
        idx_vals = [i for i in enumerate(p_split)]
        pi = []
        
        # find 'up' dirs
        dot_items = [x for x in idx_vals if x[1] == '..']
        dot_count = len(dot_items)
        
        # set range of indexes to ignore
        # ignore all 'up' dirs and the folders they represent
        del_start = dot_items[0][0] - dot_count
        del_end = dot_items[-1][0]
        del_range = list(range(del_start, del_end + 1))

        # build fixed path
        for info in idx_vals:
            val, ignore = info[1], info[0] in del_range
            
            if not ignore:
                pi.append(val)

        return os.sep.join(pi);



if __name__ == "__main__":
    p = ProjectInfo()
    jsonStr = p.get_info()
    print(json.dumps(jsonStr, indent=4, skipkeys=True))
