using System;

namespace PhotoStructor.Interfaces
{
    public interface IFileReader
    {
        string Prefix { get; }

        DateTime GetImageData(string path, out string postfix);
        string GetImageDevice(string path);
    }
}