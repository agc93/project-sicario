namespace SicarioPatch.Core
{
    internal static class CoreExtensions
    {
        internal static string ToArgument(this string path) {
            return path.Contains(' ')
                ? $"\"{path}\""
                : path;
        }
    }
}