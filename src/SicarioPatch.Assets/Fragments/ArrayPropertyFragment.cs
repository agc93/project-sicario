using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class ArrayPropertyFragment : IAssetParserFragment
    {
        public int? MatchValue { get; init; }
        public PropertyData Match(PropertyData input) {
            if (MatchValue == null || input is not ArrayPropertyData arrData) {
                throw new InvalidOperationException("Invalid parse operation: non-array input data!");
            }
            return arrData.Value[MatchValue.Value];
        }

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            var propertyDatas = initialInput as PropertyData[] ?? initialInput.ToArray();
            if (propertyDatas.All(i => i is ArrayPropertyData)) {
                return propertyDatas.Select(i =>
                {
                    var arrIn = i as ArrayPropertyData;
                    
                    return arrIn.Value.ElementAt((int) MatchValue);
                });
            }
            return new[] {propertyDatas.ElementAt((int) MatchValue)};
        }
    }
}