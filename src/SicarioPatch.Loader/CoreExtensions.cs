using System.IO;

namespace SicarioPatch.Loader
{
    internal static class CoreExtensions
    {
        internal static bool EnsureDirectoryExists(this string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            return Directory.Exists(path);
        }
    }
}