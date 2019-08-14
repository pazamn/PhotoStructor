using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PhotoStructor.Data;
using PhotoStructor.Interfaces;
using PhotoStructurer.Helpers;

namespace PhotoStructor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ConsoleHelper.WriteLine($"Start {nameof(PhotoStructurer)}", ConsoleColor.Cyan);
                Console.WriteLine();

                ConsoleHelper.WriteLine("Press any key to continue...", ConsoleColor.Gray);
                Console.ReadKey();

                var folders = from arg in args
                              where arg.ToLowerInvariant() != "/rollback"
                              select arg;

                var foldersList = folders.ToList();
                if (!foldersList.Any())
                {
                    throw new Exception("There are no parameterized folders via command line arguments.");
                }

                ConsoleHelper.WriteLine("Folders:", ConsoleColor.Green);
                foreach (var folder in foldersList)
                {
                    ConsoleHelper.WriteLine($"\t{folder}", ConsoleColor.Gray);
                }

                ConsoleHelper.WriteLine();
                foreach (var folder in foldersList)
                {
                    ConvertFolder(folder);
                }

                ConsoleHelper.WriteLine("All folders converted.", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteLine($"Exception of type {e.GetType().Name} occured.", ConsoleColor.Red);
                ConsoleHelper.WriteLine(e.Message, ConsoleColor.Red);
                ConsoleHelper.WriteLine();
            }

            ConsoleHelper.WriteLine("Press any key for exit...", ConsoleColor.Gray);
            Console.ReadKey();
        }

        private static void ConvertFolder(string folderPath)
        {
            ConsoleHelper.WriteLine($"Start converting files in folder: {folderPath}", ConsoleColor.Green);

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Directory not found by path: {folderPath}");
            }

            var photos = new Dictionary<string, IFileReader>();
            foreach (var extension in SupportedData.ImageExtensions)
            {
                var currentDirPhotos = Directory.GetFiles(folderPath, $"*{extension.Key}", SearchOption.TopDirectoryOnly);
                foreach (var item in currentDirPhotos)
                {
                    photos.Add(item, extension.Value);
                }
            }

            ConsoleHelper.WriteLine($"Found {photos.Count} photos.", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            var modifications = new List<RenamingData>();
            foreach (var photo in photos)
            {
                var photoNameRegexPattern = $@"^{photo.Value.Prefix}_\d{{4}}\d{{2}}\d{{2}}_\d{{2}}\d{{2}}\d{{2}}";

                var photoName = Path.GetFileNameWithoutExtension(photo.Key) ?? string.Empty;
                if (Regex.IsMatch(photoName, photoNameRegexPattern))
                {
                    ConsoleHelper.WriteLine($"\tSkipped: {photo.Key}", ConsoleColor.DarkGray);
                    continue;
                }

                var creationTime = photo.Value.GetImageData(photo.Key);
                if (creationTime == default(DateTime))
                {
                    throw new Exception($"Creation value of file {photo.Key} is {DateTime.MinValue:yyyyMMdd_HHmmss}.");
                }

                var renamingData = new RenamingData
                {
                    OriginalFilePath = photo.Key,
                    ModifiedFileName = $"{photo.Value.Prefix}_{creationTime:yyyyMMdd_HHmmss}"
                };

                var conflictingFiles = modifications.Where(x => x.ModifiedFileName == renamingData.ModifiedFileName).ToList();
                if (conflictingFiles.Any())
                {
                    var maxConflictedNumber = conflictingFiles.Select(x => x.ModifiedFileConflictNumber).Max();
                    renamingData.ModifiedFileConflictNumber = maxConflictedNumber + 1;
                }

                modifications.Add(renamingData);
                ConsoleHelper.WriteLine($"\tAdded: {photo.Key}", ConsoleColor.Cyan);
            }

            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine($"Calculated names for {modifications.Count} modifications.", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            if (modifications.Any())
            {
                ConsoleHelper.WriteLine("Press any key to continue with the folder...", ConsoleColor.Gray);
                Console.ReadKey();

                foreach (var modification in modifications)
                {
                    if (File.Exists(modification.ModifiedFilePath))
                    {
                        ConsoleHelper.WriteLine($"\tNot moved: {modification.ModifiedFullFileName}", ConsoleColor.Red);
                        continue;
                    }

                    File.Move(modification.OriginalFilePath, modification.ModifiedFilePath);
                    ConsoleHelper.WriteLine($"\tMoved: {modification.OriginalFileName} -> {modification.ModifiedFullFileName}", ConsoleColor.Gray);
                }
            }

            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("Folder finished", ConsoleColor.Green);
            ConsoleHelper.WriteLine();
        }
    }
}