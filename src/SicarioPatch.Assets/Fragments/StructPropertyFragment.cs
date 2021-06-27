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
        public PropertyData Match(PropertyData input) {
            if (input is StructPropertyData structProp && MatchValue != null) {
                return structProp.Value.FirstOrDefault(v =>
                    PartialMatch ? v.Name.StartsWith(MatchValue.TrimEnd('*')) : v.Name == MatchValue);
            }

            throw new InvalidDataException("Invalid parse operation: non-struct input data!");
        }

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(v => v is StructPropertyData).Cast<StructPropertyData>().SelectMany(v =>
                v.Value.Where(vs =>
                    PartialMatch ? vs.Name.StartsWith(MatchValue.TrimEnd('*')) : vs.Name == MatchValue));
        }
    }
}