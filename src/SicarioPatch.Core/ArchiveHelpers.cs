using System;
using System.IO;
using System.IO.Compression;

namespace SicarioPatch.Core
{
    public static class ArchiveHelpers
    {
        public static FileInfo ToZipFile(this DirectoryInfo dirInfo)
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.zip");
            ZipFile.CreateFromDirectory(dirInfo.FullName, path);
            return new FileInfo(path);
        }
    }
}