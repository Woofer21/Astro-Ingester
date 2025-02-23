using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace AstroIngesterCLI
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] files = System.IO.Directory.GetFiles("E:/Testing");

			foreach (string file in files)
			{
				Console.WriteLine($"Found File: {file}");

                string comment = getComment(file);
                if (string.IsNullOrEmpty(comment))
                {
                    Console.WriteLine("Unable to get comment on image. Either No comment present or it errored");
                } else Console.WriteLine(comment);
            }

			Console.WriteLine("Ended");
            Console.ReadLine();
		}

		static string getComment(string filePath)
		{
			try
			{ 
                IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(filePath);

                foreach (MetadataExtractor.Directory directory in directories)
                {
                    foreach (Tag tag in directory.Tags)
                        if (tag.Type == ExifDirectoryBase.TagUserComment)
                            return tag.Description;
                }

                return null;
            } catch (Exception e)
			{
                Console.WriteLine("Unable to get comment: " + e);
				return null;
			}

        }
    }
}
