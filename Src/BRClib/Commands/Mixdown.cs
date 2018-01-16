using System;

namespace BRClib.Commands
{
    public class MixdownCmd : ExternalCommand
    {

        public MixdownCmd(string program, string blendFile, Chunk range, 
                       string mixdownScript, string outputFolder)
            : base(program)
        {
            BlendFile = blendFile;
            Range = range;
            MixdownScript = mixdownScript;
            OutputFolder = outputFolder;
        }

        public MixdownCmd(string program, string blendFile, int start, int end, 
                       string mixdownScript, string outputFolder)
            : this(program, blendFile, new Chunk(start, end), mixdownScript, 
                   outputFolder) { }

        public MixdownCmd(string program) : base(program) { }


        public string BlendFile { get; set; }
        public Chunk Range { get; set; }
        public string MixdownScript { get; set; }
        public string OutputFolder { get; set; }

        // 0=Blend file, 1=start frame, 2=end frame, 3=mixdown_audio.py, 4=Output Folder
        const string MIXDOWN_FMT = "-b \"{0}\" -s {1} -e {2} -P \"{3}\" -- \"{4}\"";

        protected override string GetArgs()
        {
            return String.Format(MIXDOWN_FMT, 
                                    BlendFile,
                                    Range.Start,
                                    Range.End,
                                    MixdownScript,
                                    OutputFolder);
        }

    }
}
