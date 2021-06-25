using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.Fragments;
using static Parlot.Fluent.Parsers;

namespace SicarioPatch.Assets.Templates
{
    public class EnumTemplateProvider : ITemplateProvider
    {
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers() {
            return new[] {Between(Terms.Char('<'),
                Terms.Identifier().AndSkip(Terms.Char(':').And(Terms.Char(':'))).And(ZeroOrOne(Terms.Identifier()))
                    .Then<IAssetParserFragment>(m => new EnumValueFragment {EnumType = m.Item1.ToString(), EnumValue = m.Item2.ToString()}),
                Terms.Char('>'))};
            // this needs to parse a syntax like <EnumType::EnumMember> or <EnumType::> and then formulate that into an EnumValueFragment. Maybe <|whatever|> ?
            
        }
    }
}