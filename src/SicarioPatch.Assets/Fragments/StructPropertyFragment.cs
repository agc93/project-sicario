using System.Collections.Generic;
using System.IO;
using System.Linq;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class StructPropertyFragment : IAssetParserFragment
    {
        public string? MatchValue { get; init; }
        private bool PartialMatch => MatchValue != null && MatchValue.EndsWith("*");

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(v => v is StructPropertyData).Cast<StructPropertyData>().SelectMany(v =>
                v.Value.Where(vs =>
                    PartialMatch ? vs.Name.StartsWith(MatchValue.TrimEnd('*')) : vs.Name == MatchValue));
        }
    }
}