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

			FileTools fileT = new();
			DriveInfo[] drives = fileT.GetDrives();
			DriveInfo selectedDrive = null;

			bool gettingDrive = true;

			while (gettingDrive)
			{
                Console.WriteLine("Type a number to select an 'Input Drive' from the list below");
                
				for (int i = 0; i < drives.Length; i++)
                {
                    Console.WriteLine($"{i + 1}) {drives[i].Name}");
                }
                Console.WriteLine("-1) Close");

                Console.Write("Drive Number: ");
				string drive = Console.ReadLine();
                bool isValidDrive = int.TryParse(drive, out int driveNum);

				if (driveNum == -1) Process.GetCurrentProcess().Kill();

				if (isValidDrive)
				{
                    bool isDriveSelected = fileT.SelectDrive(driveNum, out selectedDrive);
                    if (isDriveSelected) gettingDrive = false;
                }
            }

            Console.WriteLine($"Selected Drive {selectedDrive.Name}");




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
