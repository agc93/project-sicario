using System;
using System.Collections.Generic;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.TypeLoaders
{
    public class AssetInstruction
    {
        public InstructionType Type { get; init; }
        public IEnumerable<PropertyData> Properties { get; init; } = new List<PropertyData>();

        public Dictionary<string, PropertyData> FixedProperties { get; init; } = new Dictionary<string, PropertyData>();
        // public Action<AssetWriter> WriterAction { get; init; }
    }
}