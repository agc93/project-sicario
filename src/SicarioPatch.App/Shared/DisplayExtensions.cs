﻿using System;
using System.Collections.Generic;
using System.Linq;
using HexPatch;
using Microsoft.Extensions.Configuration;
using ModEngine.Core;
using SicarioPatch.Components;
using SicarioPatch.Core;

namespace SicarioPatch.App.Shared
{
    public static class DisplayExtensions
    {
        public static int ToFileCount(this IEnumerable<WingmanMod> mods) {
            var wingmanMods = mods as WingmanMod[] ?? mods.ToArray();
            return wingmanMods.SelectMany(m =>
                m.FilePatches.Keys.ToList()).Concat(wingmanMods.SelectMany(m => m.AssetPatches.Keys.ToList())).Distinct().Count();
        }

        public static bool GetDocsPath(this IConfiguration config, out string docsPath, string keyName = "DocsPath")
        {
            var key = config.GetValue<string>(keyName, string.Empty);
            docsPath = key;
            return !string.IsNullOrWhiteSpace(docsPath);
        }
        
    }
}