using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AstroIngesterCore;

namespace AstroIngesterCLI
{
	class Program
	{
		static void Main(string[] args)
		{

			FileTools fileTools = new();
            bool LoadedFromConfig = false;

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];
                    switch (arg.ToLower())
                    {
                        case "--verbose":
                        case "-v":
                            ConsoleHelpers.Log("Verbose mode enabled", true, ConsoleColor.Yellow);
                            fileTools.Verbose = true;
                            break;
                        case "--config":
                        case "-c":
                            if (args.Length <= i + 1)
                            {
                                ConsoleHelpers.Error("[CNFG] No config file path provided after config argumment");
                                break;
                            }

                            string configPath = args[i + 1];
                            ConsoleHelpers.Muted($"[info(CNFG)] Loading config from: {configPath}");

                            if (!File.Exists(configPath))
                            {
                                ConsoleHelpers.Error($"[CNFG] Config file at {configPath} does not exist.");
                                break;
                            }

                            FileInfo configFile = new(configPath);
                            if (configFile.Extension != ".txt")
                            {
                                ConsoleHelpers.Error("[CNFG] Config file must be a .txt file.");
                                break;
                            }

                            ConfigManager cfmg = new(fileTools);
                            cfmg.LoadConfig(configFile);
                            LoadedFromConfig = true;
                            break;
                    }
                }
            }

            if (!LoadedFromConfig)
                fileTools.AutoDetectDrive();

			string dirPath = fileTools.InputPath;
			Dictionary<string, List<OutputPathItem>> outputPaths = fileTools.OutputPaths;

			//Select the input path
			if (dirPath == null)
			{
                ConsoleHelpers.Muted("Switching to manual selection...");
                fileTools.ManuallySelectDrive();
				dirPath = fileTools.InputPath;
			}

            if (!LoadedFromConfig)
            {//Determine output modes, this decides if we need multiple output paths or just one
                string splitQuestion = "Would you like to split up files by their type? (Y/n): ";
                ConsoleHelpers.Log(splitQuestion, false);
                string splitInput = Console.ReadLine();
                if (splitInput != null)
                {
                    if (splitInput.Equals("n", StringComparison.OrdinalIgnoreCase) || splitInput.Equals("no", StringComparison.OrdinalIgnoreCase))
                    {
                        fileTools.SeperateByType = false;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        ConsoleHelpers.Muted($"{splitQuestion}error(No)");
                    }
                    else
                    {
                        fileTools.SeperateByType = true;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        ConsoleHelpers.Muted($"{splitQuestion}success(Yes)");
                    }
                }

                if (fileTools.SeperateByType)
                {
                    string ignoreQuestion = "Would you like to move files not split by type? (y/N): ";
                    ConsoleHelpers.Log(ignoreQuestion, false);
                    string ignoreInput = Console.ReadLine();
                    if (ignoreInput != null)
                    {
                        if (ignoreInput.Equals("y", StringComparison.OrdinalIgnoreCase) || ignoreInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
                        {
                            fileTools.IgnoreUncategorized = false;
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            ConsoleHelpers.Muted($"{ignoreQuestion}success(Yes)");
                        }
                        else
                        {
                            fileTools.IgnoreUncategorized = true;
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            ConsoleHelpers.Muted($"{ignoreQuestion}error(No)");
                        }
                    }
                }

                string commentQuestion = "Would you like to split & copy files by their comment content after ingestion? (Y/n): ";
                ConsoleHelpers.Log(commentQuestion, false);
                string commentInput = Console.ReadLine();
                if (commentInput != null)
                {
                    if (commentInput.Equals("n", StringComparison.OrdinalIgnoreCase) || commentInput.Equals("no", StringComparison.OrdinalIgnoreCase))
                    {
                        fileTools.SeperateByComment = false;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        ConsoleHelpers.Muted($"{commentQuestion}error(No)");
                    }
                    else
                    {
                        fileTools.SeperateByComment = true;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        ConsoleHelpers.Muted($"{commentQuestion}success(Yes)");
                    }
                }

                fileTools.AddOutputPath();
            }
            else
            {
                bool inMenu = true;
                int menuLines = 5;

                while (inMenu)
                {
                    ConsoleHelpers.Log("Loaded config from file. Please select one of the following options: ");
                    ConsoleHelpers.Log("1) Review current config");
                    ConsoleHelpers.Log("2) Continue with current config");
                    ConsoleHelpers.Log("-1) Exit the program");
                    ConsoleHelpers.Log("> ", false);

                    string choice = Console.ReadLine();
                    ConsoleHelpers.ClearLines(menuLines);
                    switch (choice)
                    {
                        case "1":
                            ConsoleHelpers.Log("1) Review current config");
                            ConsoleHelpers.Log($"Input Path: {fileTools.InputPath}");
                            ConsoleHelpers.Log($"Verbose Mode: {(fileTools.Verbose ? "Enabled" : "Disabled")}");

                            fileTools.PrintOutputPaths(false, out int lineCount);

                            ConsoleHelpers.Log("Press any key to continue...", false);
                            string _ = Console.ReadLine();
                            ConsoleHelpers.ClearLines(lineCount + 4);
                            break;
                        case "2":
                            inMenu = false;
                            break;
                        case "-1":
                            Environment.Exit(0);
                            break;
                        default:
                            continue;
                    }
                }
            }

            if (LoadedFromConfig)
                fileTools.MoveFiles();
            else
                fileTools.StartMoving();

            Console.ReadLine();
        }
    }
}
