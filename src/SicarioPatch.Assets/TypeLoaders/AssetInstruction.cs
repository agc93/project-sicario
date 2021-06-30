using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.TypeLoaders
{
    public class AssetInstruction
    {
        public InstructionType Type { get; init; }
        public IEnumerable<PropertyData> Properties { get; init; }
    }
}