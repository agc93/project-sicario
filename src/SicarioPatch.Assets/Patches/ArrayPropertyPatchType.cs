using System;
using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public class ArrayPropertyPatchType : AssetPatchType<(string ItemValueType, List<string> Value)>
    {
        public override string Type => "arrayPropertyValue";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            (string ItemValueType, List<string> Value) parsedValue) {
            foreach (var propertyData in propData) {
                switch (parsedValue.ItemValueType) {
                    case "StrProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData {ArrayType: "StrProperty"} arrayProp) {
                            arrayProp.Value = parsedValue.Value.Select(v => new StrPropertyData
                                {Name = arrayProp.Name, Value = v, Type = "StrProperty"}).ToArray();
                        }
                        break;
                    case "IntProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData
                            {ArrayType: "IntProperty"} intArrayProp) {
                            intArrayProp.Value = parsedValue.Value.Select(v =>
                            {
                                var conv = Convert.ToInt32(double.Parse(v));
                                return new IntPropertyData {
                                    Name = intArrayProp.Name,
                                    Value = conv,
                                    Type = "IntProperty"
                                };
                            }).ToArray();
                        }
                        break;
                    case "NameProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData
                            {ArrayType: "NameProperty"} nameArrayProp) {
                            nameArrayProp.Value = parsedValue.Value.Select(v =>
                            {
                                var valData = new NamePropertyData(nameArrayProp.Name, nameArrayProp.Asset) {
                                    Value = v,
                                    Type = "NameProperty",
                                };
                                nameArrayProp.Asset.AddHeaderReference(v);
                                return valData;
                            }).ToArray();
                            
                        }
                        break;
                }
            }

            // return propData;
            return new List<AssetInstruction>();
        }

        protected override Parser<(string ItemValueType, List<string> Value)> ValueParser =>  
            Parsers.Terms.Identifier().AndSkip(Parsers.Terms.Char(':'))
                .And(Parsers.Between(Parsers.Terms.Char('['), Parsers.Separated(Parsers.Terms.Char(','), Parsers.Terms.String(StringLiteralQuotes.Single).Then(x => x.ToString()).Or(Parsers.Terms.Integer().Then(x => x.ToString()))), Parsers.Terms.Char(']')))
                .Then(x => (ValueType: x.Item1.ToString(), Value: x.Item2));
    }
}