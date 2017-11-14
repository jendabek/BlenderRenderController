using System;
using System.ComponentModel;
using System.Diagnostics;
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

        /// <summary>
        /// Starts the process asynchronously
        /// </summary>
        /// <param name="token">Cancelation token, calls the <see cref="Process.Kill()"/> method</param>
        /// <returns>The processe's exit code</returns>
        public static async Task<int> StartAsync(this Process proc, CancellationToken token = default(CancellationToken))
        {
            // we must be aware when the proc exits
            proc.EnableRaisingEvents = true;

            int result = -24;
            using (token.Register(ProcCancelCallback, proc))
            {
                result = await RunProcessAsync(proc).ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// Starts the process asynchronously and reads its standard output and error streams
        /// </summary>
        /// <param name="token">Cancelation token, calls the <see cref="Process.Kill()"/> method</param>
        /// <returns>A tuple with the Process exit code, standard output and standard error contents respectively</returns>
        public static async Task<(int, string, string)> StartAsyncGetOutput(this Process proc, CancellationToken token = default(CancellationToken))
        {
            // in this case, we will be returning the value
            // of the output streams
            proc.EnableRaisingEvents = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            StringBuilder stdOutput = new StringBuilder(),
                          stdErrors = new StringBuilder();

            // read the output and error data
            DataReceivedEventHandler outHandler = null, errHandler = null;

            outHandler = (s, e) =>
            {
                if (e.Data != null)
                {
                    stdOutput.AppendLine(e.Data);
                }
            };

            errHandler = (s, e) =>
            {
                if (e.Data != null)
                {
                    stdErrors.AppendLine(e.Data);
                }
            };

            // unregister events on exit
            proc.Exited += (s, e) => 
            {
                proc.OutputDataReceived -= outHandler;
                proc.ErrorDataReceived -= errHandler;
            };

            proc.OutputDataReceived += outHandler;
            proc.ErrorDataReceived += errHandler;

            int result = -24;
            using (token.Register(ProcCancelCallback, proc))
            {
                result = await RunProcessAsync(proc).ConfigureAwait(false);
            }

            return (result, stdOutput.ToString(), stdErrors.ToString());
        }


        private static Task<int> RunProcessAsync(Process proc)
        {
            var tcs = new TaskCompletionSource<int>();
            proc.Exited += (s, e) => tcs.SetResult(proc.ExitCode);

            bool started = proc.Start();
            if (!started)
            {
                //tcs.SetException(new NotSupportedException("Reusing existing processes is not supported."));
                //return tcs.Task;
                throw new NotSupportedException("Reusing existing processes is not supported.");
            }

            // check the process start info to see if
            // it wants to redirect Std output and / or Std error
            if (!proc.StartInfo.UseShellExecute)
            {
                if (proc.StartInfo.RedirectStandardOutput)
                {
                    proc.BeginOutputReadLine();
                }
                if (proc.StartInfo.RedirectStandardError)
                {
                    proc.BeginErrorReadLine();
                }
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
                catch (InvalidOperationException ioex)
                {
                    Trace.WriteLine("Error killing process: " + ioex.Message, "Proc Extentions");
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
