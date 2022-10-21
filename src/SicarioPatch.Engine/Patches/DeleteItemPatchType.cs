using System;
using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Engine.TypeLoaders;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.Patches
{
    [Obsolete("Completely untested, and currently without sample use case", false)]
    public class DeleteItemPatchType : AssetPatchType<IEnumerable<string>>
    {
        public override string Type => "deleteEntry";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            IEnumerable<string> parsedValue) {
            return propData.Where(pd => parsedValue.Any(pv => pd.Name == pv)).Select(pd => new AssetInstruction
                {Type = InstructionType.Remove, Properties = new() {[pd.Name] = pd}});
        }

        protected internal override Parser<IEnumerable<string>> ValueParser => Parsers
            .Separated(Parsers.Terms.Char(','), Parsers.Terms.String(StringLiteralQuotes.Single))
            .Then(res => res.Select(ts => ts.ToString()));
    }
}