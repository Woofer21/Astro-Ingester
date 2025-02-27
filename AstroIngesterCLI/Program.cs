using System;
using AstroIngesterCore;

namespace AstroIngesterCLI
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] files = System.IO.Directory.GetFiles("E:/Testing");

			foreach (string file in files)
			{
				try
                {
                    DateTime picDate = MetadataTools.GetDate(file);
                    Console.WriteLine($"{file} - {picDate}");

                    string picComment = MetadataTools.GetComment(file);
					if (string.IsNullOrEmpty(picComment)) Console.WriteLine($"{file} - No Comment Added");
					else Console.WriteLine($"{file} - {picComment}");
                }
				catch (Exception e)
				{
					Console.WriteLine("Unhandled Main: " + e.Message);
				}
			}

			Console.WriteLine("Ended");
            Console.ReadLine();
        }
    }
}
