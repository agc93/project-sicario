using System;
using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public class DuplicateArrayItemPatchType : AssetPatchType<(int SourceIndex, int? TargetIndex)>
    {
        public override string Type => "duplicateArrayItem";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, (int SourceIndex, int? TargetIndex) parsedValue) {
            foreach (var propertyData in propData.Where(ip => ip is ArrayPropertyData).Cast<ArrayPropertyData>()) {
                var inputMatch = propertyData.Value.ElementAtOrDefault(parsedValue.SourceIndex);
                if (inputMatch != null) {
                    // inputMatch.Name = parsedValue.TargetName;
                    var arrayValues = propertyData.Value.ToList();
                    arrayValues.Insert(parsedValue.TargetIndex ?? arrayValues.Count - 1, inputMatch);
                    propertyData.Value = arrayValues.ToArray();
                }
            }

            return new List<AssetInstruction>();
        }

        protected override Parser<(int SourceIndex, int? TargetIndex)> ValueParser => Parsers.Terms.Integer()
            .And(Parsers.ZeroOrOne(Parsers.Terms.Char('>'))).And(Parsers.ZeroOrOne(Parsers.Terms.Integer()))
            .Then(res => (Convert.ToInt32(res.Item1), res.Item2 == '>' ? Convert.ToInt32(res.Item3) as int? : null));
    }
}