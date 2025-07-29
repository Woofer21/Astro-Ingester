using MetadataExtractor.Formats.Exif;
using MetadataExtractor;
using Directory = MetadataExtractor.Directory;

namespace AstroIngesterCore
{
    public static class MetadataTools
    {

        public static string GetComment(string path)
        {
            try
            {
                IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);

                foreach (Directory directory in directories.OfType<ExifSubIfdDirectory>())
                {
                    foreach (Tag tag in directory.Tags)
                        if (tag.Type == ExifDirectoryBase.TagUserComment)
                            return tag.Description;
                }

                return null;
            }
            catch (Exception e)
            {
                ConsoleHelpers.Error($"Unhandled Exception: {e.Message}");
                throw;
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
            }
            catch (Exception e)
            {
                ConsoleHelpers.Error($"Unhandled Exception: {e.Message}");
                throw;
            }
        }

        public static void ListAllTags(string path)
        {
            IEnumerable<Directory> directories = ImageMetadataReader.ReadMetadata(path);

            foreach (Directory directory in directories)
            {
                foreach (Tag tag in directory.Tags)
                    ConsoleHelpers.Muted($"{directory.Name} - {tag.Name} = {tag.Description}");
            }
        }
    }
}
