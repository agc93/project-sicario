using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Engine.TypeLoaders;
using UAssetAPI.PropertyTypes;
using static SicarioPatch.Engine.Patches.PropertyParsers;

namespace SicarioPatch.Engine.Patches
{
    public record ArrayModification
    {
        public enum Operation
        {
            Replace,
            Add,
            Remove
        }
        public string ItemValueType { get; init; }
        public List<string> Value { get; init; }
        public Operation Type { get; init; } = Operation.Replace;
    }
    public class ArrayPropertyPatchType : AssetPatchType<ArrayModification>
    {
        public override string Type => "arrayPropertyValue";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            ArrayModification parsedValue) {
            foreach (var propertyData in propData) {
                switch (parsedValue.ItemValueType) {
                    case "StrProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData {ArrayType: "StrProperty"} arrayProp) {
                            var prop = arrayProp;
                            var inputValues = parsedValue.Value.Select(v => new StrPropertyData
                                {Name = prop.Name, Value = v, Type = "StrProperty"});
                            arrayProp.Value = parsedValue.Type switch {
                                ArrayModification.Operation.Add => arrayProp.Value.Concat(inputValues).ToArray(),
                                ArrayModification.Operation.Remove => arrayProp.Value.Where(v =>
                                    inputValues.All(iv => iv.Value != (v as StrPropertyData)?.Value)).ToArray(),
                                ArrayModification.Operation.Replace => inputValues.ToArray(),
                                _ => throw new InvalidEnumArgumentException(nameof(ArrayModification.Type))
                            };
                        }
                        break;
                    case "IntProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData
                            {ArrayType: "IntProperty"} intArrayProp) {
                            var prop = intArrayProp;
                            var inputValues = parsedValue.Value.Select(v =>
                            {
                                var conv = Convert.ToInt32(double.Parse(v));
                                return new IntPropertyData {
                                    Name = prop.Name,
                                    Value = conv,
                                    Type = "IntProperty"
                                };
                            });
                            intArrayProp.Value = parsedValue.Type switch {
                                ArrayModification.Operation.Replace => inputValues.ToArray(),
                                ArrayModification.Operation.Add => intArrayProp.Value.Concat(inputValues).ToArray(),
                                ArrayModification.Operation.Remove => intArrayProp.Value
                                    .WhereNotIn<IntPropertyData, int>(inputValues).ToArray(),
                                _ => throw new InvalidEnumArgumentException(nameof(ArrayModification.Type))
                            };
                        }
                        break;
                    case "NameProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData
                            {ArrayType: "NameProperty"} nameArrayProp) {
                            var prop = nameArrayProp;
                            var inputValues =  parsedValue.Value.Select(v =>
                            {
                                var valData = new NamePropertyData(prop.Name, prop.Asset) {
                                    Value = v,
                                    Type = "NameProperty",
                                };
                                prop.Asset.AddHeaderReference(v);
                                return valData;
                            });
                            nameArrayProp.Value = parsedValue.Type switch {
                                ArrayModification.Operation.Replace => inputValues.ToArray(),
                                ArrayModification.Operation.Add => nameArrayProp.Value.Concat(inputValues).ToArray(),
                                ArrayModification.Operation.Remove => nameArrayProp.Value
                                    .WhereNotIn<NamePropertyData, string>(inputValues).ToArray(),
                                _ => throw new InvalidEnumArgumentException(nameof(ArrayModification.Type))
                            };
                            
                        }
                        break;
                    case "BoolProperty":
                        if (propertyData.Type == "ArrayProperty" && propertyData is ArrayPropertyData {
                            ArrayType: "BoolProperty"
                        } boolArrayProp) {
                            boolArrayProp.Value = parsedValue.Value.Select(v =>
                            {
                                var valData = new BoolPropertyData(boolArrayProp.Name, boolArrayProp.Asset) {
                                    Value = bool.Parse(v)
                                };
                                return valData;
                            }).ToArray();
                        }
                        break;
                }
            }

            // return propData;
            return new List<AssetInstruction>();
        }

        protected internal override Parser<ArrayModification> ValueParser =>  
            Parsers.Terms.Identifier().AndSkip(Parsers.Terms.Char(':'))
                .And(Parsers.ZeroOrOne(Parsers.Terms.Char('+').Or(Parsers.Terms.Char('-'))))
                .And(Brackets("[]", Parsers.Separated(Parsers.Terms.Char(','), Parsers.Terms.String(StringLiteralQuotes.Single).Then(x => x.ToString()).Or(Parsers.Terms.Integer().Then(x => x.ToString())))))
                .Then(x => new ArrayModification {Type = x.Item2 switch{ '+' => ArrayModification.Operation.Add, '-' => ArrayModification.Operation.Remove, _ => ArrayModification.Operation.Replace}, ItemValueType = x.Item1.ToString(), Value = x.Item3});
    }
}