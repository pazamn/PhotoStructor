using System.Diagnostics;
using System.IO;

namespace PhotoStructor.Data
{
    [DebuggerDisplay("{OriginalFileName} -> {ModifiedFullFileName}")]
    public class RenamingData
    {
        public string OriginalFilePath { get; set; }

        public string OriginalFileName => Path.GetFileName(OriginalFilePath);

        public string Extension => (Path.GetExtension(OriginalFilePath) ?? string.Empty).ToLowerInvariant();

        public string ModifiedFileName { get; set; }

        public int ModifiedFileConflictNumber { get; set; }

        public bool DoRollback { get; set; }

        public string CameraModelPostfix { get; set; }

        public string ModifiedFilePostfix => ModifiedFileConflictNumber <= 0
                                           ? $"_{CameraModelPostfix}"
                                           : $"_{CameraModelPostfix}{ModifiedFileConflictNumber}";

        public string ModifiedFullFileName => $"{ModifiedFileName.Trim()}{ModifiedFilePostfix.Trim()}{Extension}";

        public string ModifiedFilePath => Path.Combine(Path.GetDirectoryName(OriginalFilePath) ?? string.Empty, ModifiedFullFileName);
    }
}