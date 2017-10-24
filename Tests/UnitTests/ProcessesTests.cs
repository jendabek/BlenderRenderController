using System;
using BRClib;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class ProcessesTests
    {


        [TestMethod]
        public void run_processes_concurrently()
        {
            var processList = new List<Process>
            {
                MockProcess(6),
                MockProcess(7),
                MockProcess(30),
                MockProcess(6),
                MockProcess(12),
                MockProcess(6),
                MockProcess(5),
                MockProcess(10),
            };

            var ps = new ProcessScheduler();
            var result = ps.RunRenderProcesses(processList, 4, default(CancellationToken), null).Result;

            // check that all processes in the list have exited
            int exitedCount = 0;
            foreach (var p in processList)
            {
                if (p.HasExited)
                {
                    exitedCount++;
                }
            }

            Assert.AreEqual(processList.Count, exitedCount);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void test_cancel_processes_parallel()
        {
            var processList = new List<Process>
            {
                MockProcess(60),
                MockProcess(70),
                MockProcess(30),
                MockProcess(60),
                MockProcess(120),
                MockProcess(60),
                MockProcess(50),
                MockProcess(100),
            };

            var ps = new ProcessScheduler();
            var tokenSource = new CancellationTokenSource();

            // cancel after 15s
            tokenSource.CancelAfter(15_000);

            var tResult = ps.RunRenderProcesses(processList, 4, tokenSource.Token, null);

            Assert.IsTrue(tResult.Status == TaskStatus.Canceled, "Operation was not cancelled");
        }



        Process MockProcess(int timeout)
        {
            var p = new Process
            {
                EnableRaisingEvents = true,
            };

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/C ping 127.0.0.1 -n " + (++timeout) + " > nul",
                UseShellExecute = false,
            };
            p.StartInfo = startInfo;

            return p;
        }



    }
}
