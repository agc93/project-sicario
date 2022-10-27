using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine
{
    public static class CoreExtensions
    {
        internal static DataTableCategory GetDataTableCategory(this AssetWriter writer) {
            return writer.data.categories[0] as DataTableCategory;
        }

        internal static List<DataTableEntry> GetDataTable(this AssetWriter writer) {
            return writer.GetDataTableCategory().Data2.Table;
        }

        internal static decimal ClampTo(this decimal d, Range? inRange) {
            if (inRange != null) {
                var range = inRange.Value;
                if (d < range.Start.Value) {
                    d = range.Start.Value;
                }
                else if (d > range.End.Value) {
                    d = range.End.Value;
                }

                return d;
            }
            return d;
        }

        internal static IEnumerable<PropertyData> WhereNotIn<T, TValue>(this IEnumerable<PropertyData> inputData, IEnumerable<T> removalCandidates)
            where T : PropertyData<TValue> {
            return inputData.Where(v =>
                removalCandidates.All(iv => v is T data && !data.Value.Equals(iv.Value)));
        }
        
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