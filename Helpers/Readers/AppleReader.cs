using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoStructor.Interfaces;

namespace PhotoStructor.Helpers.Readers
{
    public class AppleReader : IFileReader
    {
        public string Prefix => "IMG";

        public DateTime GetImageData(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                var metadata = ImageMetadataReader.ReadMetadata(path);
                var ifd0Directory = metadata.OfType<ExifIfd0Directory>().FirstOrDefault();
                if (ifd0Directory != null && !string.IsNullOrEmpty(ifd0Directory.GetDescription(ExifDirectoryBase.TagDateTime)))
                {
                    var dateTimeString = ifd0Directory.GetDescription(ExifDirectoryBase.TagDateTime);
                    var model = ifd0Directory.GetDescription(ExifDirectoryBase.TagModel);

                    if (string.IsNullOrEmpty(dateTimeString))
                    {
                        throw new FileLoadException("Date taken value is not defined in EXIF IFD0 of the file");
                    }

                    if (string.IsNullOrEmpty(model))
                    {
                        throw new FileLoadException("Camera model value is not defined in EXIF IFD0 of the file");
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
                    return dateTime;
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
                ConsoleHelper.WriteLine($"\tException occurred for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                return string.Empty;
            }
        }
    }
}