using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
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
                else switch (PropertyType) {
                    case "ArrayProperty" when propertyData is ArrayPropertyData apData:
                    {
                        if (int.TryParse(PropertyValue, out var arrayCount) && apData.Value.Length == arrayCount) {
                            yield return propertyData;
                        }
                        else {
                            yield break;
                        }

                        break;
                    }
                    case "ByteProperty" when propertyData is BytePropertyData bpData:
                    {
                        if (propertyData.Asset.SearchHeaderReference(PropertyValue) == bpData.Value) {
                            yield return propertyData;
                        }
                        else {
                            yield break;
                        }

                        break;
                    }
                    default:
                    {
                        if (PropertyValue == "*" && !string.IsNullOrWhiteSpace(propertyData.RawValue.ToString()) &&
                            propertyData.RawValue.ToString() != "0") {
                            yield return propertyData;
                        }
                        else if (string.Equals(PropertyValue, propertyData.RawValue.ToString(), StringComparison.CurrentCultureIgnoreCase)) {
                            yield return propertyData;
                        }

                        break;
                    }
                }
            }
        }
    }
}