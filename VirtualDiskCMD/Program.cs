using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VirtualDiskCMD
{
    class Program
    {
        static IVolumeInfo volume;
        static IDirectoryInfo currentDirectory;
        static bool running = true;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    ProcessCommand(args[i].Replace('\'', '\"'));
                }
            }

            while (running)
            {
                if (volume == null)
                    currentDirectory = null;

                if (currentDirectory == null)
                    Console.Write("NULL");
                else
                    Console.Write(currentDirectory.FullName);

                Console.Write("/ ");
                string cmd = Console.ReadLine();
                ProcessCommand(cmd);
            }

        }

        static void ProcessCommand(string command)
        {
            string[] parsed;
            if (Parse(command, out parsed))
            {
                ProcessParsedCommand(parsed);
            }
        }

        static void ProcessParsedCommand(string[] parsed)
        {
            Dictionary<string, Func<string[], bool>> commands = new Dictionary<string, Func<string[], bool>>();
            commands.Add("mount", OpenVolume);
            commands.Add("write", CloseVolume);
            commands.Add("unmount", Unmount);
            commands.Add("ls", ListDirectory);
            commands.Add("extract", ExtractFile);
            commands.Add("rm", RemoveFile);
            commands.Add("exit", Exit);
            commands.Add("import", ImportFile);
            commands.Add("defrag", Defrag);

            string cmd = parsed[0];
            List<string> args = new List<string>(parsed);
            args.RemoveAt(0);
            if (commands.ContainsKey(cmd))
            {
                commands[cmd](args.ToArray());
            }
            else
            {
                Console.WriteLine("Command not found.");
            }
        }

        static bool Parse(string command, out string[] p)
        {
            bool inQuote = false;

            List<string> parts = new List<string>();
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < command.Length; i++)
            {
                char c = command[i];
                if (inQuote)
                {
                    if (c == '\"')
                        inQuote = false;
                    else
                        s.Append(c);
                }
                else if (c == ' ')
                {
                    parts.Add(s.ToString());
                    s.Clear();
                }
                else if (c == '\"')
                {
                    inQuote = true;
                }
                else
                {
                    s.Append(c);
                }
            }

            if (s.Length > 0)
                parts.Add(s.ToString());

            if (inQuote)
            {
                Console.WriteLine("SYNTAX ERROR: Expected \".");
                p = new string[0];
                return false;
            }
            else
            {
                p = parts.ToArray();
                return true;
            }
        }

        static bool Unmount(string[] args)
        {
            volume = null;
            return true;
        }

        static bool Exit(string[] args)
        {
            running = false;
            return true;
        }

        static bool OpenVolume(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No path provided.");
                return false;
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("Too many arguments.");
                return false;
            }

            byte[] volumeData = File.ReadAllBytes(args[0]);
            volume = new VMFS1Volume(volumeData);
            currentDirectory = volume;
            return true;
        }

        static bool CloseVolume(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No path provided.");
                return false;
            }
            else if (args.Length > 1)
            {
                Console.WriteLine("Too many arguments.");
                return false;
            }

            byte[] volumeData = volume.ToBinary();
            File.WriteAllBytes(args[0], volumeData);
            return true;
        }

        static bool ListDirectory(string[] args)
        {
            IFileInfo[] files = currentDirectory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                Console.Write(files[i].Name);
                Console.Write("\t\t\t");
                Console.Write(files[i].Size);
                Console.WriteLine();
            }

            return true;
        }

        static bool ExtractFile(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("extract filename destination");
                return false;
            }

            string filename = args[0];
            string destination = args[1];

            IFileInfo[] files = currentDirectory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Trim('\0') == filename)
                {
                    try
                    {
                        File.WriteAllBytes(destination, files[i].ReadAllBytes());
                        return true;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                        return false;
                    }
                }
            }

            Console.WriteLine("File not found.");
            return false;
        }

        static bool ImportFile(string[] args)
        {
            if (args[0] == "?")
            {
                Console.WriteLine("Imports file into current directory.");
                Console.WriteLine("import source filename [-o]");
                Console.WriteLine("-o - Overwrite enabled.");
                return false;
            }

            string source = args[0];
            string filename = args[1];
            bool overwrite = args.Length > 2 ? args[2] == "-o" : false;

            IFileInfo[] files = currentDirectory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Trim('\0') == filename)
                {
                    try
                    {
                        if (overwrite)
                        {
                            files[i].WriteAllBytes(File.ReadAllBytes(source));
                            files[i].Create();
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("File already exists.");
                            return false;
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                        return false;
                    }
                }
            }

            // If file is new
            IFileInfo file = currentDirectory.CreateFile(filename);
            try
            {
                file.WriteAllBytes(File.ReadAllBytes(source));
                file.Create();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        static bool RemoveFile(string[] args)
        {
            string filename = args[0];

            IFileInfo[] files = currentDirectory.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.Trim('\0') == filename)
                {
                    try
                    {
                        files[i].Delete();
                        return true;

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                        return false;
                    }
                }
            }

            Console.WriteLine("File not found.");
            return false;
        }

        static bool Defrag(string[] args)
        {
            volume.Defragment();
            return true;
        }
    }
}
