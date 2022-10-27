using System.Collections.Generic;
using System.Linq;
using ModEngine.Core;
using SicarioPatch.Engine;

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
        
        public static IEnumerable<PatchParameter> WhereValid(this IEnumerable<PatchParameter> parameters) {
            return parameters.Where(p => !string.IsNullOrWhiteSpace(p?.Id));
        } 
        public static IDictionary<string, string> FallbackToDefaults(this IDictionary<string, string> dict,
            IEnumerable<PatchParameter> parameters)
        {
            var patchParameters = parameters as PatchParameter[] ?? parameters.ToArray();
            if (dict.Any() && patchParameters.Any())
            {
                return dict.Select(kv =>
                {
                    if (string.IsNullOrWhiteSpace(kv.Value))
                    {
                        return (patchParameters.FirstOrDefault(p => p.Id == kv.Key) is var matchingParam &&
                                matchingParam != null)
                            ? new KeyValuePair<string, string>(kv.Key, matchingParam.Default)
                            : kv;
                    }

                    return kv;
                }).ToDictionary(k => k.Key, v => v.Value);
            }
            return dict;
        }
    }
}