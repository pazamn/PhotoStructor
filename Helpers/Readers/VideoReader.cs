using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using PhotoStructurer.Data;
using PhotoStructurer.Interfaces;

namespace PhotoStructurer.Helpers.Readers
{
    public class VideoReader : IFileReader
    {
        public ImageData ReadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}-\d{6}"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}-\d{6}").Value;

                    return new ImageData
                    {
                        OriginalFilePath = path,
                        ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch.Replace("-", "_")}_Video"
                    };
                }

                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}_\d{6}"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}_\d{6}").Value;

                    return new ImageData
                    {
                        OriginalFilePath = path,
                        ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch}_Video"
                    };
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
                    return new ImageData
                    {
                        OriginalFilePath = path,
                        ModifiedFileName = $"{SupportedData.Prefix}_{dateTime:yyyyMMdd_HHmmss}_Video"
                    };
                }
                
                var fileInfo = new FileInfo(path);
                return new ImageData
                {
                    OriginalFilePath = path,
                    ModifiedFileName = $"{SupportedData.Prefix}_{fileInfo.LastWriteTime:yyyyMMdd_HHmmss}_Video"
                };
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occured for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                return null;
            }
        }
    }
}