using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace BRClib
{
    public class ProcessScheduler
    {
        //static List<Process> processCol;
        //static Semaphore semaphore;
        //static Thread[] threads;

        public ProcessScheduler()
        {
        }

        public Task<bool> RunRenderProcesses(IEnumerable<Process> processes, int max, CancellationToken token, IProgress<string> renderOutput)
        {
            bool canReportOutput = renderOutput != null;

            // control list
            var processCol = new List<Process>(processes);
            var tcs = new TaskCompletionSource<bool>();

            // setup processes
            foreach (var p in processes)
            {
                p.EnableRaisingEvents = true;

                if (canReportOutput)
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                            renderOutput.Report(e.Data);
                    };
                }
            }

            var pOptions = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = max
            };

            try
            {
                var pResult = Parallel.ForEach(processes, pOptions, 
                (process, state) => 
                {
                    process.Start();

                    // wait for the process to exit
                    while (!process.HasExited)
                    {
                        Thread.Sleep(10);

                        // if a cancel request is detected, kill the current and 
                        // dispose the current process and tell the loop to stop
                        if (pOptions.CancellationToken.IsCancellationRequested)
                        {
                            Debug.WriteLine("Cancellation detected, killing process");

                            process.Kill();
                            process.Dispose();

                            state.Stop();
                            return;
                        }
                    }

                    // remove the sucessfully finished process from the control list
                    processCol.Remove(process);
                });

                /* wait for the parallel loop to finish,
                   call to 'ToList' is to make sure we get
                   sync'd values */
                while (processCol.ToList().Count != 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                        break;
                    }

                    Thread.Sleep(100);
                }

                // all processes finished sucessfully
                tcs.SetResult(true);
            }
            catch (OperationCanceledException)
            {
                Trace.WriteLine("Operation cancelled");
                tcs.SetCanceled();
            }


            return tcs.Task;
        }

        public Task<bool> RunRenderProcesses(IEnumerable<Process> processes, int max, CancellationToken token)
        {
            return RunRenderProcesses(processes, max, token, null);
        }

        //Task<bool> RunRenderProcessesOLD(ICollection<Process> processes, int max, CancellationToken token, IProgress<string> renderOutput)
        //{
        //    bool canReportOutput = renderOutput != null;

        //    processCol = new List<Process>(processes);
        //    semaphore = new Semaphore(max, max);
        //    threads = new Thread[processes.Count];

        //    for (int i = 0; i < processCol.Count; i++)
        //    {
        //        Thread.Sleep(100);

        //        var proc = processCol[i];
        //        proc.EnableRaisingEvents = true;
        //        proc.Exited += Process_Exited;

        //        if (canReportOutput)
        //        {
        //            proc.StartInfo.UseShellExecute = false;
        //            proc.StartInfo.RedirectStandardOutput = true;
        //            proc.OutputDataReceived += (s, e) =>
        //            {
        //                if (e.Data != null)
        //                    renderOutput.Report(e.Data);
        //            };
        //        }

        //        var thread = new Thread(() => RunProcessThread(proc));
        //        thread.Name = "thread_" + i;
        //        threads[i] = thread;
        //        thread.Start();
        //    }

        //    var tcs = new TaskCompletionSource<bool>();

        //    // default
        //    tcs.SetResult(true);

        //    while (processCol.ToList().Count != 0)
        //    {
        //        if (token.IsCancellationRequested)
        //        {
        //            // TODO dispose any running threads and process...
        //            tcs.SetResult(false);
        //            break;
        //        }
        //    }

        //    return tcs.Task;
        //}

        //void RunProcessThread(Process process)
        //{
        //    Debug.WriteLine("[{0}] {1} is waiting in line...", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.Name);

        //    semaphore.WaitOne();

        //    Debug.WriteLine("[{0}] {1} enters the RunProcess!", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.Name);

        //    process.Start();
        //    process.WaitForExit();

        //    processCol.Remove(process);

        //    Debug.WriteLine("[{0}] {1} is leaving the RunProcess!", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.Name);

        //    semaphore.Release();
        //}

    }
}
