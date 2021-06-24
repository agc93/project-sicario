using System;
using System.Collections.Generic;
using System.Linq;
using Parlot;
using Parlot.Fluent;
using SicarioPatch.Assets.Fragments;
using UAssetAPI.PropertyTypes;
using static Parlot.Fluent.Parsers;

namespace SicarioPatch.Assets
{
    public class TemplateParser
    {
        private readonly IEnumerable<IAssetTypeLoader> _loaders;
        private Parser<(IAssetTypeLoader?, List<IAssetParserFragment>)> _parser;

        public TemplateParser(IEnumerable<IAssetTypeLoader> loaders) {
            _loaders = loaders;
        }
        public Parser<(IAssetTypeLoader?, List<IAssetParserFragment>)> GetParser() {
            var inputValParser = Terms.String().Then(ts => ts.ToString()).Or(Terms.Decimal().Then(d => d.ToString()))
                .Or(Terms.Integer().Then(i => i.ToString()));
            
            var propName = Terms.Identifier().And(ZeroOrOne(Terms.Char('*')));
            var nameIndexing = Between(Terms.Char('['), ZeroOrOne(Terms.Char('!')).And(Terms.String(StringLiteralQuotes.SingleOrDouble)), Terms.Char(']')).Then(x => new StructFragment {MatchValue = x.Item2.ToString(), InvertMatch = x.Item1 != null});
            var nameValueIndexing = Between(Terms.Char('{'), Terms.String(StringLiteralQuotes.SingleOrDouble), Terms.Char('}')).Then(x => new StructPropertyFragment {MatchValue = x.ToString()});
            var flattening = Between(Terms.Char('{'), Terms.Char('*').And(Terms.Char('*')), Terms.Char('}'))
                .Then(x => new FlattenFragment());
            var any = Between(Terms.Char('['), Terms.Char('*'), Terms.Char(']'))
                .Then(x => new AnyFragment());
            var arrayIndexing = Between(Terms.Char('['), Terms.Integer(), Terms.Char(']')).Then(x => new ArrayFragment {MatchValue = (int) x});
            var arrayValueIndexing = Between(Terms.Char('[').And(Terms.Char('[')), Terms.Integer(),
                Terms.Char(']').And(Terms.Char(']'))).Then(x => new ArrayPropertyFragment {MatchValue = (int?) x});
            var enumType = Between(Terms.Char('<'),
                Terms.Identifier().AndSkip(Terms.Char(':').And(Terms.Char(':'))).And(ZeroOrOne(Terms.Identifier()))
                    .Then(m => new EnumValueFragment {EnumType = m.Item1.ToString(), EnumValue = m.Item2.ToString()}),
                Terms.Char('>'));
                // this needs to parse a syntax like <EnumType::EnumMember> or <EnumType::> and then formulate that into an EnumValueFragment. Maybe <|whatever|> ?
            var propType = Between(Terms.Char('<'),
                Terms.Identifier().Then(id => new PropertyValueFragment {PropertyType = id.ToString()}),
                Terms.Char('>'));
            var propValue = Between(Terms.Char('<'), Terms.Identifier().AndSkip(Terms.Char('=')).And(inputValParser.Or(Terms.Char('*').Then(c => c.ToString())))
                .Then(x =>
                    new PropertyValueFragment
                        {PropertyType = x.Item1.ToString(), PropertyValue = x.Item2.ToString()}), Terms.Char('>'));

            var numberPropValue = Between(Terms.Char('<'),
                Terms.Identifier().AndSkip(Terms.Char('=')).And(
                    Separated(Terms.Char('|'),Terms.Integer().Then(Convert.ToDouble).Or(Terms.Decimal().Then(Convert.ToDouble)))), Terms.Char('>'))
                .Then(result => new NumberCollectionValueFragment(result.Item1.ToString()) {AllowedValues = result.Item2});

            var directProp = Terms.Identifier().And(ZeroOrOne(Terms.Char('*'))).Then(x => new RawPropertyFragment());

            var opts = nameIndexing
                .Or<StructFragment, AnyFragment, IAssetParserFragment>(any)
                .Or<IAssetParserFragment, StructPropertyFragment, IAssetParserFragment>(nameValueIndexing)
                .Or<IAssetParserFragment, ArrayPropertyFragment, IAssetParserFragment>(arrayValueIndexing)
                .Or<IAssetParserFragment, ArrayFragment, IAssetParserFragment>(arrayIndexing)
                .Or<IAssetParserFragment, NumberCollectionValueFragment, IAssetParserFragment>(numberPropValue)
                .Or<IAssetParserFragment, EnumValueFragment, IAssetParserFragment>(enumType)
                .Or<IAssetParserFragment, PropertyValueFragment, IAssetParserFragment>(propValue)
                .Or<IAssetParserFragment, PropertyValueFragment, IAssetParserFragment>(propType);
            var all = Separated(Terms.Text("."), opts);
            var combined = Terms.Identifier().Then(x => _loaders.FirstOrDefault(l => string.Equals(l.Name, x.ToString(), StringComparison.OrdinalIgnoreCase))).AndSkip(Terms.Char(':')).And(all);
            return combined;
        }

        public Parser<(IAssetTypeLoader?, List<IAssetParserFragment>)> Parser
        {
            get
            {
                _parser ??= GetParser().Compile();
                return _parser;
            }
        }

        public (IAssetTypeLoader Loader, List<IAssetParserFragment> Fragments) ParseTemplate(string templateStr) {
            var parser = Parser;
            var (assetTypeLoader, assetParserFragments) = parser.Parse(templateStr);
            if (assetTypeLoader == null) {
                throw new InvalidOperationException($"No supported loader for the requested data type!");
            }

            return (Loader: assetTypeLoader, Fragments: assetParserFragments);
        } 
    }
}