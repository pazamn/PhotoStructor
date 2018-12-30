using System.Collections.Generic;
using PhotoStructurer.Helpers.Readers;
using PhotoStructurer.Interfaces;

namespace PhotoStructurer.Data
{
    public static class SupportedData
    {
        public static string Prefix = "LIFE";

        public static readonly Dictionary<string, IFileReader> ImageExtensions = new Dictionary<string, IFileReader>
        {
            { ".jpg",  new ImageReader() },
            { ".png",  new ImageReader() },
            { ".heic", new AppleReader() },
            { ".mp4",  new VideoReader() },
        };
    }
}