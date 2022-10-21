using System;
using System.Linq;
using Parlot.Fluent;

namespace SicarioPatch.Engine.Patches
{
    public static class PropertyParsers
    {
        internal static Parser<string> PropertyType(params string[] types) {
            return Parsers.OneOf(types.Select(t => Parsers.Terms.Text(t.EndsWith("Property") ? t : $"{t}Property")).ToArray());
        }
        
        internal static Parser<decimal> NumberParser =>
            Parsers.Terms.Decimal().Or(Parsers.Terms.Integer().Then(Convert.ToDecimal));

        internal static Parser<int> IntParser =>
            Parsers.Terms.Integer().Then(Convert.ToInt32).Or(Parsers.Terms.Decimal().Then(Convert.ToInt32));

        internal static Parser<TParser> Brackets<TParser>(string bracketFormat, Parser<TParser> valueParsers) {
            var openChar = bracketFormat.First();
            var closeChar = bracketFormat.Last();
            return Parsers.Between(Parsers.Terms.Char(openChar), valueParsers, Parsers.Terms.Char(closeChar));
        }

        internal static Parser<Range> Range(char separator = '-') {
            return Parsers.Separated(Parsers.Terms.Char(separator), NumberParser)
                .Then(res => new Range((Index)res[0], (Index)res[1]));
        }
    }
}