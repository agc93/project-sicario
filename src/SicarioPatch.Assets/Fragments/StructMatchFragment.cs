using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class StructMatchFragment : IAssetParserFragment
    {
        public string PropertyName { get; init; }
        public string? Type { get; init; }
        public string PropertyValue { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            var matches = initialInput.Where(pd => pd is StructPropertyData).Cast<StructPropertyData>().Where(pd =>
            {
                var typeMatch = string.IsNullOrWhiteSpace(Type) || Type == "*" || pd.StructType == Type;
                var thisMatch = typeMatch && pd.Value.Any(pdv =>
                    pdv.Matches(v => v.Name, PropertyName) && pdv.Matches(v => v.ToValueString(), PropertyValue, true));
                return thisMatch;
            });
            return matches;
        }
    }
}