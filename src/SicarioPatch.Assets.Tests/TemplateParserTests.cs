using System;
using System.Collections.Generic;
using SicarioPatch.Assets.TypeLoaders;
using Xunit;

namespace SicarioPatch.Assets.Tests
{
    public class TemplateParserTests
    {
        private readonly IEnumerable<IAssetTypeLoader> _typeLoaders;
        private readonly IEnumerable<ITemplateProvider> _templateProviders;

        public TemplateParserTests(IEnumerable<IAssetTypeLoader> typeLoaders, IEnumerable<ITemplateProvider> templateProviders) {
            _typeLoaders = typeLoaders;
            _templateProviders = templateProviders;
        }

        private TemplateParser GetParser() {
            return new TemplateParser(_typeLoaders, _templateProviders);
        }
        
        
        [Fact]
        public void ParseDatatableType() {
            var templateValue = "datatable:{'BaseStats*'}.{'CanUseAoA*'}";

            var parser = GetParser();
            var result = parser.ParseTemplate(templateValue);
            
            Assert.NotNull(result.TypeLoader);
            Assert.NotNull(result.Fragments);
            Assert.NotEmpty(result.Fragments);
            Assert.Null(result.LoaderParameter);
            Assert.IsType<DataTableTypeLoader>(result.TypeLoader);
        }

        [Fact]
        public void ParseRawType() {
            var templateValue = "raw:['MercenaryModeSpawn'].<BoolProperty>";

            var templateParser = GetParser();
            var result = templateParser.ParseTemplate(templateValue);

            Assert.NotNull(result.TypeLoader);
            Assert.Equal("raw", result.TypeLoader.Name);
            Assert.IsType<RawTypeLoader>(result.TypeLoader);
        }

        [Fact]
        public void ThrowsOnUnknownType() {
            var templateValue = "nonexistent:['MercenarySpawn']";

            var templateParser = GetParser();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var result = templateParser.ParseTemplate(templateValue);
            });
        }

        [Fact]
        public void ThrowsOnBadParameterFormat() {
            var templateValue = "raw[wrong]:['Mercenary']";
            var templateParser = GetParser();

            Assert.Throws<ArgumentException>(() =>
            {
                var _ = templateParser.ParseTemplate(templateValue);
            });
        }

        [Fact]
        public void IncludesParameter() {
            var templateValue = "raw(identifier):['MercenarySpawn']";

            var templateParser = GetParser();
            var result = templateParser.ParseTemplate(templateValue);
            
            Assert.NotNull(result.TypeLoader);
            Assert.Equal("raw", result.TypeLoader.Name);
            Assert.NotNull(result.LoaderParameter);
            Assert.Equal("identifier", result.LoaderParameter);
        }

        [Fact]
        public void IncludesQuotedParameter() {
            const string templateValue = "datatable('some_value()'):['Mercenary']";

            var templateParser = GetParser();
            var result = templateParser.ParseTemplate(templateValue);
            
            Assert.Equal("datatable", result.TypeLoader?.Name);
            Assert.Equal("some_value()", result.LoaderParameter);
            Assert.NotEmpty(result.Fragments);
        }
    }
}