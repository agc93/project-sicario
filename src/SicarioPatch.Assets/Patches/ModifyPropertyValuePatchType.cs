using System;
using System.Collections.Generic;
using System.Globalization;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    public record ValueModification
    {
        public string? ValueType { get; set; }
        public Func<decimal, decimal> RunValueChange { get; init; }
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
                            intProp.Value =
                                Convert.ToInt32(Math.Min(parsedValue.RunValueChange(Convert.ToDecimal(intProp.Value)), int.MaxValue));
                        }
                        break;
                    case "FloatProperty":
                        if (propertyData is FloatPropertyData floatProp && (parsedValue.ValueType ?? "FloatProperty") == floatProp.Type) {
                            floatProp.Value =
                                Convert.ToSingle(Math.Min(parsedValue.RunValueChange(Convert.ToDecimal(floatProp.Value)), Convert.ToDecimal(float.MaxValue)));
                        }
                        break;
                }
            }

            return new List<AssetInstruction>();
        }

        protected override Parser<ValueModification> ValueParser => Parsers.Terms.Identifier().Then(id => id.ToString())
            .Or(Parsers.Terms.Char('*').Then(c => string.Empty)).AndSkip(Parsers.Terms.Char(':'))
            .And(NumericModifierParser).Then(res => new ValueModification
                {ValueType = string.IsNullOrWhiteSpace(res.Item1) ? null : res.Item1, RunValueChange = res.Item2});

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
        

        private Parser<decimal> NumberParser =>
            Parsers.Terms.Decimal().Or(Parsers.Terms.Integer().Then(Convert.ToDecimal));
    }
}