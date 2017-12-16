using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BRClib;
using System.Threading;
using static UnitTests.TestHelpers;

namespace UnitTests
{
    [TestClass]
    public class AsyncProcessExtentionTests
    {
        [TestMethod]
        public void StartAsync_exec_GetBlendProc()
        {
            var getBlendInfoProc = GetBlendDataProc(BLEND_PATH);
            var result = getBlendInfoProc.StartAsync().Result;

            // assert sucess exit code
            Assert.AreEqual(0, result, "Process was not sucessfull");
        }

        [TestMethod]
        public void StartAsync_exec_GetBlendProc_and_get_output()
        {
            var getBlendInfoProc = GetBlendDataProc(BLEND_PATH);

            var report = getBlendInfoProc.StartAsync(true, true).Result;

            // assert sucess exit code
            Assert.AreEqual(0, report.ExitCode);

            Console.WriteLine("Std output: \n" + report.StdOutput);
            Console.WriteLine(new String('-', 50));
            Console.WriteLine("Std error: \n" + report.StdError);

        }
    }
}
