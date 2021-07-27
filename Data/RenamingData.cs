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

        public string UserDefinedPostfix { get; set; }

        public string ModifiedFilePostfix => ModifiedFileConflictNumber > 0  && !string.IsNullOrEmpty(UserDefinedPostfix) ? $"_{UserDefinedPostfix}{ModifiedFileConflictNumber}"
                                           : ModifiedFileConflictNumber <= 0 && !string.IsNullOrEmpty(UserDefinedPostfix) ? $"_{UserDefinedPostfix}"  
                                           : ModifiedFileConflictNumber > 0  && string.IsNullOrEmpty(UserDefinedPostfix)  ? $"{ModifiedFileConflictNumber}"
                                           : ModifiedFileConflictNumber <= 0 && string.IsNullOrEmpty(UserDefinedPostfix)  ? $""
                                           : $"_";

        public string ModifiedFullFileName => $"{ModifiedFileName.Trim()}{ModifiedFilePostfix.Trim()}{Extension}";

        public string ModifiedFilePath => Path.Combine(Path.GetDirectoryName(OriginalFilePath) ?? string.Empty, ModifiedFullFileName);
    }
}