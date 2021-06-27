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
        public static Parser<string> ValueInputParser => Terms.String().Then(ts => ts.ToString()).Or(Terms.Decimal().Then(d => d.ToString()))
                .Or(Terms.Integer().Then(i => i.ToString()));
        private readonly IEnumerable<IAssetTypeLoader> _loaders;
        private readonly IEnumerable<ITemplateProvider> _providers;
        private Parser<(IAssetTypeLoader?, List<IAssetParserFragment>)> _parser;

        public TemplateParser(IEnumerable<IAssetTypeLoader> loaders, IEnumerable<ITemplateProvider> templateProviders) {
            _loaders = loaders;
            _providers = templateProviders;
        }
        public Parser<(IAssetTypeLoader?, List<IAssetParserFragment>)> GetParser() {
            var any = Between(Terms.Char('['), Terms.Char('*'), Terms.Char(']'))
                .Then(x => new AnyFragment());
            var flattening = Between(Terms.Char('{'), Terms.Char('*').And(Terms.Char('*')), Terms.Char('}'))
                .Then(x => new FlattenFragment());

            
            var propName = Terms.Identifier().And(ZeroOrOne(Terms.Char('*')));
            var directProp = Terms.Identifier().And(ZeroOrOne(Terms.Char('*'))).Then(x => new RawPropertyFragment());

            var defaultOpts = any
                .Or<AnyFragment, FlattenFragment, IAssetParserFragment>(flattening);
            /*var opts = nameIndexing
                .Or<StructFragment, AnyFragment, IAssetParserFragment>(any)
                .Or<IAssetParserFragment, StructPropertyFragment, IAssetParserFragment>(nameValueIndexing)
                .Or<IAssetParserFragment, ArrayPropertyFragment, IAssetParserFragment>(arrayValueIndexing)
                .Or<IAssetParserFragment, ArrayFragment, IAssetParserFragment>(arrayIndexing)
                .Or<IAssetParserFragment, NumberCollectionValueFragment, IAssetParserFragment>(numberPropValue)
                .Or<IAssetParserFragment, EnumValueFragment, IAssetParserFragment>(enumType)
                .Or<IAssetParserFragment, PropertyValueFragment, IAssetParserFragment>(propValue)
                .Or<IAssetParserFragment, PropertyValueFragment, IAssetParserFragment>(propType);*/

            var fragments = _providers.SelectMany(p => p.GetParsers()).Aggregate(defaultOpts, (frag, opt) => opt.Or(frag));
            
            var all = Separated(Terms.Text("."), fragments);
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