using BlenderRenderController.Properties;
using BRClib;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlenderRenderController
{
    static class Helper
    {
        private static Logger logger = LogManager.GetLogger("Helper");

        static public bool ClearOutputFolder(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                DirectoryInfo[] subDirs = dir.GetDirectories();

                // clear files in the output
                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.Delete();
                }

                // clear files in the 'chunks' subdir
                var chunkSDir = subDirs.FirstOrDefault(di => di.Name == Constants.ChunksSubfolder);
                if (chunkSDir != null)
                {
                    Directory.Delete(chunkSDir.FullName, true);
                }

                return true;
            }
            catch (IOException)
            {
                string msg = "Can't clear output folder, files are in use";
                logger.Error(msg);
                MessageBox.Show(msg);
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Trace(ex.StackTrace);
                MessageBox.Show("An unexpected error ocurred, sorry.\n\n" + ex.Message);
                return false;
            }
        }
        

        static public IEnumerable<Control> FindControlsByTag(Control.ControlCollection controls, string key)
        {
            List<Control> controlsWithTags = new List<Control>();

            foreach (Control c in controls)
            {
                if (c.Tag != null)
                {
                    // splits tag content into string array
                    string[] tags = c.Tag.ToString().Split(';');

                    // if key maches, add to list
                    if (tags.Contains(key))
                        controlsWithTags.Add(c);
                }

                if (c.HasChildren)
                {
                    //Recursively check all children controls as well; ie groupboxes or tabpages
                    controlsWithTags.AddRange(FindControlsByTag(c.Controls, key));
                }
            }

            return controlsWithTags;
        }


    }
}
