using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlenderRenderController
{

    static class Helper
    {

        /* public static IEnumerable<string> AlphanumericSort(this IEnumerable<string> me)
         {
             string[] Separator = new string[] { "-" };
         }*/
        static public void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
        static public void print(string text)
        {
            Trace.WriteLine(text);
        }
    }
}
