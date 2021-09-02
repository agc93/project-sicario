using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parlot;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public class TextPropertyValuePatchType : AssetPatchType<(string? Namespace, string Key, string Value)>
    {
        public override string Type => "textProperty";

        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, (string? Namespace, string Key, string Value) parsedValue) {
            foreach (var textPropertyData in propData.Where(pd => pd.Type == "TextProperty" && pd is TextPropertyData).Cast<TextPropertyData>()) {
                if (!string.IsNullOrWhiteSpace(parsedValue.Namespace)) {
                    textPropertyData.BaseBlankString = new UString(parsedValue.Namespace, Encoding.ASCII);
                }
                textPropertyData.Value[^2] = parsedValue.Key;
                textPropertyData.Value[^1] = parsedValue.Value;
            }

            return new List<AssetInstruction>();
        }

        protected internal override Parser<(string? Namespace, string Key, string Value)> ValueParser => Parsers.ZeroOrOne(Parsers.Terms.Identifier()).And(Parsers.Terms.Identifier()
            .Or(Parsers.Terms.String(StringLiteralQuotes.Single)).Or(Parsers.Terms.Char('*')
                .Then(c => new TextSpan(Guid.NewGuid().ToString("N").ToUpper())))
            .AndSkip(Parsers.Terms.Char(':')).And(Parsers.Terms.String())).Then(res =>
                (res.Item1.ToString(), res.Item2.Item1.ToString(), res.Item2.Item2.ToString()));
    }
}