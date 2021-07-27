using System;

namespace PhotoStructor.Interfaces
{
    public interface IFileReader
    {
        string Prefix { get; }

        DateTime GetImageData(string path);
        string GetImageDevice(string path);
    }
}