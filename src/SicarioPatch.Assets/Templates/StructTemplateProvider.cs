using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.Fragments;

namespace SicarioPatch.Assets.Templates
{
    public class StructTemplateProvider : ITemplateProvider
    {
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers() {
            return new List<Parser<IAssetParserFragment>> {
                Parsers.Between(Parsers.Terms.Char('['),
                        Parsers.ZeroOrOne(Parsers.Terms.Char('!')).And(Parsers.Terms.String(StringLiteralQuotes.SingleOrDouble)),
                        Parsers.Terms.Char(']'))
                    .Then<IAssetParserFragment>(x => new StructFragment {
                        MatchValue = x.Item2.ToString(), InvertMatch = x.Item1 == '!'
                    }),
                Parsers.Between(Parsers.Terms.Char('{'), Parsers.Terms.String(StringLiteralQuotes.SingleOrDouble), Parsers.Terms.Char('}'))
                    .Then<IAssetParserFragment>(x => new StructPropertyFragment {MatchValue = x.ToString()})
            };
        }
    }
}