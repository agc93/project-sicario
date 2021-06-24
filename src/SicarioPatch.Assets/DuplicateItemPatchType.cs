using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets
{
    public class DuplicateItemPatchType : AssetPatchType<(string SourceName, string TargetName)>
    {
        public override string Type => "duplicateEntry";
        protected override IEnumerable<PropertyData> RunPatch(IEnumerable<PropertyData> inputProperties, (string SourceName, string TargetName) parsedValue) {
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

            return newOutcomes;
        }

        protected override Parser<(string SourceName, string TargetName)> ValueParser => 
            Parsers.Terms.String()
                .AndSkip(Parsers.Terms.Char('>'))
                .And(Parsers.Terms.String())
                .Then<(string SourceName, string TargetName)>(x => (SourceName: x.Item1.ToString(), TargetName: x.Item2.ToString()) );
    }
}