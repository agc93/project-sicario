using System;
using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Patches
{
    public class DuplicateItemPatchType : AssetPatchType<(string SourceName, int SourceIndex, string TargetName, int Index)>
    {
        public override string Type => "duplicateEntry";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> inputProperties,
            (string SourceName, int SourceIndex, string TargetName, int Index) parsedValue) {
            var newOutcomes = new Dictionary<string, PropertyData>();
            var properties = inputProperties.Where(ip => ip is StructPropertyData).Cast<StructPropertyData>()
                .Where(v => v.Name == parsedValue.SourceName).ToList();
            foreach (var property in properties) {
                if (properties.IndexOf(property) == parsedValue.SourceIndex) {
                    var newProp = new StructPropertyData(parsedValue.TargetName, property.Asset)
                        {Value = property.Value};
                    newOutcomes.Add(parsedValue.Index > 0 ? parsedValue.Index.ToString() : parsedValue.TargetName,
                        newProp);
                }
            }

            return new[] {new AssetInstruction {Type = InstructionType.Add, Properties = newOutcomes}};
        }

        protected internal override Parser<(string SourceName, int SourceIndex, string TargetName, int Index)> ValueParser => 
            Parsers.Terms.String()
                .And(Parsers.ZeroOrOne(Parsers.Terms.Char('[').SkipAnd(Parsers.Terms.Integer()).AndSkip(Parsers.Terms.Char(']'))).Then(zo => Convert.ToInt32(zo)))
                .AndSkip(Parsers.Terms.Char('>'))
                .And(Parsers.Terms.String())
                .And(Parsers.ZeroOrOne(Parsers.Terms.Char('(').SkipAnd(Parsers.Terms.Integer()).AndSkip(Parsers.Terms.Char(')'))).Then(zo => Convert.ToInt32(zo)))
                .Then(x => (SourceName: x.Item1.ToString(), SourceIndex: x.Item2, TargetName: x.Item3.ToString(), Index: x.Item4));
    }
}