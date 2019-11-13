using System;
using System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoStructor.Data;
using PhotoStructor.Interfaces;

namespace PhotoStructor.Helpers.Readers
{
    public class AppleReader : IFileReader
    {
        public string Prefix => "IMG";

        public DateTime GetImageData(string path, out string postfix)
        {
            try
            {
                postfix = "i";
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                //todo-AD Get actual timestamp of .HEIC, not modification time

                var fileInfo = new FileInfo(path);
                return fileInfo.LastWriteTime;
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occured for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                postfix = "i";
                return default(DateTime);
            }
        }

        public string GetImageDevice(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                var ifd0Directory = ImageMetadataReader.ReadMetadata(path).OfType<ExifIfd0Directory>().FirstOrDefault();
                if (ifd0Directory == null)
                {
                    return "unknown";
                }

                var cameraModel = ifd0Directory.GetDescription(ExifDirectoryBase.TagModel);
                return cameraModel ?? "unknown";
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occured for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                return string.Empty;
            }
        }
    }
}