using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
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
    public class PropertyValueFragment : IAssetParserFragment
    {
        public string PropertyType { get; init; }
        public string PropertyValue { get; init; }
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            if (PropertyType == null) {
                throw new InvalidOperationException("Invalid parse operation: no property criteria set!");
            }

            foreach (var propertyData in initialInput.Where(pd => pd.Type == PropertyType)) {
                if (string.IsNullOrWhiteSpace(PropertyValue)) {
                    yield return propertyData;
                }
                else if (PropertyType == "ByteProperty" && propertyData is BytePropertyData bpData) {
                    if (propertyData.Asset.SearchHeaderReference(PropertyValue) == bpData.Value) {
                        yield return propertyData;
                    }
                    else {
                        yield break;
                    }
                }
                else if (PropertyValue == "*" && !string.IsNullOrWhiteSpace(propertyData.RawValue.ToString()) &&
                         propertyData.RawValue.ToString() != "0") {
                    yield return propertyData;
                }
                else if (PropertyValue == propertyData.RawValue.ToString()) {
                    yield return propertyData;
                }
            }
        }
    }

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