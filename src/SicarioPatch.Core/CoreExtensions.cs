using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}