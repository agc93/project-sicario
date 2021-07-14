using System.Collections.Generic;
using System.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.TypeLoaders
{
    public class RawTypeLoader : IAssetTypeLoader
    {
        public string Name => "raw";
        public IEnumerable<PropertyData> LoadData(AssetReader reader) {
            return reader.categories.Where(c => c is NormalCategory {Data: { }}).Cast<NormalCategory>()
                .SelectMany(nc => nc.Data);
            // return  ? normal.Data : new List<PropertyData>();
        }

        public AssetWriter RunInstructions(AssetWriter writer, IEnumerable<AssetInstruction> instructions) {
            return writer;
        }
    }
}