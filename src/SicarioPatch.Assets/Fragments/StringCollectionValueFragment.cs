using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class StringCollectionValueFragment : IAssetParserFragment
    {
        public string PropertyType => "StrProperty";
        public List<string> AllowedValues { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            if (PropertyType == null || AllowedValues == null || !AllowedValues.Any()) {
                throw new InvalidOperationException("Invalid parse operation: no property criteria set!");
            }

            foreach (var propertyData in initialInput.Where(pd => pd.Type == PropertyType).Cast<StrPropertyData>()) {
                if (AllowedValues.Any(av => av == propertyData.Value)) {
                    yield return propertyData;
                }
            }
        }
    }
}