using System.Collections.Generic;
using System.Linq;

namespace SicarioPatch.Core
{
    public static class SicarioModExtensions
    {
        public static List<string> GetFilesModified(this SicarioMod mod) {
            // var fp = mod.FilePatches?.Keys.ToList();
            var fp = new List<string>();
            var ap = mod.AssetPatches?.Keys.ToList();
            return ap.Concat(fp).Distinct().ToList();
        }
        
        public static int GetPatchCount(this SicarioMod mod) {
            // var allFilePatches = mod.FilePatches.SelectMany(fp => fp.Value).SelectMany(ps => ps.Patches).Count();
            var allFilePatches = 0;
            var allPatches = mod.AssetPatches.SelectMany(fp => fp.Value).SelectMany(ps => ps.Patches).Count();
            return allFilePatches + allPatches;
        }
    }
}