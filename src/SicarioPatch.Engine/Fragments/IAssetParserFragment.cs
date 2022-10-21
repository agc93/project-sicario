using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.Fragments
{
    public interface IAssetParserFragment
    {
        // public PropertyData Match(PropertyData input);
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput);
    }
}