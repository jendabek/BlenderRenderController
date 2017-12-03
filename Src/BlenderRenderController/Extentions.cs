using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlenderRenderController
{
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
                @this.Invoke(action, param);
            }
            else
            {
                action(param);
            }

        }
        public static void InvokeAction<TArgs>(this Control @this, Action<object, TArgs> action, object obj, TArgs e)
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, obj, e);
            }
            else
            {
                action(obj, e);
            }
        }

        /// <summary>
        /// Starts the process asynchronously
        /// </summary>
        /// <param name="token">Cancelation token, calls the <see cref="Process.Kill()"/> method</param>
        /// <returns>The processe's exit code</returns>
        public static async Task<int> StartAsync(this Process proc, CancellationToken token = default)
        {
            // we must be aware of when the proc exits
            proc.EnableRaisingEvents = true;

            int result = 0;
            using (token.Register(ProcCancelCallback, proc))
            {
                result = await RunProcessAsync(proc).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Starts the process asynchronously and optionally reads its standard output and error streams
        /// </summary>
        /// <param name="token">Cancelation token, calls <see cref="Process.Kill()"/></param>
        /// <param name="getStdOut">If set to true, this method will read the Std Output and
        /// save its contents in <see cref="ProcessResult.StdOutput"/>, otherwise it will be null</param>
        /// <param name="getStdErr">If set to true, this method will read the Std Error and
        /// save its contents in <see cref="ProcessResult.StdError"/>, otherwise it will be null</param>
        /// <returns>A <see cref="ProcessResult"/> object with the exit code and, optionally, its 
        /// standard output and standard error contents as strings</returns>
        public async static Task<ProcessResult> StartAsync (this Process proc, 
                                                            bool getStdOut, 
                                                            bool getStdErr, 
                                                            CancellationToken token = default)
        {
            if (!getStdErr && !getStdOut)
            {
                var eCode = await StartAsync(proc, token);
                return new ProcessResult(eCode);
            }

            proc.EnableRaisingEvents = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = getStdOut;
            proc.StartInfo.RedirectStandardError = getStdErr;

            Task<int> eCodeTask;
            Task<string> soTask, seTask;

            using (token.Register(ProcCancelCallback, proc))
            {
                eCodeTask = RunProcessAsync(proc);
                soTask = getStdOut ? proc.StandardOutput.ReadToEndAsync() : Task.FromResult<string>(null);
                seTask = getStdErr ? proc.StandardError.ReadToEndAsync() : Task.FromResult<string>(null);

                await Task.WhenAll(eCodeTask, soTask, seTask).ConfigureAwait(false);
            }

            return new ProcessResult(eCodeTask.Result, soTask.Result, seTask.Result);
        }

        /*
        /// <summary>
        /// Starts the process asynchronously and optionally reads its standard error stream
        /// </summary>
        /// <param name="getStdErr">If set to true, this method will read and return the Std Error 
        /// contents as a string, otherwise "stdError" will be null</param>
        /// <param name="token">Cancelation token, calls the <see cref="Process.Kill()"/> method</param>
        /// <returns>A tuple with the Process exit code and, optionally, its standard error content
        /// as a string</returns>
        public static async Task<(int eCode, string stdError)> 
            StartAsync(this Process proc, bool getStdErr, CancellationToken token = default)
        {
            var result = await StartAsync(proc, false, getStdErr, token).ConfigureAwait(false);
            return (result.eCode, result.stdError);
        }
*/

        private static Task<int> RunProcessAsync(Process proc)
        {
            var tcs = new TaskCompletionSource<int>();

            EventHandler procExited = null;
            procExited = (s, e) =>
            {
                var p = s as Process;
                tcs.SetResult(p.ExitCode);
                p.Exited -= procExited;
            };

            proc.Exited += procExited;

            // Not sure about the guarantees of the Exited event in case of
            // process reuse
            bool started = proc.Start();
            if (!started)
            {
                throw new InvalidOperationException($"Could not start process: {proc}.\n" +
                    "Obs: Reusing existing processes is not supported.");
            }

            return tcs.Task;
        }

        private static void ProcCancelCallback(object obj)
        {
            if (obj is Process proc)
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString(), category: "Proc Extentions");
                }
                finally
                {
                    proc.Close();
                }
            }
        }


        /// <summary>
        /// Safely raises any EventHandler event asynchronously.
        /// </summary>
        /// <param name="sender">The object raising the event (usually this).</param>
        /// <param name="args">The TEventArgs for this event.</param>
        public static void Raise<TArgs>(this MulticastDelegate thisEvent, object sender, TArgs args)
        {
            var localMCD = thisEvent;
            AsyncCallback callback = ar => ((EventHandler<TArgs>)ar.AsyncState).EndInvoke(ar);

            foreach (Delegate d in localMCD.GetInvocationList())
            {
                if (d is EventHandler<TArgs> uiMethod)
                {
                    if (d.Target is ISynchronizeInvoke target)
                    {
                        target.BeginInvoke(uiMethod, new object[] { sender, args });
                    }
                    else
                    {
                        uiMethod.BeginInvoke(sender, args, callback, uiMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Safely raises any EventHandler event asynchronously.
        /// </summary>
        /// <param name="sender">The object raising the event (usually this).</param>
        /// <param name="args">The EventArgs for this event.</param>
        public static void Raise(this MulticastDelegate thisEvent, object sender, EventArgs args)
        {
            var localMCD = thisEvent;
            AsyncCallback callback = ar => ((EventHandler)ar.AsyncState).EndInvoke(ar);

            foreach (Delegate d in localMCD.GetInvocationList())
            {
                if (d is EventHandler uiMethod)
                {
                    if (d.Target is ISynchronizeInvoke target)
                    {
                        target.BeginInvoke(uiMethod, new object[] { sender, args });
                    }
                    else
                    {
                        uiMethod.BeginInvoke(sender, args, callback, uiMethod);
                    }
                }
            }
        }

    }



}
