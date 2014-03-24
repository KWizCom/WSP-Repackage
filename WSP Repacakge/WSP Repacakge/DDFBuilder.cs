using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSP_Repacakge
{
    public class DDFBuilder
    {
        public static void Build(string directory, string CabFileName)
        {
            ddfBuilder builder = new ddfBuilder(directory, CabFileName);
            if (builder.solutionPath != null)
            {
                builder.BuildDDF(directory, 1, "");
            }
        }
        class ddfBuilder
        {
            string _path;
            string solutionFile;
            StreamWriter writer;
            public ddfBuilder(string path, string CabFileName)
            {
                if (string.IsNullOrEmpty(CabFileName))
                    CabFileName = "tmp.cab";

                if (Directory.Exists(path))
                {
                    _path = path;
                    if (path.EndsWith("\\"))
                    {
                        if (File.Exists(path + "makecab.ddf"))
                        {
                            solutionFile = path + "makecab.ddf";
                        }
                        else
                        {

                            try
                            {
                                FileStream nStream = File.Create(path + "makecab.ddf");
                                nStream.Close();
                                solutionFile = path + "makecab.ddf";
                                writer = new StreamWriter(solutionFile);
                                writer.WriteLine(".OPTION EXPLICIT ;Generates Errors");
                                writer.WriteLine(".Set CabinetNameTemplate=" + CabFileName + "; The name of the file");
                                writer.WriteLine(".Set DiskDirectoryTemplate=CDROM");
                                writer.WriteLine(".Set CompressionType=MSZIP");
                                writer.WriteLine(".Set UniqueFiles=\"Off\"");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("The following error occured: " + ex.Message);
                            }
                        }

                    }
                    else
                    {
                        if (File.Exists(path + "\\makecab.ddf"))
                        {
                            solutionFile = path + "\\makecab.ddf";
                        }
                        else
                        {
                            try
                            {
                                FileStream nStream = File.Create(path + "\\makecab.ddf");
                                nStream.Close();
                                solutionFile = path + "\\makecab.ddf";
                                writer = new StreamWriter(solutionFile);
                                writer.WriteLine(".OPTION EXPLICIT ;Generates Errors");
                                writer.WriteLine(".Set CabinetNameTemplate=" + CabFileName + "; The name of the file");
                                writer.WriteLine(".Set DiskDirectoryTemplate=CDROM");
                                writer.WriteLine(".Set CompressionType=MSZIP");
                                writer.WriteLine(".Set UniqueFiles=\"Off\"");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("The following error occurred: " + ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid file path.");
                }
            }
            public string solutionPath
            {
                get
                {
                    return _path;
                }

            }
            public void BuildDDF(string path, int level, string fileAt)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();
                    string[] directories = Directory.GetDirectories(path);
                    addFiles(path, level, fileAt);
                    foreach (string directory in directories)
                    {
                        string entry = addFolder(directory, level);

                        string input = ".Set DestinationDir = " + entry;

                        writer.WriteLine(input);

                        BuildDDF(directory, level + 1, entry);

                    }
                    if (level == 1)
                    {
                        writer.Close();
                        Console.WriteLine("Operation Completed Successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            private string addFolder(string directory, int level)
            {

                //addFolder(path, root);
                string[] dirComps = directory.Split('\\');
                string dirEntry = "";
                try
                {
                    if (dirComps[0] != null)
                    {
                        for (int i = level; i > 0; i--)
                        {
                            dirEntry += dirComps[dirComps.Length - i] + "\\";

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The following error occurred while adding folder: " + ex.Message);
                }
                return dirEntry;
            }

            private void addFiles(string path, int level, string fileAt)
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    string[] fileComps = file.Split('\\');
                    try
                    {
                        if (fileComps[0] != null)
                        {
                            writer.WriteLine(fileAt + fileComps[fileComps.Length - 1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("The following error occurred while adding files: " + ex.Message);
                    }
                }
            }

        }
    }
}
