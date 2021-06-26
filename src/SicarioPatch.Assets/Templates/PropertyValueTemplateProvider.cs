using System;
using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.Fragments;
using static Parlot.Fluent.Parsers;

namespace SicarioPatch.Assets.Templates
{
    public class PropertyValueTemplateProvider : ITemplateProvider
    {
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers() {
            var propType = Between(Terms.Char('<'),
                Terms.Identifier().Then<IAssetParserFragment>(id => new PropertyValueFragment
                    {PropertyType = id.ToString()}),
                Terms.Char('>'));
            var propValue = Between(Terms.Char('<'), Terms.Identifier().AndSkip(Terms.Char('=')).And(TemplateParser.ValueInputParser.Or(Terms.Char('*').Then(c => c.ToString())))
                .Then<IAssetParserFragment>(x =>
                    new PropertyValueFragment
                        {PropertyType = x.Item1.ToString(), PropertyValue = x.Item2.ToString()}), Terms.Char('>'));

            var numberPropValue = Between(Terms.Char('<'),
                    Terms.Identifier().AndSkip(Terms.Char('=')).And(
                        Separated(Terms.Char('|'),Terms.Integer().Then(Convert.ToDouble).Or(Terms.Decimal().Then(Convert.ToDouble)))), Terms.Char('>'))
                .Then<IAssetParserFragment>(result => new NumberCollectionValueFragment(result.Item1.ToString()) {AllowedValues = result.Item2});
            return new List<Parser<IAssetParserFragment>> {propType, propValue, numberPropValue};
        }
    }
}