import os
import sys
import bpy


blendPath = bpy.context.blend_data.filepath;
projName  = bpy.path.display_name_from_filepath( blendPath );

# get output path
arg_sep_index = sys.argv.index("--") + 1
outFolderPath = sys.argv[arg_sep_index:][0]

# get audio container
a_codec = bpy.context.scene.render.ffmpeg.audio_codec
fallback = False;

if a_codec == 'PCM':
    a_conteiner = 'wav'
elif a_codec == 'VORBIS':
    a_conteiner = 'ogg'
elif a_codec == 'NONE':
    # fallback to BRC default
    fallback = True
else:
    a_conteiner = a_codec.lower()

"""
 Mixdown using the current project settings, if 'fallback' is set, then
 use the old 'ac3' defaults
"""

if fallback:
    mixDownFilePath = os.path.join(outFolderPath, (projName + '.ac3'))
    bpy.ops.sound.mixdown(filepath=mixDownFilePath, container='AC3', codec='AC3', 
                          accuracy=1024, bitrate=512,format="F32",split_channels=False)
else:
    mixDownFilePath = os.path.join(outFolderPath, (projName + '.' + a_conteiner))
    bpy.ops.sound.mixdown(filepath=mixDownFilePath)

