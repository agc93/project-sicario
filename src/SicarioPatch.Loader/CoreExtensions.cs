using System.IO;
using Spectre.Console.Cli;

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

        internal static bool Is(this FlagValue<bool> flagValue, bool target, bool defaultValue = false) {
            return flagValue.IsSet ? flagValue.Value == target : defaultValue == target;
        }
    }
}