using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoStructurer.Data;
using PhotoStructurer.Interfaces;

namespace PhotoStructurer.Helpers.Readers
{
    public class ImageReader : IFileReader
    {
        public ImageData ReadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                var metadata = ImageMetadataReader.ReadMetadata(path);
                var ifd0Directory = metadata.OfType<ExifIfd0Directory>().FirstOrDefault();
                if (ifd0Directory != null)
                {
                    var dateTimeString = ifd0Directory.GetDescription(ExifDirectoryBase.TagDateTime);
                    var model = ifd0Directory.GetDescription(ExifDirectoryBase.TagModel);

                    if (string.IsNullOrEmpty(model))
                    {
                        throw new FileLoadException("Camera model value is not defined in EXIF IFD0 of the file");
                    }

                    if (string.IsNullOrEmpty(dateTimeString))
                    {
                        throw new FileLoadException("Date taken value is not defined in EXIF IFD0 of the file");
                    }

                    var dateTimeRegexPattern = @"\d{4}:\d{2}:\d{2} \d{2}:\d{2}:\d{2}";
                    if (!Regex.IsMatch(dateTimeString, dateTimeRegexPattern))
                    {
                        throw new FileLoadException($"Date taken value is converted in wrong format. Expected: '{dateTimeRegexPattern}', but was: '{dateTimeString}'.");
                    }

                    var splitDateTimeStr = dateTimeString.Split(' ');
                    var date = DateTime.ParseExact(splitDateTimeStr[0], "yyyy:MM:dd", CultureInfo.CurrentCulture);

                    var splitTime = splitDateTimeStr[1].Split(':');
                    var time = new TimeSpan(Convert.ToInt32(splitTime[0]), Convert.ToInt32(splitTime[1]), Convert.ToInt32(splitTime[2]));

                    var dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
                    return new ImageData
                    {
                        OriginalFilePath = path,
                        ModifiedFileName = $"{SupportedData.Prefix}_{dateTime:yyyyMMdd_HHmmss}_{model}"
                    };
                }
                
                if (Regex.IsMatch(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}-\d{6}"))
                {
                    var dateTimeMatch = Regex.Match(Path.GetFileNameWithoutExtension(path) ?? string.Empty, @"\d{8}-\d{6}").Value;

                    return new ImageData
                    {
                        OriginalFilePath = path,
                        ModifiedFileName = $"{SupportedData.Prefix}_{dateTimeMatch.Replace("-", "_")}_Unknown"
                    };
                }

                var fileInfo = new FileInfo(path);
                return new ImageData
                {
                    OriginalFilePath = path,
                    ModifiedFileName = $"{SupportedData.Prefix}_{fileInfo.LastWriteTime:yyyyMMdd_HHmmss}_Unknown"
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