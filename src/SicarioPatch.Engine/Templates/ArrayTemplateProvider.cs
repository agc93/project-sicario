using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Engine.Fragments;

namespace SicarioPatch.Engine.Templates
{
    public class ArrayTemplateProvider : ITemplateProvider
    {
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers()
        {
            var arrayIndexing = Parsers.Between(Parsers.Terms.Char('['), Parsers.Terms.Integer(), Parsers.Terms.Char(']')).Then<IAssetParserFragment>(x => new ArrayFragment {MatchValue = (int) x});
            var arrayValueIndexing = Parsers.Between(Parsers.Terms.Char('[').And(Parsers.Terms.Char('[')), Parsers.Terms.Integer(),
                Parsers.Terms.Char(']').And(Parsers.Terms.Char(']'))).Then<IAssetParserFragment>(x => new ArrayPropertyFragment {MatchValue = (int?) x});
            var arrayFlatten = Parsers.Between(Parsers.Terms.Char('[').And(Parsers.Terms.Char('[')), Parsers.Terms.Char('*'),
                Parsers.Terms.Char(']').And(Parsers.Terms.Char(']'))).Then<IAssetParserFragment>(x => new ArrayFlattenFragment());
            return new List<Parser<IAssetParserFragment>> {arrayIndexing, arrayValueIndexing, arrayFlatten};
        }
    }
}