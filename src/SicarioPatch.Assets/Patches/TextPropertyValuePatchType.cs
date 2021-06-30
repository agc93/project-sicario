using System;
using System.Collections.Generic;
using System.Linq;
using Parlot;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public class TextPropertyValuePatchType : AssetPatchType<KeyValuePair<string, string>>
    {
        public override string Type => "textProperty";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            KeyValuePair<string, string> parsedValue) {
            foreach (var textPropertyData in propData.Where(pd => pd.Type == "TextProperty" && pd is TextPropertyData).Cast<TextPropertyData>()) {
                textPropertyData.Value[^2] = parsedValue.Key;
                textPropertyData.Value[^1] = parsedValue.Value;
            }

            return new List<AssetInstruction>();
        }

        protected override Parser<KeyValuePair<string, string>> ValueParser => Parsers.Terms.Identifier()
            .Or(Parsers.Terms.String(StringLiteralQuotes.Single)).Or(Parsers.Terms.Char('*')
                .Then(c => new TextSpan(Guid.NewGuid().ToString("N").ToUpper())))
            .AndSkip(Parsers.Terms.Char(':')).And(Parsers.Terms.String(StringLiteralQuotes.Single)).Then(res =>
                new KeyValuePair<string, string>(res.Item1.ToString(), res.Item2.ToString()));
    }
}