using System.Collections.Generic;
using System.Linq;
using HexPatch;
using Microsoft.Extensions.Configuration;

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
    }
}