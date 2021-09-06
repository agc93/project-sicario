using System;
using System.Collections.Generic;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.TypeLoaders
{
    public class AssetInstruction
    {
        public InstructionType Type { get; init; }
        public Dictionary<string, PropertyData> Properties { get; init; } = new Dictionary<string, PropertyData>();
    }
}