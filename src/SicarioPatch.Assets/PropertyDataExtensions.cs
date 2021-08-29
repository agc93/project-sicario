using System;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets
{
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