using System.Collections.Generic;
using System.Linq;
using SicarioPatch.Core;

namespace SicarioPatch.App
{
    public static class CoreExtensions
    {
        public static IDictionary<string, string> FallbackToDefaults(this IDictionary<string, string> dict,
            IEnumerable<PatchParameter> parameters)
        {
            if (dict.Any() && parameters.Any())
            {
                return dict.Select(kv =>
                {
                    if (string.IsNullOrWhiteSpace(kv.Value))
                    {
                        return (parameters.FirstOrDefault(p => p.Id == kv.Key) is var matchingParam &&
                                matchingParam != null)
                            ? new KeyValuePair<string, string>(kv.Key, matchingParam.Default)
                            : kv;
                    }

                    return kv;
                }).ToDictionary(k => k.Key, v => v.Value);
            }
            return dict;
        }

        public static void AddToOrder<TKey>(this Dictionary<TKey, int> dict, TKey obj, bool? state = null)
        {
            if (state.HasValue)
            {
                switch (state)
                {
                    case true when !dict.ContainsKey(obj):
                        dict.Add(obj, 10);
                        break;
                    case false when dict.ContainsKey(obj):
                        dict.Remove(obj);
                        break;
                }
            }
            else
            {
                if (!dict.ContainsKey(obj))
                {
                    dict.Add(obj, 10);
                }
                else
                {
                    dict.Remove(obj);
                }
            }
        }
        
        public static void Move<T>(this List<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1 && newIndex > -1 && newIndex < list.Count)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > list.Count) newIndex = list.Count;
                    // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }

        }
    }
}