using System.Collections.Generic;
using SicarioPatch.Engine.TypeLoaders;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine
{
    public interface IAssetTypeLoader
    {
        public string Name { get; }
        public IEnumerable<PropertyData> LoadData(AssetReader reader, string? parameter);
        // public AssetWriter AddData(AssetWriter writer, IEnumerable<PropertyData> additionalData);
        public AssetWriter RunInstructions(AssetWriter writer, IEnumerable<AssetInstruction> instructions);
    }
}