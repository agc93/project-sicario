using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ModEngine.Core;

namespace SicarioPatch.Core
{
    public static class CoreExtensions
    {
        internal static string ToArgument(this string path) {
            return path.Contains(' ')
                ? $"\"{path}\""
                : path;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            return pairs.ToDictionary(k => k.Key, v => v.Value);
        }

        public static IEnumerable<Patch> GetFilePatches(this WingmanMod mod)
        {
            var allPatches = mod.FilePatches.SelectMany(fp => fp.Value).SelectMany(ps => ps.Patches);
            return allPatches;
        }

        public static string GetParentDirectoryPath(this FileInfo fi) {
            return fi.Directory?.FullName ?? Path.GetDirectoryName(fi.FullName);
        }

        //this is a horrible hack but otherwise the build process mutates the original mod selection
        internal static List<WingmanMod> RebuildModList(this List<WingmanMod> sourceList) {
            var json = JsonSerializer.Serialize(sourceList);
            return JsonSerializer.Deserialize<List<WingmanMod>>(json);
        }
    }
}