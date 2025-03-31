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
                Console.WriteLine("Switching to manual selection...");
				fileTools.ManuallySelectDrive();
				dirPath = fileTools.InputPath;
			}
            Console.WriteLine($"Selected Base Directory: {dirPath}");


			//string[] files = System.IO.Directory.GetFiles("E:/Testing");

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

			Console.WriteLine("Ended");
            Console.ReadLine();
        }
    }
}
