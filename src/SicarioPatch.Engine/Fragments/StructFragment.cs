using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.Fragments
{
    public class StructFragment : IAssetParserFragment
    {
        public string? MatchValue { get; init; }
        public bool InvertMatch { get; init; } = false;
        private bool PartialMatch => MatchValue != null && MatchValue.EndsWith("*");

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(v =>
                InvertMatch ? !Matches(v) : Matches(v));
        }

        private bool Matches(PropertyData v) {
            return PartialMatch ? v.Name.StartsWith(MatchValue.TrimEnd('*')) : v.Name == MatchValue;
        }
    }
}