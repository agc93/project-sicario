using System.Collections.Generic;
using System.Linq;

namespace SicarioPatch.Core
{
    public static class WingmanModExtensions
    {
        public static List<string> GetFilesModified(this WingmanMod mod) {
            var fp = mod.FilePatches?.Keys.ToList();
            var ap = mod.AssetPatches?.Keys.ToList();
            return ap.Concat(fp).Distinct().ToList();
        }
        
        public static int GetPatchCount(this WingmanMod mod) {
            var allFilePatches = mod.FilePatches.SelectMany(fp => fp.Value).SelectMany(ps => ps.Patches).Count();
            var allPatches = mod.AssetPatches.SelectMany(fp => fp.Value).SelectMany(ps => ps.Patches).Count();
            return allFilePatches + allPatches;
        }
    }
}