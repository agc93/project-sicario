using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.Fragments
{
    public class AnyFragment : IAssetParserFragment
    {
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput;
        }
    }
}