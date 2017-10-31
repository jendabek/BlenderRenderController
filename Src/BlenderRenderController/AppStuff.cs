using NLog;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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


    public static class Extentions
    {
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

        /// <summary>
        /// Starts the process asynchronously
        /// </summary>
        /// <param name="token">Cancelation token, calls the <see cref="Process.Kill()"/> method</param>
        /// <returns>The processe's exit code</returns>
        public static async Task<int> StartAsync(this Process proc, CancellationToken token = default(CancellationToken))
        {
            // make sure proc's start info is correct
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.EnableRaisingEvents = true;

            // register cancel token to kill the process
            var reg = token.Register(() =>
            {
                try
                {
                    proc.Kill();
                }
                catch (InvalidOperationException ioex)
                {
                    Trace.WriteLine("Error killing process: " + ioex.Message, "Proc Extentions");
                }
                finally
                {
                    proc.Dispose();
                }
            });

            var result = await RunProcessAsync(proc).ConfigureAwait(false);
            reg.Dispose();

            return result;
        }

        /// <summary>
        /// Starts the process asynchronously
        /// </summary>
        /// <param name="token">Cancelation token, calls the <see cref="Process.Kill()"/> method</param>
        /// <returns>A tuple with the Process exit code, standard output and standard error strings</returns>
        public static async Task<(int, string, string)> StartAsyncGetOutput(this Process proc, CancellationToken token = default(CancellationToken))
        {
            StringBuilder stdOutput = new StringBuilder(),
                          stdErrors = new StringBuilder();

            // read the output and error data
            proc.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    stdOutput.AppendLine(e.Data);
                }
            };

            proc.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    stdErrors.AppendLine(e.Data);
                }
            };

            var result = await StartAsync(proc, token);
            return (result, stdOutput.ToString(), stdErrors.ToString());
        }


        private static Task<int> RunProcessAsync(Process proc)
        {
            var tcs = new TaskCompletionSource<int>();

            proc.Exited += (s, e) => tcs.SetResult(proc.ExitCode);

            bool started = proc.Start();
            if (!started)
            {
                throw new InvalidOperationException("Could not start process: " + proc);
            }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            return tcs.Task;
        }
    }
}
