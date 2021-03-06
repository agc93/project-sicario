﻿using System.Collections.Generic;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets
{
    public interface IAssetTypeLoader
    {
        public string Name { get; }
        public IEnumerable<PropertyData> LoadData(AssetReader reader);
        // public AssetWriter AddData(AssetWriter writer, IEnumerable<PropertyData> additionalData);
        public AssetWriter RunInstructions(AssetWriter writer, IEnumerable<AssetInstruction> instructions);
    }
}