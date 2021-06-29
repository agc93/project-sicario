using System.Collections.Generic;
using System.Linq;
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
}