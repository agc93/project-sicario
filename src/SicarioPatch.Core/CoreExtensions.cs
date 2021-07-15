using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HexPatch;

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

        public static IEnumerable<Patch> GetAllPatches(this Mod mod)
        {
            var allPatches = mod.FilePatches.SelectMany(fp => fp.Value).SelectMany(ps => ps.Patches);
            return allPatches;
        }
        
        public static float NextFloat(this Random rand, float minValue, float maxValue, int decimalPlaces = 1)
        {
            var randNumber = rand.NextDouble() * (maxValue - minValue) + minValue;
            return Convert.ToSingle(randNumber.ToString("f" + decimalPlaces));
        }

        public static byte[] ToValueBytes(this string s, bool addTerminator = false) {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(s);
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            return lengthByte.Concat(strBytes).Concat(new byte[1] {0}).ToArray();
        }
        
        public static string ToValueBytes(this string s, out int byteLength) {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(s);
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            byteLength = strBytes.Length + 1;
            return BitConverter.ToString(lengthByte.Concat(strBytes).ToArray());
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