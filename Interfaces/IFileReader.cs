using PhotoStructurer.Data;

namespace PhotoStructurer.Interfaces
{
    public interface IFileReader
    {
        ImageData ReadFile(string path);
    }
}