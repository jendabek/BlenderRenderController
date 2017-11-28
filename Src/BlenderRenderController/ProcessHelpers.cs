using NLog;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace BlenderRenderController
{
    static class ProcessFactory
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public static Process MixdownProcess(string blenderEXE, string args)
        {
            var mixdownCom = new Process();
            var info = new ProcessStartInfo()
            {
                FileName = blenderEXE,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = args
            };
            mixdownCom.StartInfo = info;
            mixdownCom.EnableRaisingEvents = true;
            mixdownCom.Exited += (pSender, pe) =>
            {
                logger.Info("Mixdown exit code: " + (pSender as Process).ExitCode);
            };

            logger.Info("Rendering mixdown");
            logger.Trace("cmd:> blender " + mixdownCom.StartInfo.Arguments);

            return mixdownCom;
        }

        public static Process ConcatProcess(string ffmpegEXE, string args)
        {
            var concatenateCom = new Process();
            var info = new ProcessStartInfo
            {
                FileName = ffmpegEXE,
                CreateNoWindow = true,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            concatenateCom.StartInfo = info;
            concatenateCom.EnableRaisingEvents = true;
            concatenateCom.Exited += (s, e) =>
            {
                logger.Info("FFmpeg exit code: {0}", (s as Process).ExitCode);
            };

            logger.Trace("cmd:> ffmpeg " + concatenateCom.StartInfo.Arguments);
            logger.Info("Joining chunks");

            return concatenateCom;
        }
    }

    /// <summary>
    /// A helper class to run external processes
    /// </summary>
    class ProcessRunner
    {
        Process _proc;
        string _stdErr, _stdOut, _procName;
        int _procExitCode;

        public ProcessRunner()
        { }

        public ProcessRunner(Process process)
        {
            _proc = process;
        }

        public Process Process { get => _proc; set => _proc = value; }


        public Task<bool> Run(CancellationToken token = default)
        {
            var tcs = new TaskCompletionSource<bool>();
            _procName = Path.GetFileNameWithoutExtension(_proc.StartInfo.FileName);

            var pTask = _proc.StartAsync(true, true, token);

            pTask.ContinueWith(t =>
            {
                tcs.SetResult(false);
            },
            TaskContinuationOptions.OnlyOnCanceled);

            pTask.ContinueWith(t =>
            {
                _procExitCode = t.Result.ExitCode;
                _stdOut = t.Result.StdOutput;
                _stdErr = t.Result.StdError;

                tcs.SetResult(_procExitCode == 0);
            },
            TaskContinuationOptions.ExecuteSynchronously |
            TaskContinuationOptions.OnlyOnRanToCompletion);

            return tcs.Task;
        }

        public void SaveReport(string folderPath)
        {
            var tmp = Path.GetRandomFileName();
            var file = _proc.ProcessName + '_' + Path.ChangeExtension(tmp, "txt");
            var filePath = Path.Combine(folderPath, file);

            string fmt = Properties.Resources.BadProcResult_Report;
            using (var sw = File.AppendText(filePath))
            {
                sw.Write(_procName + ' ');
                sw.WriteLine(string.Format(fmt, _procExitCode, _stdErr, _stdOut));
                sw.Write("\n\n");
            }
        }
    }

    /// <summary>
    /// Holds information about a process that ran asynchronously
    /// </summary>
    public class ProcessResult
    {
        public ProcessResult()
        {}

        public ProcessResult(int exitCode) : this(exitCode, null, null)
        { }

        public ProcessResult(int exitCode, string stdOutput, string stdError)
        {
            ExitCode = exitCode;
            StdOutput = stdOutput;
            StdError = stdError;
        }

        public int ExitCode { get; set; }
        public string StdOutput { get; set; }
        public string StdError { get; set; }
    }

}
