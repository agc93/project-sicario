using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class FlattenFragment : IAssetParserFragment
    {
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(x => x is StructPropertyData).Cast<StructPropertyData>()
                .SelectMany(spd => spd.Value);
        }
    }
}