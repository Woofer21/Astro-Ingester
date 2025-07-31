using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                foreach (string arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "--verbose":
                        case "-v":
                            ConsoleHelpers.Log("Verbose mode enabled", true, ConsoleColor.Yellow);
                            fileTools.Verbose = true;
                            break;
                    }
                }
            }

            fileTools.AutoDetectDrive();
			string dirPath = fileTools.InputPath;
			List<OutputPathItem> outputPaths = fileTools.OutputPaths;

			//Select the input path
			if (dirPath == null)
			{
                ConsoleHelpers.Muted("Switching to manual selection...");
                fileTools.ManuallySelectDrive();
				dirPath = fileTools.InputPath;
			}
            ConsoleHelpers.Muted($"Selected Base Directory: {dirPath}");

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

			//Start processing input files
   //         string[] directories = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories);
   //         foreach (string directory in directories)
			//{
   //             ConsoleHelpers.Muted($"Indexing files in {directory}...");
   //             string[] files = Directory.GetFiles(directory, "*.*");
   //             foreach (string fileName in files)
   //             {
   //                 ConsoleHelpers.Muted($"Processing file: {fileName}");
                   
   //             }
			//}

            //Directory.CreateDirectory("d:/photography/nikon d5600/images/2026/01/01");


			//foreach (string file in files)
			//{
			//	try
   //             {
   //                 DateTime picDate = MetadataTools.GetDate(file);
   //                 Console.WriteLine($"{file} - {picDate}");

   //                 string picComment = MetadataTools.GetComment(file);
			//		if (string.IsNullOrEmpty(picComment)) Console.WriteLine($"{file} - No Comment Added");
			//		else Console.WriteLine($"{file} - {picComment}");
   //             }
			//	catch (Exception e)
			//	{
			//		Console.WriteLine("Unhandled Main: " + e.Message);
			//	}
			//}

			ConsoleHelpers.Log("Ended");
            Console.ReadLine();
        }
    }
}
