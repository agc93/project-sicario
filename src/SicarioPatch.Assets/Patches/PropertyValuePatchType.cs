using System;
using System.Collections.Generic;
using System.Globalization;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public class PropertyValuePatchType : AssetPatchType<(string ValueType, string Value)>
    {
        public override string Type => "propertyValue";

        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            (string ValueType, string Value) parsedValue) {
            foreach (var propertyData in propData) {
                switch (parsedValue.ValueType) {
                    case "IntProperty":
                        if (propertyData.Type == "IntProperty" && propertyData is IntPropertyData intProp) {
                            intProp.Value = Convert.ToInt32(double.Parse(parsedValue.Value));
                            // intProp.Value = Int32.Parse(parsedValue.Value);
                        }
                        break;
                    case "FloatProperty":
                        if (propertyData.Type == "FloatProperty" && propertyData is FloatPropertyData floatProp) {
                            floatProp.Value = Convert.ToSingle(parsedValue.Value);
                        }
                        break;
                    case "BoolProperty":
                        if (propertyData.Type == "BoolProperty" && propertyData is BoolPropertyData boolProp) {
                            boolProp.Value = bool.Parse(parsedValue.Value);
                        }
                        break;
                    case "StrProperty":
                        if (propertyData.Type == "StrProperty" && propertyData is StrPropertyData strProp) {
                            strProp.Value = parsedValue.Value;
                        }
                        break;
                    case "TextProperty":
                        if (propertyData.Type == "TextProperty" && propertyData is TextPropertyData textProp) {
                            textProp.Value[^1] = parsedValue.Value;
                        }
                        break;
                    case "NameProperty":
                        if (propertyData.Type == "NameProperty" && propertyData is NamePropertyData nameProp) {
                            nameProp.Asset.AddHeaderReference(parsedValue.Value);
                            nameProp.Value = parsedValue.Value;
                        }
                        break;
                    case "ByteProperty":
                        if (propertyData.Type == "ByteProperty" && propertyData is BytePropertyData byteProp) {
                            var headerRef = 0;
                            try {
                                headerRef = byteProp.Asset.SearchHeaderReference(parsedValue.Value);
                            }
                            catch (HeaderOutOfRangeException e) {
                                headerRef = byteProp.Asset.AddHeaderReference(parsedValue.Value);
                            }

                            if (headerRef != 0) {
                                byteProp.Value = headerRef;
                            }
                        }
                        break;
                }
            }

            return new List<AssetInstruction>();
        }

        protected override Parser<(string ValueType, string Value)> ValueParser => Parsers.Terms.Identifier().AndSkip(Parsers.Terms.Char(':'))
            // .And(Parsers.Terms.String(StringLiteralQuotes.SingleOrDouble))
            .And(Parsers.Terms.Identifier().Then(id => id.ToString()).Or(Parsers.Terms.Decimal().Then(d => d.ToString(CultureInfo.InvariantCulture))).Or(Parsers.Terms.String(StringLiteralQuotes.Single).Then(x => x.ToString())))
            .Then(x => (ValueType: x.Item1.ToString(), Value: x.Item2.ToString()));
    }
}