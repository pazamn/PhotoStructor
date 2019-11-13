using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.QuickTime;
using PhotoStructor.Interfaces;

namespace PhotoStructor.Helpers.Readers
{
    public class VideoReader : IFileReader
    {
        public string Prefix => "VID";

        public DateTime GetImageData(string path, out string postfix)
        {
            try
            {
                postfix = string.Empty;
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}-\d{6}"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}-\d{6}").Value;
                    throw new NotImplementedException("Implement it here.");
                    //return new ImageData
                    //{
                    //    OriginalFilePath = path,
                    //    ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch.Replace("-", "_")}_Video"
                    //};
                }

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}_\d{6}"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}_\d{6}").Value;
                    throw new NotImplementedException("Implement it here.");
                    //return new ImageData
                    //{
                    //    OriginalFilePath = path,
                    //    ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch}_Video"
                    //};
                }

                var metadata = ImageMetadataReader.ReadMetadata(path);
                var quickTimeDirectory = metadata.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
                if (quickTimeDirectory != null)
                {
                    var dateTimeString = quickTimeDirectory.Tags.FirstOrDefault(x => x.Name == "Created")?.Description;
                    if (string.IsNullOrEmpty(dateTimeString))
                    {
                        throw new FileLoadException("Date taken value is not defined in EXIF IFD0 of the file");
                    }

                    var dateTime = DateTime.ParseExact(dateTimeString, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.CurrentCulture);
                    return dateTime;
                }
                
                var fileInfo = new FileInfo(path);
                return fileInfo.LastWriteTime;
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occured for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                postfix = string.Empty;
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