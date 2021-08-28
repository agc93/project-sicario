using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class StructMatchFragment : IAssetParserFragment
    {
        public string PropertyName { get; init; }
        public string? Type { get; init; }
        public string PropertyValue { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            var matches = initialInput.Where(pd => pd is StructPropertyData).Cast<StructPropertyData>().Where(pd =>
            {
                var typeMatch = string.IsNullOrWhiteSpace(Type) || Type == "*" || pd.StructType == Type;
                var thisMatch = typeMatch && pd.Value.Any(pdv =>
                    pdv.Matches(v => v.Name, PropertyName) && pdv.Matches(v => v.ToValueString(), PropertyValue, true));
                return thisMatch;
            });
            var found = matches.ToList();
            return matches;
        }
    }

    public static class PropertyDataExtensions
    {
        internal static bool Matches<T>(this T v, Func<T, string> propertyFunc, string matchValue, bool allowWildcardMatch = false) {
            var wildcardMatch = allowWildcardMatch && matchValue == "*" && string.IsNullOrWhiteSpace(propertyFunc(v));
            var partialMatch = matchValue != null && matchValue.EndsWith("*");
            return wildcardMatch || partialMatch ? propertyFunc(v).StartsWith(matchValue.TrimEnd('*')) : propertyFunc(v) == matchValue;
        }

        internal static string? ToValueString<T>(this T v, Func<T, object> objFunc = null) where T : PropertyData {
            var initial = v.ToString();
            objFunc ??= obj => obj.RawValue;
            var obj = objFunc(v);
            if (!string.IsNullOrWhiteSpace(initial) && initial != v.GetType().ToString() && initial != obj.GetType().ToString()) {
                return initial;
            }
            var objValue = obj.ToString();
            if (!string.IsNullOrWhiteSpace(objValue) && objValue != obj.GetType().ToString()) {
                return objValue;
            }
            return null;
        }
    }
}