using NLog;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;

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

    static class Extentions
    {
        private delegate void SetPropertyThreadSafeDelegate<TResult>(Control @this, Expression<Func<TResult>> property, TResult value);

        public static void SetPropertyThreadSafe<TResult>(this Control @this, Expression<Func<TResult>> property, TResult value)
        {
            var propertyInfo = (property.Body as MemberExpression).Member
                as PropertyInfo;

            if (propertyInfo == null ||
                !@this.GetType().IsSubclassOf(propertyInfo.ReflectedType) ||
                @this.GetType().GetProperty(
                    propertyInfo.Name,
                    propertyInfo.PropertyType) == null)
            {
                throw new ArgumentException("The lambda expression 'property' must reference a valid property on this Control.");
            }

            if (@this.InvokeRequired)
            {
                @this.Invoke(new SetPropertyThreadSafeDelegate<TResult>
                (SetPropertyThreadSafe),
                new object[] { @this, property, value });
            }
            else
            {
                @this.GetType().InvokeMember(
                    propertyInfo.Name,
                    BindingFlags.SetProperty,
                    null,
                    @this,
                    new object[] { value });
            }
        }

        public static void InvokeAction(this Control @this, Action action)
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        public static void InvokeAction<T>(this Control @this, Action<T> action, T param)
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action);
            }
            else
            {
                action(param);
            }

        }
    }
}
