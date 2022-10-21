using System;
using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Engine.Fragments;
using static Parlot.Fluent.Parsers;
using static SicarioPatch.Engine.Patches.PropertyParsers;

namespace SicarioPatch.Engine
{
    public class PatchTemplateContext
    {
        public IAssetTypeLoader? TypeLoader { get; init; }
        public string? LoaderParameter { get; init; }
        public IEnumerable<IAssetParserFragment> Fragments { get; init; }
    }
    public class TemplateParser
    {
        public static Parser<string> ValueInputParser => Terms.String().Then(ts => ts.ToString()).Or(Terms.Decimal().Then(d => d.ToString()))
                .Or(Terms.Integer().Then(i => i.ToString()));
        private readonly IEnumerable<IAssetTypeLoader> _loaders;
        private readonly IEnumerable<ITemplateProvider> _providers;
        private Parser<PatchTemplateContext> _parser;

        public TemplateParser(IEnumerable<IAssetTypeLoader> loaders, IEnumerable<ITemplateProvider> templateProviders) {
            _loaders = loaders;
            _providers = templateProviders;
        }

        private static Parser<IAssetParserFragment> GetDefaultFragments() {
            var any = Between(Terms.Char('['), Terms.Char('*'), Terms.Char(']'))
                .Then(x => new AnyFragment());
            var flattening = Between(Terms.Char('{'), Terms.Char('*').And(Terms.Char('*')), Terms.Char('}'))
                .Then(x => new FlattenFragment());

            var defaultOpts = any
                .Or<AnyFragment, FlattenFragment, IAssetParserFragment>(flattening);
            return defaultOpts;
        }

        private static Parser<(string, string)> GetTypeLoaderParser() {
            var typeLoader = Terms.Identifier().Then(id => id.ToString())
                .And(ZeroOrOne(Brackets("()",
                    Terms.Identifier().Or(Terms.String(StringLiteralQuotes.Single))
                        .Then(s => s.ToString().Trim('\'')))
                ));
            return typeLoader;
        }
        
        public Parser<PatchTemplateContext> GetParser() {

            var defaultOpts = GetDefaultFragments();
            var fragments = _providers.SelectMany(p => p.GetParsers()).Aggregate(defaultOpts, (frag, opt) => opt.Or(frag));
            
            var allParts = Separated(Terms.Text("."), fragments);
            
            var typeLoader = GetTypeLoaderParser();

            var combined = typeLoader
                .AndSkip(Terms.Char(':'))
                .And(allParts)
                .Then(res => new PatchTemplateContext {TypeLoader = GetTypeLoader(res.Item1.ToString()), Fragments = res.Item3, LoaderParameter = res.Item2?.ToString()});

            return combined;
        }

        private IAssetTypeLoader? GetTypeLoader(string name) {
            return _loaders.FirstOrDefault(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public Parser<PatchTemplateContext> Parser
        {
            get
            {
                _parser ??= GetParser().Compile();
                return _parser;
            }
        }

        public PatchTemplateContext ParseTemplate(string templateStr) {
            var parser = Parser;
            var templateContext = parser.Parse(templateStr);
            if (templateContext == null) {
                throw new ArgumentException("Invalid template value!");
            }
            if (templateContext.TypeLoader == null) {
                throw new InvalidOperationException($"No supported loader for the requested data type!");
            }
            return templateContext;
        } 
    }
}