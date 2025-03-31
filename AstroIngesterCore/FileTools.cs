using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCore
{
    public class FileTools
    {
        private DriveInfo[] drives = DriveInfo.GetDrives();
        private DriveInfo selectedDrive;
        private string inputPath;
        public string InputPath
        {
            get { return inputPath; }
        }

        public DriveInfo[] GetDrives()
        {
            return drives;
        }

        public bool ManuallySelectDrive()
        {
            while (true)
            {
                Console.Write("Enter the path you would like to input files from (-1 to kill program): ");
                string pathName = Console.ReadLine();

                if (pathName != null)
                {
                    if (pathName == "-1")
                        Environment.Exit(0);

                    if (Directory.Exists(pathName)) {
                        inputPath = pathName;
                        return true;
                    } 
                }
            }
        }

        public bool ManuallySelectDrive(string pathName)
        {
            if (Directory.Exists(pathName))
            {
                inputPath = pathName;
            } else
            {
                Console.WriteLine("Invalid path, switching to manual input...");
                ManuallySelectDrive();
            }
            return true;
        }

        public bool AutoDetectDrive()
        {
            for (int i = 0; i < drives.Length; i++)
            {
                DriveInfo drive = drives[i];
                List<string> directories = [..Directory.GetDirectories(drive.Name, "*")];
                
                foreach (string dirName in directories)
                {
                    if (dirName == $@"{drive.Name}DCIM")
                    {
                        bool selecting = true;
                        Console.WriteLine($"Found Drive {dirName}");

                        while (selecting)
                        {
                            Console.Write("Select this drive? (Y/N): ");
                            string choice = Console.ReadLine();

                            if (choice != null && (choice.Equals("y", StringComparison.CurrentCultureIgnoreCase) || choice.Equals("yes", StringComparison.CurrentCultureIgnoreCase)))
                            {
                                inputPath = dirName;
                                return true;
                            }
                            else if (choice != null && (choice.Equals("n", StringComparison.CurrentCultureIgnoreCase) || choice.Equals("no", StringComparison.CurrentCultureIgnoreCase)))
                            {
                                selecting = false;
                            }
                        }
                    }
                }
            }

            Console.WriteLine("No Drives Detected");
            return false;
        }

    }
}
