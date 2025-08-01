using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCore
{
    public class FileTools
    {
        private DriveInfo[] drives = DriveInfo.GetDrives();
        private DriveInfo? selectedDrive;
        public Dictionary<string, List<OutputPathItem>> OutputPaths { get; } = [];
        public bool SeperateByType { get; set; } = true;
        public bool SeperateByComment { get; set; } = true;
        public bool IgnoreUncategorized { get; set; } = true;
        public bool Verbose { get; set; } = false;

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
                ConsoleHelpers.Log("Enter the path to the \"root folder\" you would like to input files from. \nThe program will search for picture files through each folder in the selected \"root folder\". \nEnter \"-1\" to exit\n> ", false);
                string? pathName = Console.ReadLine();

                if (pathName != null)
                {
                    if (pathName == "-1")
                        Environment.Exit(0);

                    if (Directory.Exists(pathName)) {
                        inputPath = pathName;
                        ConsoleHelpers.ClearLines(4);
                        ConsoleHelpers.Muted("Selected input path: ", false);
                        ConsoleHelpers.Success(inputPath);
                        return true;
                    } else
                    {
                        ConsoleHelpers.ClearLines(4);
                        ConsoleHelpers.Error($"Invalid path: {pathName}, please try again");
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
                ConsoleHelpers.Error("Invalid path, switching to manual input...");
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
                        ConsoleHelpers.Info($"Found Drive {dirName}");

                        while (selecting)
                        {
                            ConsoleHelpers.Log($"Select this drive? (Y/n): ", false);
                            string? choice = Console.ReadLine();

                            if (choice != null && (choice.Equals("n", StringComparison.CurrentCultureIgnoreCase) || choice.Equals("no", StringComparison.CurrentCultureIgnoreCase)))
                            {
                                selecting = false;
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Muted($"Select {dirName}? (Y/n): ", false);
                                ConsoleHelpers.Error("No");
                                return false;
                            } else
                            {
                                inputPath = dirName;
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Muted($"Select {dirName}? (Y/n): ", false);
                                ConsoleHelpers.Success("Yes");
                                return true;
                            }
                        }
                    }
                }
            }

            ConsoleHelpers.Muted("No Drives Detected");
            return false;
        }

        public void AddOutputPath()
        {
            bool choseOutputForAll = false;
            bool chooseLoop = true;
            while (chooseLoop)
            {
                int menuLineCount = 8;
                ConsoleHelpers.Log("Choose an option from the list: ", true);
                ConsoleHelpers.Log("1) Add an output path for a specifed file extention", true, SeperateByType ? ConsoleColor.White : ConsoleColor.DarkGray);
                ConsoleHelpers.Log("2) Add an output path for a specified comment content", true, SeperateByComment ? ConsoleColor.White : ConsoleColor.DarkGray);
                ConsoleHelpers.Log("3) Add an output path for all others", false, IgnoreUncategorized ? ConsoleColor.DarkGray : ConsoleColor.White);
                if (!IgnoreUncategorized) ConsoleHelpers.Error(" (required)");
                else ConsoleHelpers.Log("");
                ConsoleHelpers.Log("4) View current output paths", true, OutputPaths.Count > 0 ? ConsoleColor.White : ConsoleColor.DarkGray);
                ConsoleHelpers.Log("5) Continue Program", true, !IgnoreUncategorized ? choseOutputForAll ? ConsoleColor.White : ConsoleColor.DarkGray : ConsoleColor.White);
                ConsoleHelpers.Log("-1) Quit program\n> ", false);
                string? choice = Console.ReadLine();

                if (choice != null)
                {
                    if (choice == "-1")
                        Environment.Exit(0);
                    else if (choice == "1" && SeperateByType)
                    {
                        ConsoleHelpers.ClearLines(menuLineCount);
                        ConsoleHelpers.Success("1) Add an output path for a specifed file extention");

                        OutputPathItem? pathItem = null;
                        bool loop = true;
                        while (loop)
                        {
                            ConsoleHelpers.Log("Enter the output path, the files will be copied to <Your Entered Path>/<year>/<month>/<day>\n> ", false);
                            string? path = Console.ReadLine();

                            if (path != null && Directory.Exists(path))
                            {
                                pathItem = new OutputPathItem(path);
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Muted("Selected output path: ", false);
                                ConsoleHelpers.Success($"{path}/year/month/day");
                                loop = false;
                            } 
                            else
                            {
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Error($"Invalid path: {path}, please try again");
                            }
                        }

                        ConsoleHelpers.Log("Enter the file extentions you would like to copy to this path, seperated by commas (e.g. .jpg, .png, .tif): ", false);
                        string? extentionsInput = Console.ReadLine();
                        if (extentionsInput != null)
                        {
                            List<string> extentions = [.. extentionsInput.Split(',').Select(ext => ext.Trim())];
                            pathItem!.Extentions = extentions;
                            ConsoleHelpers.ClearLines(1);
                            ConsoleHelpers.Muted("Selected extentions: ", false);
                            ConsoleHelpers.Success(extentionsInput);
                        } 
                        else
                        {
                            ConsoleHelpers.Error("Unkown Error");
                        }

                        OutputPaths.TryGetValue("extention", out List<OutputPathItem>? outputPaths);
                        if (outputPaths == null)
                            OutputPaths.Add("extention", [pathItem!]);
                        else
                            OutputPaths["extention"].Add(pathItem!);
                    }
                    else if (choice == "2" && SeperateByComment)
                    {
                        ConsoleHelpers.ClearLines(menuLineCount);
                        ConsoleHelpers.Success("2) Add an output path for a specified comment content");

                        OutputPathItem? pathItem = null;
                        bool loop = true;
                        while (loop)
                        {
                            ConsoleHelpers.Log("Enter the output path, the files with matching comment will be copied to <Your Entered Path>/\n> ", false);
                            string? path = Console.ReadLine();

                            if (path != null && Directory.Exists(path))
                            {
                                pathItem = new OutputPathItem(path);
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Muted("Selected output path: ", false);
                                ConsoleHelpers.Success($"{path}/");
                                loop = false;
                            }
                            else
                            {
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Error($"Invalid path: {path}, please try again");
                            }
                        }

                        ConsoleHelpers.Log("Enter the EXACT comment content to match, you can add multiple by seprating them by a comma: ", false);
                        string? commentsInput = Console.ReadLine();
                        if (commentsInput != null)
                        {
                            List<string> comments = [.. commentsInput.Split(',').Select(ext => ext.Trim())];
                            pathItem!.Comments = comments;
                            ConsoleHelpers.ClearLines(1);
                            ConsoleHelpers.Muted("Comment to check against: ", false);
                            ConsoleHelpers.Success(commentsInput);
                        }
                        else
                        {
                            ConsoleHelpers.Error("Unkown Error");
                        }

                        OutputPaths.TryGetValue("comment", out List<OutputPathItem>? outputPaths);
                        if (outputPaths == null)
                            OutputPaths.Add("comment", [pathItem!]);
                        else
                            OutputPaths["comment"].Add(pathItem!);
                    }
                    else if (choice == "3" && !IgnoreUncategorized)
                    {
                        ConsoleHelpers.ClearLines(menuLineCount);
                        ConsoleHelpers.Success("3) Add an output path for all others");

                        bool loop = true;
                        while (loop)
                        {
                            ConsoleHelpers.Log("Enter the output path for all other files to go to, they will go to <Your Entered Path>/year/month/day\n> ", false);
                            string? path = Console.ReadLine();

                            if (path != null && Directory.Exists(path))
                            {
                                OutputPaths.TryGetValue("other", out List<OutputPathItem>? outputPaths);
                                if (outputPaths == null)
                                    OutputPaths.Add("other", []);
                                else
                                    OutputPaths["other"].Add(new OutputPathItem(path));

                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Muted("Selected output path: ", false);
                                ConsoleHelpers.Success($"{path}/year/month/day");
                                loop = false;
                            }
                            else
                            {
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Error($"Invalid path: {path}, please try again");
                            }
                        }
                        choseOutputForAll = true;
                    }
                    else if (choice == "4" && OutputPaths.Count > 0)
                    {
                        int lineCount = 0;

                        ConsoleHelpers.ClearLines(menuLineCount);
                        ConsoleHelpers.Success("4) View current output paths");

                        for (int i = 0; i < 2; i++)
                        {
                            ConsoleColor color = i == 0 ? ConsoleColor.White : ConsoleColor.DarkGray;
                            foreach (string outputPathKey in OutputPaths.Keys)
                            {
                                List<OutputPathItem> outputPathItems = OutputPaths[outputPathKey];

                                lineCount++;
                                ConsoleHelpers.Log(outputPathKey, true, color);
                                for (int j = 0; j < outputPathItems.Count; j++)
                                {
                                    lineCount++;
                                    OutputPathItem pathItem = outputPathItems[j];
                                    ConsoleHelpers.Log($"|-> Output Path {j + 1}: {pathItem.Path}", true, color);
                                    if (pathItem.Extentions.Count > 0)
                                    {
                                        lineCount++;
                                        ConsoleHelpers.Log(" |-> Extentions: " + string.Join(", ", pathItem.Extentions), true, color);
                                    }
                                    if (pathItem.Comments.Count > 0)
                                    {
                                        lineCount++;
                                        ConsoleHelpers.Log(" |-> Comments: " + string.Join(", ", pathItem.Comments), true, color);
                                    }
                                }
                            }

                            if (i == 0)
                            {
                                ConsoleHelpers.Log("Press enter to return to menu...", false);
                                Console.ReadKey();
                                ConsoleHelpers.ClearLines(lineCount + 1);
                            }
                        }

                    }
                    else if ((choice == "5" && (!IgnoreUncategorized && choseOutputForAll)) || (choice == "5" && IgnoreUncategorized))
                    {
                        ConsoleHelpers.ClearLines(menuLineCount);
                        chooseLoop = false;
                    }
                    else
                        ConsoleHelpers.ClearLines(menuLineCount);
                }
            }
        }

        //public bool AddOutputPath(string path)
        //{
        //    if (Directory.Exists(path))
        //    {
        //        OutputPaths.Add(new OutputPathItem(path));
        //        return true;
        //    }
        //    else
        //    {
        //        ConsoleHelpers.Error("Invalid output path");
        //        return false;
        //    }
        //}

        public void StartMoving()
        {
            //        extention     ->  year        ->    month     ->    day -> List<FileInfo>
            Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<int, List<FileInfo>>>>> categorizedFiles = [];
            Dictionary<string, List<FileInfo>> categorizedByComment = [];

            string[] directories = Directory.GetDirectories(InputPath, "*", SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                if (Verbose) ConsoleHelpers.Muted($"Processing directory: {directory}");

                string[] filePaths = Directory.GetFiles(directory);
                foreach (string filePath in filePaths)
                {
                    if (Verbose) ConsoleHelpers.Muted($"|--> Processing file: {filePath}");

                    FileInfo fileInfo = new FileInfo(filePath);
                    DateTime fileTakenDate = MetadataTools.GetDate(filePath);
                    int year = fileTakenDate.Year;
                    int month = fileTakenDate.Month;
                    int day = fileTakenDate.Day;
                    string type = fileInfo.Extension.ToLower();
                    string? comment = MetadataTools.GetComment(filePath);

                    if (categorizedFiles.ContainsKey(type))
                    {
                        if (categorizedFiles[type].ContainsKey(year))
                        {
                            if (categorizedFiles[type][year].ContainsKey(month))
                            {
                                if (categorizedFiles[type][year][month].ContainsKey(day))
                                {
                                    categorizedFiles[type][year][month][day].Add(fileInfo);
                                }
                                else
                                {
                                    categorizedFiles[type][year][month].Add(day, [fileInfo]);
                                }
                            }
                            else
                            {
                                categorizedFiles[type][year].Add(month, []);
                                categorizedFiles[type][year][month].Add(day, [fileInfo]);
                            }
                        }
                        else
                        {
                            categorizedFiles[type].Add(year, []);
                            categorizedFiles[type][year].Add(month, []);
                            categorizedFiles[type][year][month].Add(day, [fileInfo]);
                        }
                    }
                    else
                    {
                        categorizedFiles.Add(type, []);
                        categorizedFiles[type].Add(year, []);
                        categorizedFiles[type][year].Add(month, []);
                        categorizedFiles[type][year][month].Add(day, [fileInfo]);
                    }

                    if (SeperateByComment)
                    {
                        if (comment != null)
                        {
                            if (!categorizedByComment.ContainsKey(comment))
                            {
                                categorizedByComment[comment] = [];
                            }
                            categorizedByComment[comment].Add(fileInfo);
                        }
                    }

                    if (Verbose) ConsoleHelpers.Muted($" |--> Categorized: {year}, {month}, {day}, {type}{(SeperateByComment ? $", {comment}" : "")}");
                }
            }

            if (Verbose)
            {
                foreach (string type in categorizedFiles.Keys)
                {
                    ConsoleHelpers.Muted(type);
                    foreach (int year in categorizedFiles[type].Keys)
                    {
                        ConsoleHelpers.Muted($"|-> {year}");
                        foreach (int month in categorizedFiles[type][year].Keys)
                        {
                            ConsoleHelpers.Muted($" |-> {month}");
                            foreach (int day in categorizedFiles[type][year][month].Keys)
                            {
                                ConsoleHelpers.Muted($"  |-> {day}");
                                foreach (FileInfo file in categorizedFiles[type][year][month][day])
                                {
                                    ConsoleHelpers.Muted($"   |-> {file.Name}");
                                }
                            }
                        }
                    }
                }
            }

            if (Verbose) ConsoleHelpers.Muted($"Starting Move Processing...");

        }
    }
}
