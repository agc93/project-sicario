using System;
using System.Linq;
using Parlot.Fluent;

namespace SicarioPatch.Assets.Patches
{
    public static class PropertyParsers
    {
        internal static Parser<string> PropertyType(params string[] types) {
            return Parsers.OneOf(types.Select(t => Parsers.Terms.Text(t.EndsWith("Property") ? t : $"{t}Property")).ToArray());
        }
        
        internal static Parser<decimal> NumberParser =>
            Parsers.Terms.Decimal().Or(Parsers.Terms.Integer().Then(Convert.ToDecimal));
    }
}