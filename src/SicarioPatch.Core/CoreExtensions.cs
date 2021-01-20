using System.Collections.Generic;
using System.Linq;

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
    }
}