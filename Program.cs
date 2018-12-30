using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhotoStructurer.Data;
using PhotoStructurer.Helpers;
using PhotoStructurer.Interfaces;

namespace PhotoStructurer
{
    public class Program
    {
        //todo-AD Upload it to github

        public static void Main(string[] folders)
        {
            try
            {
                ConsoleHelper.WriteLine($"Start {nameof(PhotoStructurer)}", ConsoleColor.Cyan);
                Console.WriteLine();

                ConsoleHelper.WriteLine("Press any key to continue...", ConsoleColor.Gray);
                Console.ReadKey();

                if (!folders.Any())
                {
                    throw new Exception("There are no parameterized folders via command line arguments.");
                }

                ConsoleHelper.WriteLine("Folders:", ConsoleColor.Green);
                foreach (var folder in folders)
                {
                    ConsoleHelper.WriteLine($"\t{folder}", ConsoleColor.Gray);
                }

                ConsoleHelper.WriteLine();
                foreach (var folder in folders)
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
                var photosToMove = currentDirPhotos.Where(x => !(Path.GetFileNameWithoutExtension(x) ?? string.Empty).StartsWith($"{SupportedData.Prefix}_"));
                foreach (var item in photosToMove)
                {
                    photos.Add(item, extension.Value);
                }
            }

            ConsoleHelper.WriteLine($"Found {photos.Count} photos.", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

            var modifications = new List<ImageData>();
            foreach (var photo in photos)
            {
                var imageData = photo.Value.ReadFile(photo.Key);
                if (imageData == null)
                {
                    ConsoleHelper.WriteLine($"\tSkipped: {photo}", ConsoleColor.Yellow);
                    continue;
                }

                var conflictingFiles = modifications.Where(x => x.ModifiedFileName == imageData.ModifiedFileName).ToList();
                if (conflictingFiles.Any())
                {
                    var maxConflictedNumber = conflictingFiles.Select(x => x.ModifiedFileConflictNumber).Max();
                    imageData.ModifiedFileConflictNumber = maxConflictedNumber + 1;
                }

                modifications.Add(imageData);
                ConsoleHelper.WriteLine($"\tAdded: {photo}", ConsoleColor.Gray);
            }

            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine($"Calculated names for {photos.Count} modifications.", ConsoleColor.Green);
            ConsoleHelper.WriteLine();

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

            ConsoleHelper.WriteLine();
            ConsoleHelper.WriteLine("Folder finished", ConsoleColor.Green);
            ConsoleHelper.WriteLine();
        }
    }
}