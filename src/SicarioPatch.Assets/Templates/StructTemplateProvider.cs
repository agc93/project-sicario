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
                        MatchValue = x.Item2.ToString(), InvertMatch = !string.IsNullOrWhiteSpace(x.Item1.ToString())
                    }),
                Parsers.Between(Parsers.Terms.Char('{'), Parsers.Terms.String(StringLiteralQuotes.SingleOrDouble), Parsers.Terms.Char('}'))
                    .Then<IAssetParserFragment>(x => new StructPropertyFragment {MatchValue = x.ToString()})
            };
        }
    }
}