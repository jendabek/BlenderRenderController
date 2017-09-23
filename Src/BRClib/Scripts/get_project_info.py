import os
import json
import bpy
import math


class Error(Exception):
    pass

class FolderCountError(Error):
    
    def __init__(self, msg, needed, got):
        self.needed = needed
        self.got = got
        self.msg = msg


class ProjectInfo:
    
    def __init__(self, legacy = False):
        self.legacy = legacy
        if self.legacy:
            print("Running in legacy mode")


    # get number of Scenes and active scene name
    scene = bpy.context.scene
    scenes = bpy.data.scenes

    # requests info from blender
    def getInfo(self):

        print("Requesting infos...")

        blendPath = bpy.context.blend_data.filepath;
        projectName  = bpy.path.display_name_from_filepath( blendPath );

        # get values from strings
        scenesNum = str(self.scenes).partition('[')[-1].rpartition(']')[0]
        sceneActive = str(self.scene).partition('("')[-1].rpartition('")')[0]

        scene = self.scene

        # set infos acording to active Scene
        start = scene.frame_start
        end   = scene.frame_end
        totalLength = end - start + 1
        imgFormat = scene.render.image_settings.file_format
        resolutionPercentage = scene.render.resolution_percentage
        resolution = "{0} x {1}".format(math.floor(scene.render.resolution_x * resolutionPercentage / 100),
                                        math.floor(scene.render.resolution_y * resolutionPercentage / 100))
        
        # ffmpeg info
        ffmpegFmt = scene.render.ffmpeg.format
        ffmpegCodec = scene.render.ffmpeg.codec

        # calc real fps
        fpsSource  = scene.render.fps
        fpsBase = scene.render.fps_base
        fps = fpsSource / fpsBase

        # convert output path to absolute
        # make sure path separator is constant
        output = scene.render.filepath
        outputPath = bpy.path.native_pathsep(bpy.path.abspath(output)) 

        # split and see if it needs fixing (one of the elements is '..')
        outSplit = outputPath.split(os.sep)
        
        if ".." in outSplit:
            try:
                print("Path has relative folders '/../'")
                outputPath = self.fixPath(outSplit)
            except FolderCountError as err:
                msg, n, g = err.args
                print("{0}. Expected {1}, got {2}".format(msg,n,g))
                print(type(err)) # can be detected by BRC
                raise err

        print("Building data...")

        if self.legacy:
            # old 0.8.2 format
            return {
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
		            'renderFormat': imgFormat
                };
        else:
            # new format for v0.9.8.0 and later
            return {
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
		            'imgFormat': imgFormat,
                    'ffmpegFmt': ffmpegFmt,
                    'ffmpegCodec': ffmpegCodec
                };

    # fixes relative paths
    def fixPath(self, path):

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

    # makes a json string from jsonData
    def makeJson(self, jsonData):
        return json.dumps(jsonData, indent=4, skipkeys=True, sort_keys=True)


if __name__ == "__main__":
    p = ProjectInfo()
    jsonStr = p.getInfo()
    print(p.makeJson(jsonStr))
