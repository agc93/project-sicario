using System.Collections.Generic;
using UAssetAPI;

namespace SicarioPatch.Assets
{
    public static class CoreExtensions
    {
        internal static DataTableCategory GetDataTableCategory(this AssetWriter writer) {
            return writer.data.categories[0] as DataTableCategory;
        }

        internal static List<DataTableEntry> GetDataTable(this AssetWriter writer) {
            return writer.GetDataTableCategory().Data2.Table;
        }
    }
}