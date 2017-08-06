import os
import json
import bpy
import re
import math
from bpy import context
from bpy import ops
from bpy import data


class ProjectInfo:
    
    def __init__(self, legacy = False):
        self.jsonData = []
        self.legacy = legacy


    # requests info and builds jsonData
    def getInfo(self):
        blendPath = bpy.context.blend_data.filepath;
        projectName  = bpy.path.display_name_from_filepath( blendPath );

        # get number of Scenes and active scene name
        scenes = bpy.data.scenes
        scene = bpy.context.scene

        # get values from strings
        scenesNum = str(scenes).partition('[')[-1].rpartition(']')[0]
        sceneActive = str(scene).partition('("')[-1].rpartition('")')[0]

        # set infos acording to active Scene
        start = scene.frame_start
        end   = scene.frame_end
        totalLength = end - start + 1
        renderFormat = scene.render.image_settings.file_format
        resolutionPercentage = scene.render.resolution_percentage
        resolution = "{0} x {1}".format(math.floor(scene.render.resolution_x * resolutionPercentage / 100),
                                      math.floor(scene.render.resolution_y * resolutionPercentage / 100))
        # calc real fps
        fpsSource  = scene.render.fps
        fpsBase = scene.render.fps_base
        fps = fpsSource / fpsBase

        # convert output path to absolute
        # make sure path separator is constant
        output = bpy.data.scenes[sceneActive].render.filepath
        outputPath = bpy.path.native_pathsep(bpy.path.abspath(output)) 

        # split and see if it needs fixing (one of the elements is '..')
        outSplit = outputPath.split(os.sep)
        
        if ".." in outSplit:
            outputPath = self.fixPath(outSplit)


        data = {
            'projectName': projectName,
            'blendPath': blendPath,
		    'start': start,
		    'end': end,
		    'fps': fps,
            'totalLength': totalLength,
            'resolution': resolution,
		    'outputPath': outputPath,
            'scenesNum': scenesNum,
		    'sceneActive': sceneActive,
		    'renderFormat': renderFormat
        };

        dataLeg = {
            'projectName': projectName,
		    'start': start,
		    'end': end,
		    'fps': fpsSource,
		    'fpsBase': fpsBase,
            'resolution': resolution,
		    'resolutionPercentage': resolutionPercentage,
		    'outputPath': outputPath,
            'scenesNum': scenesNum,
		    'sceneActive': sceneActive,
		    'renderFormat': renderFormat
        };

        if self.legacy == True:
            # old 0.8.2 format
            self.jsonData = dataLeg
        else:
            # new format to use in future versions
            self.jsonData = data


    # fixes relative paths
    def fixPath(self, path):
        print("Fixing output path...")
        relIndexes = [i for i, x in enumerate(path) if x == ".."]
        foldersToDel = []
        
        count = 0
        for idx in relIndexes:
            count = count+1
            folderIdx = idx - count
            print("idx: {2} ,count: {0}, folderIdx: {1}".format(count, folderIdx, idx))
            if folderIdx not in foldersToDel:
                foldersToDel.append(folderIdx)
            else:
                count = count+1
                folderIdx = idx - count
                foldersToDel.append(folderIdx)

        if len(foldersToDel) > 0:
            relIndexes = list(set(relIndexes + foldersToDel))

        relIndexes.sort()

        print("Deleting indexes: " + str(relIndexes))

        # removes elemets from largest index down
        for i in relIndexes[::-1]:
            del path[i]

        outputAbs = os.sep.join(path)
        return outputAbs

    def makeJson(self):
        return json.dumps(self.jsonData, indent=4, skipkeys=True, sort_keys=True)


if __name__ == "__main__":
    p = ProjectInfo()
    p.getInfo()
    print(p.makeJson())
