import os
import bpy
import sys


blendPath = bpy.context.blend_data.filepath;
projName  = bpy.path.display_name_from_filepath( blendPath );

# get output path
arg_sep_index = sys.argv.index("--") + 1
out_arg = sys.argv[arg_sep_index:][0]

outFolderPath = out_arg
mixDownPath = outFolderPath + os.sep + projName + ".ac3"

bpy.ops.sound.mixdown( filepath=mixDownPath,
		               container='AC3',
					   codec='AC3',
					   accuracy=1024,
					   bitrate=512,
					   format="F32",
					   split_channels=False);

