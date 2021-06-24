using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public interface IAssetParserFragment
    {
        // public PropertyData Match(PropertyData input);
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput);
    }
}