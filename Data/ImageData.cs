using System.Diagnostics;
using System.IO;

namespace PhotoStructurer.Data
{
    [DebuggerDisplay("{OriginalFileName} -> {ModifiedFullFileName}")]
    public class ImageData
    {
        public string OriginalFilePath { get; set; }

        public string OriginalFileName => Path.GetFileName(OriginalFilePath);

        public string Extension => (Path.GetExtension(OriginalFilePath) ?? string.Empty).ToLowerInvariant();

        public string ModifiedFileName { get; set; }

        public int ModifiedFileConflictNumber { get; set; }

        public string ModifiedFilePostfix => ModifiedFileConflictNumber <= -1
                                           ? string.Empty
                                           : $"_{ModifiedFileConflictNumber}";

        public string ModifiedFullFileName => $"{ModifiedFileName.Trim()}{ModifiedFilePostfix.Trim()}_{Path.GetFileNameWithoutExtension(OriginalFileName)}{Extension}";

        public string ModifiedFilePath => Path.Combine(Path.GetDirectoryName(OriginalFilePath) ?? string.Empty, ModifiedFullFileName);
    }
}