using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Patches
{
    public class DeleteItemPatchType : AssetPatchType<IEnumerable<string>>
    {
        public override string Type => "deleteEntry";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            IEnumerable<string> parsedValue) {
            return propData.Where(pd => parsedValue.Any(pv => pd.Name == pv)).Select(pd => new AssetInstruction
                {Type = InstructionType.Remove, Properties = new[] {pd}});
        }

        protected override Parser<IEnumerable<string>> ValueParser => Parsers
            .Separated(Parsers.Terms.Char(','), Parsers.Terms.String(StringLiteralQuotes.Single))
            .Then(res => res.Select(ts => ts.ToString()));
    }

    public class DuplicateItemPatchType : AssetPatchType<(string SourceName, string TargetName)>
    {
        public override string Type => "duplicateEntry";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> inputProperties,
            (string SourceName, string TargetName) parsedValue) {
            var newOutcomes = new List<PropertyData>();
            foreach (var property in inputProperties.Where(ip => ip is  StructPropertyData).Cast<StructPropertyData>().Where(v => v.Name == parsedValue.SourceName)) {
                // property.Name = parsedValue.TargetName;
                var newProp = new StructPropertyData(parsedValue.TargetName, property.Asset) {Value = property.Value};
                /*var newProp = new StructPropertyData {
                Name = parsedValue.TargetName,
                Value = property.Value,
                Type = "StructProperty",
                StructType = property.StructType
                };*/
                newOutcomes.Add(newProp);
                // property.Name = parsedValue.SourceName;
            }

            return new[] {new AssetInstruction {Type = InstructionType.Add, Properties = newOutcomes}};
        }

        protected override Parser<(string SourceName, string TargetName)> ValueParser => 
            Parsers.Terms.String()
                .AndSkip(Parsers.Terms.Char('>'))
                .And(Parsers.Terms.String())
                .Then(x => (SourceName: x.Item1.ToString(), TargetName: x.Item2.ToString()) );
    }
}