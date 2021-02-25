using System;
using System.ComponentModel;
using System.Linq;
using Fluid;
using Fluid.Values;
using HexPatch;

namespace SicarioPatch.Templating
{
    public static class PatchFilters
    {
        public static FluidValue FromString(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(input.ToStringValue())));
        }

        public static FluidValue FromInt(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(
                BitConverter.ToString(BitConverter.GetBytes(Convert.ToInt32(input.ToNumberValue()))));
        }

        public static FluidValue FromFloat(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(
                BitConverter.ToString(BitConverter.GetBytes(Convert.ToSingle(input.ToNumberValue()))));
        }

        public static FluidValue FromBool(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(BitConverter.GetBytes(bool.Parse(input.ToStringValue()))));
        }

        public static FluidValue FromShort(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(BitConverter.GetBytes(Convert.ToInt16(input.ToNumberValue()))));
        }

        public static FluidValue InvertBoolean(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var defaultState = arguments.Count > 0 && (bool.TryParse(arguments.At(0).ToStringValue(), out var def) && def);
            var resultValue = new StringValue(bool.TryParse(input.ToStringValue(), out var inputBool)
                ? (!inputBool).ToString()
                : input.ToStringValue());
            return resultValue;
        }

        public static FluidValue MultiplyValue(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            if (arguments.Count > 2)
            {
                var inValue = input.ToNumberValue();
                var baseFactor = arguments.At(0).ToNumberValue();
                var ampFactor = arguments.At(1).ToNumberValue();
                var threshold = arguments.At(2).ToNumberValue();
                var finalFactor = baseFactor * (inValue > threshold ? ampFactor : ampFactor - 1);
                return new StringValue((input.ToNumberValue() * finalFactor).ToString());
            }
            return new StringValue((input.ToNumberValue() * arguments.At(0).ToNumberValue()).ToString());
        }

        public static FluidValue AmplifyInput(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var threshold = arguments.Count > 1 ? arguments.At(1).ToNumberValue() : 100;
            var inputFactor = input.ToNumberValue();
            var amp = arguments.At(0).ToNumberValue();
            var resultFactor = inputFactor > threshold ? inputFactor * amp : inputFactor * (threshold - amp);
            return NumberValue.Create(resultFactor);
        }

        public static FluidValue ToRow(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            // var strBytes = System.Text.Encoding.UTF8.GetBytes(input.ToStringValue()).Concat(new byte[1] {0x0}).ToArray();
            var strBytes = System.Text.Encoding.UTF8.GetBytes(input.ToStringValue());
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            return new StringValue(BitConverter.ToString(lengthByte.Concat(strBytes).ToArray()));
        }

        public static TemplateContext AddFilters(this TemplateContext templCtx)
        {
            templCtx.Filters.AddFilter("float", PatchFilters.FromFloat);
            templCtx.Filters.AddFilter("string", PatchFilters.FromString);
            templCtx.Filters.AddFilter("int", PatchFilters.FromInt);
            templCtx.Filters.AddFilter("mult", PatchFilters.MultiplyValue);
            templCtx.Filters.AddFilter("amp", PatchFilters.AmplifyInput);
            templCtx.Filters.AddFilter("row", PatchFilters.ToRow);
            templCtx.Filters.AddFilter("bool", PatchFilters.FromBool);
            templCtx.Filters.AddFilter("int16", PatchFilters.FromShort);
            templCtx.Filters.AddFilter("not", PatchFilters.InvertBoolean);
            return templCtx;
        }
    }
}