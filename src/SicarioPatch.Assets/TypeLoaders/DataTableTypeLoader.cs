using System.Collections.Generic;
using System.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.TypeLoaders
{
    public class DataTableTypeLoader : IAssetTypeLoader
    {
        public string Name => "datatable";
        public IEnumerable<PropertyData> LoadData(AssetReader reader) {
            var rows = (reader.categories[0] as DataTableCategory).Data2.Table;
            return rows.Select(r => r.Data);
        }

        public AssetWriter AddData(AssetWriter writer, IEnumerable<PropertyData> additionalData) {
            foreach (var structData in additionalData.Where(ad => ad is StructPropertyData).Cast<StructPropertyData>()) {
                writer.data.AddHeaderReference(structData.Name);
                (writer.data.categories[0] as DataTableCategory).Data2.Table.Add(new DataTableEntry(structData, 0));
            }

            return writer;
        }
    }
}