using System;
using System.Globalization;
using System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using PhotoStructurer.Data;
using PhotoStructurer.Interfaces;

namespace PhotoStructurer.Helpers.Readers
{
    public class AppleReader : IFileReader
    {
        public ImageData ReadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"File cannot be found by path: {path}");
                }

                //todo-AD Get actual timestamp of .HEIC, not modification time

                var fileInfo = new FileInfo(path);
                return new ImageData
                {
                    OriginalFilePath = path,
                    ModifiedFileName = $"{SupportedData.Prefix}_{fileInfo.LastWriteTime:yyyyMMdd_HHmmss}_iPhone"
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