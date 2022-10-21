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
    }
}