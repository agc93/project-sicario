using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets
{
    public interface IAssetPatchType
    {
        string Type { get; }
        IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, string inputValue);
    }
    
    public abstract class AssetPatchType<T> : IAssetPatchType
    {
        public abstract string Type { get; }
        public IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, string inputValue) {
            var parsed = ValueParser.Parse(inputValue);
            return RunPatch(propData, parsed);
        }

        protected abstract IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, T parsedValue);

        protected internal abstract Parser<T> ValueParser { get; }
    }
}