using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Parlot;
using Parlot.Fluent;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets
{
    public class DuplicatePropertyPatchType : AssetPatchType<(string SourceName, string TargetName)>
    {
        public override string Type => "duplicateProperty";
        protected override IEnumerable<PropertyData> RunPatch(IEnumerable<PropertyData> inputProperties, (string SourceName, string TargetName) parsedValue) {
            foreach (var property in inputProperties.Where(ip => ip is StructPropertyData).Cast<StructPropertyData>()) {
                var inputMatch = property.Value.FirstOrDefault(v => v.Name == parsedValue.SourceName);
                if (inputMatch != null) {
                    // inputMatch.Name = parsedValue.TargetName;
                    property.Value.Add(inputMatch);
                    var dupe = property.Value.Last(v => v.Name == parsedValue.SourceName);
                    dupe.Name = parsedValue.TargetName;
                }
            }

            // return inputProperties;
            return new List<PropertyData>();
        }

        protected override Parser<(string SourceName, string TargetName)> ValueParser => 
            Parsers.Terms.String()
            .AndSkip(Parsers.Terms.Char('>'))
            .And(Parsers.Terms.String())
            .Then<(string SourceName, string TargetName)>(x => (SourceName: x.Item1.ToString(), TargetName: x.Item2.ToString()) );
    }

    public class PropertyValuePatchType : AssetPatchType<(string ValueType, string Value)>
    {
        public override string Type => "propertyValue";

        protected override IEnumerable<PropertyData> RunPatch(IEnumerable<PropertyData> propData, (string ValueType, string Value) parsedValue) {
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
                            floatProp.Value = float.Parse(parsedValue.Value);
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
                    case "ByteProperty":
                        if (propertyData.Type == "ByteProperty" && propertyData is BytePropertyData byteProp) {
                            byteProp.Value = byteProp.Asset.SearchHeaderReference(parsedValue.Value);
                        }
                        break;
                }
            }

            return new List<PropertyData>();
        }

        protected override Parser<(string ValueType, string Value)> ValueParser => Parsers.Terms.Identifier().AndSkip(Parsers.Terms.Char(':'))
            // .And(Parsers.Terms.String(StringLiteralQuotes.SingleOrDouble))
            .And(Parsers.Terms.Identifier().Then(id => id.ToString()).Or(Parsers.Terms.Decimal().Then(d => d.ToString(CultureInfo.InvariantCulture))).Or(Parsers.Terms.String(StringLiteralQuotes.Single).Then(x => x.ToString())))
            .Then(x => (ValueType: x.Item1.ToString(), Value: x.Item2.ToString()));
    }
}