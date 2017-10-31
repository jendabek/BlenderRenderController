using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlenderRenderController;
using System.Threading;
using static UnitTests.TestHelpers;

namespace UnitTests
{
    [TestClass]
    public class AsyncProcessExtentionTests
    {
        ManualResetEventSlim finishedEvent = new ManualResetEventSlim();

        [TestMethod]
        public void run_proc_async()
        {
            var getBlendInfoProc = GetBlendDataProc(BLEND_PATH);
            var result = getBlendInfoProc.StartAsync().Result;

            // assert sucess exit code
            Assert.AreEqual(0, result, "Process was not sucessfull");
        }

        [TestMethod]
        public void run_proc_async_and_get_output()
        {
            var getBlendInfoProc = GetBlendDataProc(BLEND_PATH);

            var (exitCode, stdOutput, stdErrors) = getBlendInfoProc.StartAsyncGetOutput().Result;

            // assert sucess exit code
            Assert.AreEqual(0, exitCode);

            Console.WriteLine("Std output: \n" + stdOutput);
            Console.WriteLine(new String('-', 50));
            Console.WriteLine("Std error: \n" + stdErrors);

        }
    }
}
