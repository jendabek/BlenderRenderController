using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRClib
{
    public class Parsers
    {
        public static BlendData ParsePyOutput(IEnumerable<string> output)
        {
            StringBuilder jsonInfo = new StringBuilder();
            bool jsonStarted = false;
            int curlyStack = 0;

            // Filter out errors and create data
            foreach (string line in output)
            {
                if (line.Contains("{"))
                {
                    jsonStarted = true;
                    curlyStack++;
                }
                if (jsonStarted)
                {
                    if (!line.ToLower().Contains("blender quit") && curlyStack > 0)
                    {
                        jsonInfo.AppendLine(line);
                    }
                    if (line.Contains("}"))
                    {
                        curlyStack--;
                        if (curlyStack == 0)
                        {
                            jsonStarted = false;
                        }
                    }
                }
            }

            var json = jsonInfo.ToString();
            var bData = JsonConvert.DeserializeObject<BlendData>(json);

            return bData;
        }
    }
}
