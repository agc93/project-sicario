using System.Collections.Generic;
using System.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.TypeLoaders
{
    public class RawTypeLoader : IAssetTypeLoader
    {
        public string Name => "raw";
        public IEnumerable<PropertyData> LoadData(AssetReader reader, string? parameter) {
            var cats = reader.categories.Where(c => c is NormalCategory {Data: { }}).Cast<NormalCategory>();
            if (string.IsNullOrWhiteSpace(parameter)) {
                return cats.SelectMany(nc => nc.Data);
            } else if (int.TryParse(parameter, out var idx) && cats.ElementAtOrDefault(idx) is not null) {
                return cats.ElementAt(idx).Data;
            }
            return new List<PropertyData>();
        }

        public AssetWriter RunInstructions(AssetWriter writer, IEnumerable<AssetInstruction> instructions) {
            return writer;
        }
    }
}