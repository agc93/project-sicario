using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.TypeLoaders
{
    public class AssetInstruction
    {
        public InstructionType Type { get; init; }
        public Dictionary<string, PropertyData> Properties { get; init; } = new Dictionary<string, PropertyData>();
    }
}