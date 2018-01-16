using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRClib.Commands
{
    public class GetInfoCmd : ExternalCommand
    {
        // 0=Blend file, 1=get_project_info.py
        const string GETINFO_FMT = "-b \"{0}\" -P \"{1}\"";

        public GetInfoCmd(string programPath) : base(programPath)
        {
        }

        public GetInfoCmd(string program, string blendFile, string projInfoScript)
            : base(program)
        {
            BlendFile = blendFile;
            ProjInfoScript = projInfoScript;
        }

        public string BlendFile { get; set; }
        public string ProjInfoScript { get; set; }


        protected override string GetArgs()
        {
            return String.Format(GETINFO_FMT,
                                    BlendFile, 
                                    ProjInfoScript);
        }
    }
}
