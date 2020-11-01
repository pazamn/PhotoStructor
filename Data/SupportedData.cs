using System.Collections.Generic;
using PhotoStructor.Helpers.Readers;
using PhotoStructor.Interfaces;

namespace PhotoStructor.Data
{
    public static class SupportedData
    {
        public static readonly Dictionary<string, IFileReader> ImageExtensions = new Dictionary<string, IFileReader>
        {
            { ".jpg",  new ImageReader() },
            { ".png",  new ImageReader() },
            { ".heic", new AppleReader() },
            { ".mp4",  new VideoReader() },
            { ".mov",  new VideoReader() },
        };
    }
}