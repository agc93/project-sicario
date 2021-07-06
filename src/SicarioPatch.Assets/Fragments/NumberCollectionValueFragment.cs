using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class NumberCollectionValueFragment : IAssetParserFragment
    {
        public NumberCollectionValueFragment(string propertyType) {
            PropertyType = propertyType;
        }
        
        public string PropertyType { get; }
        public List<double> AllowedValues { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            if (PropertyType == null || AllowedValues == null || !AllowedValues.Any()) {
                throw new InvalidOperationException("Invalid parse operation: no property criteria set!");
            }

            foreach (var propertyData in initialInput.Where(pd => pd.Type == PropertyType)) {
                if (double.TryParse(propertyData.RawValue.ToString(), out var dValue) && AllowedValues.Any(av => av == dValue)) {
                    yield return propertyData;
                }
            }
        }
    }
}