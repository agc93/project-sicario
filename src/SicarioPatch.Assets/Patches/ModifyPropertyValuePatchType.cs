using System;
using System.Collections.Generic;
using System.Globalization;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;
using SicarioPatch.Assets.TypeLoaders;
using static SicarioPatch.Assets.Patches.PropertyParsers;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public record ValueModification
    {
        public string? ValueType { get; set; }
        public Func<decimal, decimal> RunValueChange { get; init; }
        public Range? ValueRange { get; init; }
    }
    public class ModifyPropertyValuePatchType : AssetPatchType<ValueModification>
    {
        public override string Type => "modifyPropertyValue";

        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData,
            ValueModification parsedValue) {
            foreach (var propertyData in propData) {
                switch (propertyData.Type) {
                    case "IntProperty":
                        if (propertyData is IntPropertyData intProp && (parsedValue.ValueType ?? "IntProperty") == intProp.Type) {
                            var outValue = parsedValue.RunValueChange(Convert.ToDecimal(intProp.Value));
                            outValue.ClampTo(parsedValue.ValueRange);
                            intProp.Value =
                                Convert.ToInt32(Math.Min(outValue, int.MaxValue));
                        }
                        break;
                    case "FloatProperty":
                        if (propertyData is FloatPropertyData floatProp && (parsedValue.ValueType ?? "FloatProperty") == floatProp.Type) {
                            var outValue = parsedValue.RunValueChange(Convert.ToDecimal(floatProp.Value));
                            outValue = outValue.ClampTo(parsedValue.ValueRange);
                            floatProp.Value =
                                Convert.ToSingle(Math.Min(outValue, decimal.MaxValue));
                        }
                        break;
                }
            }

            return new List<AssetInstruction>();
        }

        protected internal override Parser<ValueModification> ValueParser => PropertyType("Int", "Float").Then(id => id.ToString())
            .Or(Parsers.Terms.Char('*').Then(c => string.Empty)).AndSkip(Parsers.Terms.Char(':'))
            .And(NumericModifierParser)
            .And(RangeParser)
            .Then(res => new ValueModification
                {ValueType = string.IsNullOrWhiteSpace(res.Item1) ? null : res.Item1, RunValueChange = res.Item2, ValueRange = res.Item3.Equals(default) ? null : res.Item3});

        private Parser<Range> RangeParser => ZeroOrOne(Brackets("()", Range()));
        private Parser<Func<decimal, decimal>> NumericModifierParser => Parsers.Terms.Char('+')
            .SkipAnd(NumberParser)
            .Then<Func<decimal, decimal>>(res => (arg) => arg + res)
            .Or(Parsers.Terms.Char('-').SkipAnd(NumberParser).Then<Func<decimal, decimal>>(res => arg => arg - res))
            .Or(Parsers.Terms.Char('/').SkipAnd(NumberParser).Then<Func<decimal, decimal>>(res => arg => arg / res))
            .Or(Parsers.Terms.Char('*').SkipAnd(NumberParser).Then<Func<decimal, decimal>>(res => arg => arg * res));

        private Parser<Func<string, string>> StringModifierParser =>
            Parsers.Terms.Char('+').SkipAnd(Parsers.Terms.String())
                .Then<Func<string, string>>(res => (arg) => arg + res)
                .Or(Parsers.Terms.Char('-').SkipAnd(Parsers.Terms.String())
                    .Then<Func<string, string>>(res => (arg) => arg.Replace(res.ToString(), string.Empty)));
    }
}