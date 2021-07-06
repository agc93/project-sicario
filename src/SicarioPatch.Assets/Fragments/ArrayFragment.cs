using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class ArrayFragment : IAssetParserFragment
    {
        public int MatchValue { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return (initialInput.ElementAtOrDefault(MatchValue) is { } matchProp)
                ? new[] {matchProp}
                : new List<PropertyData>();
        }
    }
}