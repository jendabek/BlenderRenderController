import os
import json
import bpy
import math


class ProjectInfo:
    

    # get number of Scenes and active scene name
    scene = bpy.context.scene
    scenes = bpy.data.scenes


    # requests info from blender
    def get_info(self):

        print("Requesting infos...")

        blendPath = bpy.context.blend_data.filepath;
        projectName  = bpy.path.display_name_from_filepath( blendPath );

        # get values from strings
        sceneActive = str(self.scene).partition('("')[-1].rpartition('")')[0]

        scene = self.scene

        # set infos acording to active Scene
        start = scene.frame_start
        end   = scene.frame_end
        imgFormat = scene.render.image_settings.file_format
        resolutionPercentage = scene.render.resolution_percentage
        resolution = "{0} x {1}".format(math.floor(scene.render.resolution_x * resolutionPercentage / 100),
                                        math.floor(scene.render.resolution_y * resolutionPercentage / 100))
        
        # ffmpeg info
        ffmpegFmt = scene.render.ffmpeg.format
        ffmpegCodec = scene.render.ffmpeg.codec
        ffmpegAudio = scene.render.ffmpeg.audio_codec

        # calc real fps
        fpsSource  = scene.render.fps
        fpsBase = scene.render.fps_base
        fps = fpsSource / fpsBase

        # convert output path to absolute
        # make sure path separator is constant
        output = bpy.path.abspath(scene.render.filepath)
        outputPath = bpy.path.native_pathsep(output) 

        # split and see if it needs fixing (one of the elements is '..')
        outSplit = outputPath.split(os.sep)
        
        if ".." in outSplit:
            print("Path has relative folders '/../'")
            print("Before: " + output)
            outputPath = self.fix_path(outSplit)
            print("After: " + outputPath, end='\n\n')

        print("Building data...")

        return {
                'projectName': projectName,
                'blendPath': blendPath,
		        'start': start,
		        'end': end,
		        'fps': fps,
                'resolution': resolution,
		        'outputPath': outputPath,
		        'sceneActive': sceneActive,
		        'imgFormat': imgFormat,
                'ffmpegFmt': ffmpegFmt,
                'ffmpegCodec': ffmpegCodec,
                'ffmpegAudio': ffmpegAudio
            };


    # fixes relative paths
    def fix_path(self, path):

        print("Fixing output path...")
        relIndexes = [i for i, x in enumerate(path) if x == ".."]
        foldersToDel = []
        
        count = 0
        for idx in relIndexes:
            count += 1
            folderIdx = idx - count
            
            if folderIdx not in foldersToDel:
                foldersToDel.append(folderIdx)
                count += 1


        if len(foldersToDel) > 0:
            relIndexes = list(set(relIndexes + foldersToDel))

        relIndexes.sort()

        print("Deleting indexes: " + str(relIndexes))

        # removes elemets from largest index down
        for i in relIndexes[::-1]:
            del path[i]

        outputAbs = os.sep.join(path)
        return outputAbs



if __name__ == "__main__":
    p = ProjectInfo()
    jsonStr = p.get_info()
    print(json.dumps(jsonStr, indent=4, skipkeys=True, sort_keys=True))
