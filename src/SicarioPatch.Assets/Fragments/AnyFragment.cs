using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class AnyFragment : IAssetParserFragment
    {
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput;
        }
    }
}