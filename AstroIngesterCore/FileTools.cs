using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                        ConsoleHelpers.Muted($"Selected input path: success({inputPath})");
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
                                ConsoleHelpers.Muted($"Select {dirName}? (Y/n): error(No)");
                                return false;
                            } else
                            {
                                inputPath = dirName;
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Muted($"Select {dirName}? (Y/n): success(Yes)");
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
                ConsoleHelpers.Log("Choose an option from the list: ");
                ConsoleHelpers.Log("1) Add an output path for a specifed file extention", true, SeperateByType ? ConsoleColor.White : ConsoleColor.DarkGray);
                ConsoleHelpers.Log("2) Add an output path for a specified comment content", true, SeperateByComment ? ConsoleColor.White : ConsoleColor.DarkGray);
                ConsoleHelpers.Log($"3) Add an output path for all others {(!IgnoreUncategorized ? "error((required))" : "")}", true, IgnoreUncategorized ? ConsoleColor.DarkGray : ConsoleColor.White);
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
                                ConsoleHelpers.Muted($"Selected output path: success({path}/year/month/day)");
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
                            List<string> extentions = [.. extentionsInput.Split(',').Select(ext => ext.Trim().ToLower())];
                            pathItem!.Extentions = extentions;
                            ConsoleHelpers.ClearLines(1);
                            ConsoleHelpers.Muted($"Selected extentions: success({extentionsInput})");
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
                                ConsoleHelpers.Muted($"Selected output path: success({path}/)");
                                loop = false;
                            }
                            else
                            {
                                ConsoleHelpers.ClearLines(2);
                                ConsoleHelpers.Error($"Invalid path: {path}, please try again");
                            }
                        }

                        ConsoleHelpers.Log("Filter by extentions in adition to comments? (Y/n): ", false);
                        string? extentionsChoice = Console.ReadLine();
                        if (extentionsChoice != null && (choice.Equals("n", StringComparison.CurrentCultureIgnoreCase) || choice.Equals("no", StringComparison.CurrentCultureIgnoreCase)))
                        {
                            ConsoleHelpers.ClearLines(1);
                            ConsoleHelpers.Muted("Filter by extentions in adition to comments? (Y/n): error(No)");
                        } 
                        else
                        {
                            ConsoleHelpers.ClearLines(1);
                            ConsoleHelpers.Muted("Filter by extentions in adition to comments? (Y/n): success(Yes)");

                            ConsoleHelpers.Log("Enter the file extentions you would like to copy to this path, seperated by commas (e.g. .jpg, .png, .tif): ", false);
                            string? extentionsInput = Console.ReadLine();
                            if (extentionsInput != null)
                            {
                                List<string> extentions = [.. extentionsInput.Split(',').Select(ext => ext.Trim().ToLower())];
                                pathItem!.Extentions = extentions;
                                ConsoleHelpers.ClearLines(1);
                                ConsoleHelpers.Muted($"Selected extentions: success({extentionsInput})");
                            }
                            else
                            {
                                ConsoleHelpers.Error("Unkown Error");
                            }
                        }

                        ConsoleHelpers.Log("Enter the EXACT comment content to match, you can add multiple by seprating them by a comma: ", false);
                        string? commentsInput = Console.ReadLine();
                        if (commentsInput != null)
                        {
                            List<string> comments = [.. commentsInput.Split(',').Select(ext => ext.Trim())];
                            pathItem!.Comments = comments;
                            ConsoleHelpers.ClearLines(1);
                            ConsoleHelpers.Muted($"Comment to check against: success({commentsInput})");
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
                                ConsoleHelpers.Muted($"Selected output path: success({path}/year/month/day)");
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
                        PrintOutputPaths(true, out int lineCount);

                        ConsoleHelpers.ClearLines(menuLineCount);
                        ConsoleHelpers.Success("4) View current output paths");

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
        public void AddOutputPath(OutputPathItem pathItem, string key)
        {
            if (!OutputPaths.ContainsKey(key))
                OutputPaths.Add(key, []);

            OutputPaths[key].Add(pathItem);
        }

        public void PrintOutputPaths(bool reprint, out int lineCount)
        {
            lineCount = 0;

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
                        if (pathItem.HasExtentions)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> Extensions: " + string.Join(", ", pathItem.Extensions), true, color);
                        }
                        if (pathItem.HasComments)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> Comments: " + string.Join(", ", pathItem.Comments2), true, color);
                        }
                        if (pathItem.HasBeforeDates)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> Before Dates: " + string.Join(", ", pathItem.BeforeDates), true, color);
                        }
                        if (pathItem.HasAfterDates)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> After Dates: " + string.Join(", ", pathItem.AfterDates), true, color);
                        }
                        if (pathItem.HasDays)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> Days: " + string.Join(", ", pathItem.Days), true, color);
                        }
                        if (pathItem.HasMonths)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> Months: " + string.Join(", ", pathItem.Months), true, color);
                        }
                        if (pathItem.HasYears)
                        {
                            lineCount++;
                            ConsoleHelpers.Log(" |-> Years: " + string.Join(", ", pathItem.Years), true, color);
                        }
                    }
                }

                if (i == 0 && reprint)
                {
                    ConsoleHelpers.Log("Press enter to return to menu...", false);
                    Console.ReadKey();
                    ConsoleHelpers.ClearLines(lineCount + 1);
                }

                if (!reprint) i += 2;
            }
        } 

        public async Task StartMoving()
        {
            string[] directories = Directory.GetDirectories(InputPath, "*", SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                Dictionary<string, List<MoveOperationItem>> categorizedOperations = new()
                {
                    { "extention", [] },
                    { "comment", [] },
                    { "other", [] }
                };
                ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

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

                    if (SeperateByType && OutputPaths.ContainsKey("extention"))
                    {
                        foreach (OutputPathItem pathitem in OutputPaths["extention"])
                        {
                            if (pathitem.Extentions.Contains(type))
                            {
                                string outputPath = Path.Combine(pathitem.Path, year.ToString(), month.ToString(), day.ToString());
                                MoveOperationItem moveItem = new MoveOperationItem(fileInfo.FullName, outputPath, fileInfo.Name, year, month, day, type, comment);
                                categorizedOperations["extention"].Add(moveItem);
                            }
                        }
                    }

                    if (SeperateByComment && OutputPaths.ContainsKey("comment"))
                    {
                        foreach (OutputPathItem pathItem in OutputPaths["comment"])
                        {
                            if (pathItem.Comments.Contains(comment))
                            {
                                string outputPath = Path.Combine(pathItem.Path);
                                MoveOperationItem moveItem = new MoveOperationItem(fileInfo.FullName, outputPath, fileInfo.Name, year, month, day, type, comment);
                                categorizedOperations["comment"].Add(moveItem);
                            }
                        }
                    }

                    if (OutputPaths.ContainsKey("other"))
                    {
                        foreach (OutputPathItem pathItem in OutputPaths["other"])
                        {
                            string outputPath = Path.Combine(pathItem.Path, year.ToString(), month.ToString(), day.ToString());
                            MoveOperationItem moveItem = new MoveOperationItem(fileInfo.FullName, outputPath, fileInfo.Name, year, month, day, type, comment);
                            categorizedOperations["other"].Add(moveItem);
                        }
                    }
                }

                foreach (string typeKey in categorizedOperations.Keys)
                {
                    await Task.Run(() =>
                    {
                        int count = 0;
                        int totalCount = categorizedOperations[typeKey].Count;
                        Parallel.ForEach(categorizedOperations[typeKey], parallelOptions, operation =>
                        {
                            try
                            {
                                if (!Directory.Exists(operation.DestinationPath))
                                    Directory.CreateDirectory(operation.DestinationPath);

                                string destPath = Path.Combine(operation.DestinationPath, operation.Name);

                                if (File.Exists(operation.SourcePath))
                                    File.Copy(operation.SourcePath, destPath, false);
                                
                                count++;
                                float progressDecimal = (float)count / totalCount;
                                string progressBar = new string('█', (int)(progressDecimal * 20)).PadRight(20, '░');
                                ConsoleHelpers.SyncWrite(1, $"Processing {typeKey} list {progressBar} ({count}/{totalCount}) ({operation.Name})", true, ConsoleColor.DarkGray);
                            }
                            catch (Exception error)
                            {
                                ConsoleHelpers.Error($"Error processing file {operation.SourcePath}: {error.Message}");
                            }
                        });
                        ConsoleHelpers.ClearLines(1);
                        ConsoleHelpers.Success($"Successfully processed {count}/{totalCount} files");
                        ConsoleHelpers.Muted(" ");
                    });
                }
            }
        }

        public async Task MoveFiles()
        {
            string[] directories = Directory.GetDirectories(InputPath, "*", SearchOption.AllDirectories);
            foreach (string directory in directories)
            {
                Dictionary<string, List<MoveOperationItem>> categorizedOperations = new()
                {
                    { "sort", [] },
                    { "copy", [] },
                    { "other", [] }
                };
                ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

                ConsoleHelpers.Muted($"Processing directory: {directory}");

                string[] filePaths = Directory.GetFiles(directory);
                int count = 0;
                int totalCount = filePaths.Length;
                for (int i = 0; i < totalCount; i++)
                {
                    string filePath = filePaths[i];
                    //if (Verbose) ConsoleHelpers.Muted($"|--> Indexing file: {filePath}");

                    FileInfo fileInfo = new FileInfo(filePath);
                    DateTime fileTakenDate = MetadataTools.GetDate(filePath);
                    int year = fileTakenDate.Year;
                    int month = fileTakenDate.Month;
                    int day = fileTakenDate.Day;
                    string type = fileInfo.Extension.ToLower();
                    string? comment = MetadataTools.GetComment(filePath);

                    if (OutputPaths.TryGetValue("sort", out List<OutputPathItem>? sortOutList))
                    {
                        foreach (OutputPathItem outputItem in sortOutList)
                        {
                            if (outputItem.VerifyFileAgainstOptions(fileInfo))
                            {
                                string outputPath = Path.Combine(outputItem.Path, year.ToString(), month.ToString(), day.ToString());
                                MoveOperationItem moveItem = new MoveOperationItem(fileInfo.FullName, outputPath, fileInfo.Name, year, month, day, type, comment);
                                categorizedOperations["sort"].Add(moveItem);
                            }
                        }
                    }

                    if (OutputPaths.TryGetValue("copy", out List<OutputPathItem>? copyOutList))
                    {
                        foreach (OutputPathItem outputItem in copyOutList)
                        {
                            if (outputItem.VerifyFileAgainstOptions(fileInfo))
                            {
                                string outputPath = Path.Combine(outputItem.Path);
                                MoveOperationItem moveItem = new MoveOperationItem(fileInfo.FullName, outputPath, fileInfo.Name, year, month, day, type, comment);
                                categorizedOperations["copy"].Add(moveItem);
                            }
                        }
                    }

                    if (OutputPaths.TryGetValue("other", out List<OutputPathItem>? otherOutList))
                    {
                        foreach (OutputPathItem outputItem in otherOutList)
                        {
                            if (outputItem.VerifyFileAgainstOptions(fileInfo))
                            {
                                string outputPath = Path.Combine(outputItem.Path, year.ToString(), month.ToString(), day.ToString());
                                MoveOperationItem moveItem = new MoveOperationItem(fileInfo.FullName, outputPath, fileInfo.Name, year, month, day, type, comment);
                                categorizedOperations["other"].Add(moveItem);
                            }
                        }
                    }

                    if (count == 0)
                        ConsoleHelpers.Muted(" ");

                    count++;
                    float progressDecimal = (float)count / totalCount;
                    string progressBar = new string('█', (int)(progressDecimal * 20)).PadRight(20, '░');
                    ConsoleHelpers.SyncWrite(1, $"Indexing file {fileInfo.Name} {progressBar} ({count}/{totalCount})", true, ConsoleColor.DarkGray);
                }

                ConsoleHelpers.ClearLines(2);
                ConsoleHelpers.Success($"Successfully indexed {totalCount}/{totalCount} files from {directory}");

                foreach (string typeKey in categorizedOperations.Keys)
                {
                    ConsoleHelpers.Muted(" ");

                    await Task.Run(() =>
                    {
                        int count = 0;
                        int errorCount = 0;
                        int totalCount = categorizedOperations[typeKey].Count;
                        Parallel.ForEach(categorizedOperations[typeKey], parallelOptions, operation =>
                        {
                            try
                            {
                                if (!Directory.Exists(operation.DestinationPath))
                                    Directory.CreateDirectory(operation.DestinationPath);

                                string destPath = Path.Combine(operation.DestinationPath, operation.Name);

                                if (File.Exists(operation.SourcePath))
                                    File.Copy(operation.SourcePath, destPath, false);

                                count++;
                                float progressDecimal = (float)count / totalCount;
                                string progressBar = new string('█', (int)(progressDecimal * 20)).PadRight(20, '░');
                                ConsoleHelpers.SyncWrite(1, $"Processing {typeKey} list {progressBar} ({count}/{totalCount}) [error({errorCount} )] ({operation.Name})", true, ConsoleColor.DarkGray);
                            }
                            catch (Exception error)
                            {
                                count++;
                                errorCount++;
                                float progressDecimal = (float)count / totalCount;
                                string progressBar = new string('█', (int)(progressDecimal * 20)).PadRight(20, '░');
                                ConsoleHelpers.SyncWrite(1, $"Processing {typeKey} list {progressBar} ({count}/{totalCount}) error([{errorCount}] ({operation.Name}))", true, ConsoleColor.DarkGray);
                                //ConsoleHelpers.SyncWrite($"Error processing file {operation.SourcePath}: {error.Message}", true, ConsoleColor.Red);
                            }
                        });
                        ConsoleHelpers.ClearLines(1);
                        ConsoleHelpers.Success($"Successfully processed {totalCount}/{totalCount} [error({errorCount} )] files");
                    });
                }
            }

            ConsoleHelpers.Log("All operations completed, press enter to exit...", false);
        }
    }
}
