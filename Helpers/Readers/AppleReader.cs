using System;
using System.IO;
using PhotoStructor.Data;
using PhotoStructor.Interfaces;
using PhotoStructurer.Helpers;

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

                //todo-AD Get actual timestamp of .HEIC, not modification time

                var fileInfo = new FileInfo(path);
                return fileInfo.LastWriteTime;
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"\tException occured for file:\r\n\t{path}", ConsoleColor.Red);
                ConsoleHelper.WriteLine($"\t{e.Message}", ConsoleColor.Red);
                ConsoleHelper.WriteLine();

                return default(DateTime);
            }
        }
    }
}