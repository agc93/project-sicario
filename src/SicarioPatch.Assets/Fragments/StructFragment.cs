using System.Collections.Generic;
using System.IO;
using System.Linq;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class StructFragment : IAssetParserFragment
    {
        public string? MatchValue { get; init; }
        public bool InvertMatch { get; init; } = false;
        private bool PartialMatch => MatchValue != null && MatchValue.EndsWith("*");
        public PropertyData Match(PropertyData input) {
            if (input is StructPropertyData structProp && MatchValue != null) {
                return structProp.Value.FirstOrDefault(v =>
                    PartialMatch ? v.Name.StartsWith(MatchValue.TrimEnd('*')) : v.Name == MatchValue);
            }

            throw new InvalidDataException("Invalid parse operation: non-struct input data!");
        }

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(v =>
                InvertMatch ? !Matches(v) : Matches(v));
        }

        private bool Matches(PropertyData v) {
            return PartialMatch ? v.Name.StartsWith(MatchValue.TrimEnd('*')) : v.Name == MatchValue;
        }
    }
}