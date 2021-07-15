using System;
using System.Collections.Generic;
using System.Linq;
using HexPatch;
using Microsoft.Extensions.Configuration;
using SicarioPatch.Components;

namespace SicarioPatch.App.Shared
{
    public static class DisplayExtensions
    {
        public static int ToFileCount(this IEnumerable<Mod> mods)
        {
            return mods.SelectMany(m =>
                m.FilePatches.Keys.ToList()).Distinct().Count();
        }

        public static bool GetDocsPath(this IConfiguration config, out string docsPath, string keyName = "DocsPath")
        {
            var key = config.GetValue<string>(keyName, string.Empty);
            docsPath = key;
            return !string.IsNullOrWhiteSpace(docsPath);
        }

        public static string ToolName(this IBrandProvider brand, NameFormat format = NameFormat.Normal) {
            var toolName = $"{brand.AppName} {brand.ToolName}";
            var acronym = string.Join(string.Empty,
                toolName.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(s => s[0])
            );
            return format switch {
                NameFormat.Normal => toolName,
                NameFormat.Short => acronym,
                NameFormat.Long => $"{toolName} ({acronym})",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }

    public enum NameFormat
    {
        Normal,
        Short,
        Long
    }
}