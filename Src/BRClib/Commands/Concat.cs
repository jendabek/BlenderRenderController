using System;

namespace BRClib.Commands
{
    public class ConcatCmd : ExternalCommand
    {

        public ConcatCmd(string program, string concatTextFile, string outputFile, 
                      string mixdownFile = null, TimeSpan? duration = null)
            : base(program)
        {
            ConcatTextFile = concatTextFile;
            OutputFile = outputFile;
            MixdownFile = mixdownFile;
            Duration = duration;
        }

        public ConcatCmd(string program) : base(program) { }


        public string ConcatTextFile { get; set; }
        public string OutputFile { get; set; }
        public string MixdownFile { get; set; }
        public TimeSpan? Duration { get; set; }

        // ref: https://ffmpeg.org/ffmpeg-all.html#concat-1
        // 0=ChunkTxtPath, 1=Optional mixdown input, 2=Optional duration, 3=Final file path + .EXT
        const string CONCAT_FMT = "-f concat -safe 0 -i \"{0}\" {1} -c:v copy {2} \"{3}\" -y";

        protected override string GetArgs()
        {
            var mixdText = !string.IsNullOrWhiteSpace(MixdownFile) 
                ? "-i \"" + MixdownFile + "\" -map 0:v -map 1:a" : string.Empty;

            var durText = Duration.HasValue
                ? "-t " + Duration.Value.ToString(@"hh\:mm\:ss") : string.Empty;

            return string.Format(CONCAT_FMT, 
                                    ConcatTextFile, 
                                    mixdText, 
                                    durText, 
                                    OutputFile);
        }

    }
}
