using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.Fragments;
using static Parlot.Fluent.Parsers;

namespace SicarioPatch.Assets.Templates
{
    public class StructTemplateProvider : ITemplateProvider
    {
        private static Parser<char> OpenBrace => Terms.Char('{');
        private static Parser<char> CloseBrace => Terms.Char('}');
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers() {
            return new List<Parser<IAssetParserFragment>> {
                Between(Terms.Char('['),
                        ZeroOrOne(Terms.Char('!')).And(Terms.String()),
                        Terms.Char(']'))
                    .Then<IAssetParserFragment>(x => new StructFragment {
                        MatchValue = x.Item2.ToString(), InvertMatch = x.Item1 == '!'
                    }),
                Between(
                    OpenBrace, 
                    ZeroOrOne(Terms.Identifier().AndSkip(Terms.Char(':'))).And(Parsers.Between(OpenBrace, Terms.String().AndSkip(Terms.Char('=')).And(Terms.String()), CloseBrace)), 
                    CloseBrace
                ).Then<IAssetParserFragment>(res => new StructMatchFragment {
                    Type = res.Item1.ToString(),
                    PropertyName = res.Item2.Item1.ToString(),
                    PropertyValue = res.Item2.Item2.ToString()
                }),
                Between(OpenBrace, Terms.String(), CloseBrace)
                    .Then<IAssetParserFragment>(x => new StructPropertyFragment {MatchValue = x.ToString()})
            };
        }
    }
}