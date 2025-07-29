using System;
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
			fileTools.AutoDetectDrive();

			string dirPath = fileTools.InputPath;
			if (dirPath == null)
			{
                ConsoleHelpers.Muted("Switching to manual selection...");
                fileTools.ManuallySelectDrive();
				dirPath = fileTools.InputPath;
			}
            ConsoleHelpers.Muted($"Selected Base Directory: {dirPath}");

            string[] directories = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories);
            foreach (string directory in directories)
			{
                ConsoleHelpers.Muted($"Indexing files in {directory}...");
			}

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
