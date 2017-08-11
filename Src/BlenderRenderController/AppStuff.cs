using NLog;

namespace BlenderRenderController
{
    class Constants
    {
        public const string PyGetInfo = "get_project_info.py";
        public const string PyMixdown = "mixdown_audio.py";
        public const string ChunksSubfolder = "chunks";
        public const string ScriptsSubfolder = "Scripts";
        public const string APP_TITLE = "Blender Render Controller";
        public const string ChunksTxtFileName = "chunklist.txt";

        /// <summary>
        /// In case <see cref="PyGetInfo"/> fails to fix relative paths,
        /// this will be in the process output
        /// </summary>
        public const string PY_FolderCountError = "<class '__main__.FolderCountError'>";
    }

    class CommandARGS
    {
        ///// <summary>
        ///// Concatenate command ARGS
        ///// <para>0=ChunkTxtPath, 1=AudioARGS, 3=Project name, 4=file .EXT</para>
        ///// </summary>
        //public const string ConcatenateComARGS = "-f concat -safe 0 -i \"{0}\" {1} -c:v copy \"{3}.{4}\" -y";

        ///// <summary>
        ///// Concatenate command with mixdown audio ARGS
        ///// <para>0=mixdown audio file path</para>
        ///// </summary>
        //public const string ConcatenateAudioARGS = "-i \"{0}\" -map 0:v -map 1:a";

        /// <summary>
        /// Gets the Concatenate ARGS with or without mixdown
        /// </summary>
        /// <param name="mixdownFound">If true, args will have mixdown parameters</param>
        /// <returns>
        /// <para>mixdownFound is True: 0=ChunkTxtPath, 1=Mixdown audio, 2=Project name, 3=file .EXT</para>
        /// <para>mixdownFound is False: 0=ChunkTxtPath, 1=Project name, 2=file .EXT</para>
        /// </returns>
        public static string GetConcatenateArgs(bool mixdownFound)
        {
            return mixdownFound 
                ? "-f concat -safe 0 -i \"{0}\" -i \"{1}\" -map 0:v -map 1:a -c:v copy \"{2}{3}\" -y" 
                : "-f concat -safe 0 -i \"{0}\" -c:v copy \"{1}{2}\" -y";
        }

        /// <summary>
        /// Get info command ARGS
        /// <para>0=Blend file, 1=get_project_info.py</para>
        /// </summary>
        public const string GetInfoComARGS = "-b \"{0}\" -P \"{1}\"";

        /// <summary>
        /// Mixdown command ARGS
        /// 0=Blend file, 1=start, 2=end, 3=mixdown_audio.py, 4=output
        /// </summary>
        public const string MixdownComARGS = "-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- \"{4}\"";

        /// <summary>
        /// 0=Blend file, 1=output, 2=Renderer, 3=Frame start, 4=Frame end
        /// </summary>
        public const string RenderComARGS = "-b \"{0}\" -o \"{1}\" -E {2} -s {3} -e {4} -a";
    }

    enum AppState
    {
        RENDERING_ALL, READY_FOR_RENDER, AFTER_START, NOT_CONFIGURED
    }

    enum AppErrorCode
    {
        BLENDER_PATH_NOT_SET, FFMPEG_PATH_NOT_SET, BLEND_FILE_NOT_EXISTS,
        RENDER_FORMAT_IS_IMAGE, BLEND_OUTPUT_INVALID, UNKNOWN_OS
    }


    class NlogHelper
    {
        public static void ChangeLogLevel(LogLevel level, string loggerName = null)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                if (!string.IsNullOrEmpty(loggerName))
                {
                    if (rule.NameMatches(loggerName))
                        rule.EnableLoggingForLevel(level);

                    else
                        continue;
                }
                else
                    rule.EnableLoggingForLevel(level);
            }

            LogManager.ReconfigExistingLoggers();
        }
    }

}
