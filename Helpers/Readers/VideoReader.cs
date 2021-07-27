using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using PhotoStructor.Interfaces;

namespace PhotoStructor.Helpers.Readers
{
    public class VideoReader : IFileReader
    {
        public string Prefix => "VID";

        public DateTime GetImageData(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"^\d{8}-\d{6}$"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"^\d{8}-\d{6}$").Value;
                    throw new NotImplementedException("Implement it here.");
                    //return new ImageData
                    //{
                    //    OriginalFilePath = path,
                    //    ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch.Replace("-", "_")}_Video"
                    //};
                }

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"^\d{8}_\d{6}$"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"^\d{8}_\d{6}$").Value;
                    throw new NotImplementedException("Implement it here.");
                    //return new ImageData
                    //{
                    //    OriginalFilePath = path,
                    //    ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch}_Video"
                    //};
                }

                var metadata = ImageMetadataReader.ReadMetadata(path);

                var metadataDirectory = metadata.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();
                if (metadataDirectory != null && !string.IsNullOrEmpty(metadataDirectory.GetDescription(7)))
                {
                    var dateTimeString = metadataDirectory.GetDescription(7);
                    var model = metadataDirectory.GetDescription(22);

                    if (string.IsNullOrEmpty(dateTimeString))
                    {
                        throw new FileLoadException("Date taken value is not defined in EXIF IFD0 of the file");
                    }

                    if (string.IsNullOrEmpty(model))
                    {
                        throw new FileLoadException("Camera model value is not defined in EXIF IFD0 of the file");
                    }
                    
                    var dateTime = DateTime.Parse(dateTimeString);
                    return dateTime;
                }

                var quickTimeDirectory = metadata.OfType<QuickTimeMovieHeaderDirectory>().FirstOrDefault();
                if (quickTimeDirectory != null)
                {
                    var dateTimeString = quickTimeDirectory.Tags.FirstOrDefault(x => x.Name == "Created")?.Description;
                    if (string.IsNullOrEmpty(dateTimeString))
                    {
                        throw new FileLoadException("Date taken value is not defined in EXIF IFD0 of the file");
                    }

                    var originalDateTime = DateTime.ParseExact(dateTimeString, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.CurrentCulture);
                    var dateTime = TimeZone.CurrentTimeZone.ToLocalTime(originalDateTime);

                    if (dateTime.Year >= 2000)
                    {
                        return dateTime;
                    }
                }
                
                var fileInfo = new FileInfo(path);
                return fileInfo.LastWriteTime;
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occurred for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();
                
                return default;
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

                var metadataDirectory = ImageMetadataReader.ReadMetadata(path).OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();
                if (metadataDirectory == null)
                {
                    return "unknown";
                }

                var cameraModel = metadataDirectory.GetDescription(22);
                return cameraModel ?? "unknown";
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occurred for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                return string.Empty;
            }
        }
    }
}