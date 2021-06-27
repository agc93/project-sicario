using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class AnyFragment : IAssetParserFragment
    {
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput;
        }
    }
    public class FlattenFragment : IAssetParserFragment
    {
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(x => x is StructPropertyData).Cast<StructPropertyData>()
                .SelectMany(spd => spd.Value);
        }
    }

    public class ArrayFragment : IAssetParserFragment
    {
        public int MatchValue { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return new[] {initialInput.ElementAt(MatchValue)};
        }
    }
}