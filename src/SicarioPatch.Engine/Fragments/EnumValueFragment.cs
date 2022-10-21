using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.Fragments
{
    public class EnumValueFragment : IAssetParserFragment
    {
        public string EnumType { get; init; }
        public string EnumValue { get; init; }
        public string EnumString => $"{EnumType}::{EnumValue}";

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            if (string.IsNullOrWhiteSpace(EnumType)) {
                throw new InvalidOperationException("Invalid parse: no enum definition!");
            }

            foreach (var propertyData in initialInput.Where(pd => pd.Type == "ByteProperty" && pd is BytePropertyData).Cast<BytePropertyData>()) {
                if (propertyData.EnumType == propertyData.Asset.SearchHeaderReference(EnumType)) {
                    if (string.IsNullOrWhiteSpace(EnumValue)) {
                        yield return propertyData;
                    }
                    else if (propertyData.Value == propertyData.Asset.SearchHeaderReference(EnumString)) {
                        yield return propertyData;
                    }
                }
            }
        }
    }
}