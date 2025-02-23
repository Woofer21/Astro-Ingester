using MetadataExtractor.Formats.Exif;
using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AstroIngesterCLI
{
    static class MetadataHelpers
    {
        public static string GetComment(string path)
        { 
            try
            {
                IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);

                foreach (Directory directory in directories)
                {
                    foreach (Tag tag in directory.Tags)
                        if (tag.Type == ExifDirectoryBase.TagUserComment)
                            return tag.Description;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to get comment: " + e);
                return null;
            }
        }

        public static DateTime GetDate(string path)
        {
            try
            {
                IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);

                ExifIfd0Directory directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                directory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out DateTime date);

                return date;
            } catch (Exception e)
            {
                Console.WriteLine($"Unhandled Exception: {e.Message}");
                throw;
            }
        }

        public static void ListAllTags(string path)
        {
            IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);

            foreach (Directory directory in directories)
            {
                foreach (Tag tag in directory.Tags)
                    Console.WriteLine($"{directory.Name} - {tag.Name} = {tag.Description}");
            }
        }
    }
}
