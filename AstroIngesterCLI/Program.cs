using System;

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
					DateTime picDate = MetadataHelpers.GetDate(file);
					Console.WriteLine($"{file} - {picDate}");
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
