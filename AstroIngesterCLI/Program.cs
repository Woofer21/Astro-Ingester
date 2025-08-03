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
                                ConsoleHelpers.Error("No config file path provided after config argumment");
                                break;
                            }

                            string configPath = args[i + 1];
                            ConsoleHelpers.Info($"Loading config from: {configPath}");

                            if (!File.Exists(configPath))
                            {
                                ConsoleHelpers.Error($"Config file at {configPath} does not exist.");
                                break;
                            }

                            break;
                    }
                }
            }

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

			//Determine output modes, this decides if we need multiple output paths or just one
			string splitQuestion = "Would you like to split up files by their type? (Y/n): ";
            ConsoleHelpers.Log(splitQuestion, false);
			string splitInput = Console.ReadLine();
			if (splitInput != null)
			{
				if (splitInput.Equals("n", StringComparison.OrdinalIgnoreCase) || splitInput.Equals("no", StringComparison.OrdinalIgnoreCase))
				{
                    fileTools.SeperateByType = false;
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ConsoleHelpers.Muted(splitQuestion, false);
                    ConsoleHelpers.Error("No");
                }
                else
				{
                    fileTools.SeperateByType = true;
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ConsoleHelpers.Muted(splitQuestion, false);
                    ConsoleHelpers.Success("Yes");
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
                        ConsoleHelpers.Muted(ignoreQuestion, false);
                        ConsoleHelpers.Success("Yes");
                    }
                    else
                    {
                        fileTools.IgnoreUncategorized = true;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        ConsoleHelpers.Muted(ignoreQuestion, false);
                        ConsoleHelpers.Error("No");
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
                    ConsoleHelpers.Muted(commentQuestion, false);
                    ConsoleHelpers.Error("No");
                }
                else
                {
                    fileTools.SeperateByComment = true;
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ConsoleHelpers.Muted(commentQuestion, false);
                    ConsoleHelpers.Success("Yes");
                }
            }

            fileTools.AddOutputPath();
            fileTools.StartMoving();

			ConsoleHelpers.Log("Ended");
            Console.ReadLine();
        }
    }
}
