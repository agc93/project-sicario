using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Fluid.Values;
using HexPatch;
using SicarioPatch.Core;

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

        public static FluidValue FromUInt8(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            return new StringValue(BitConverter.ToString(new[] {byte.Parse(input.ToStringValue())}));
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

        public static FluidValue FromWord(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var strBytes = System.Text.Encoding.UTF8.GetBytes(input.ToStringValue());
            var lengthByte = BitConverter.GetBytes(strBytes.Length + 1);
            return new StringValue(BitConverter.ToString(lengthByte.Concat(strBytes).ToArray()));
        }

        public static FluidValue ToRandom(FluidValue input, FilterArguments arguments, TemplateContext ctx)
        {
            var minValue = input.ToNumberValue();
            var maxValue = arguments.At(0).ToNumberValue();
            var rand = new Random(DateTime.UtcNow.Millisecond);
            var range = new[] {minValue, maxValue};
            var finalValue = NumberValue.Zero;
            if (range.All(r => Math.Abs(r) == r && int.TryParse(r.ToString(), out var _)))
            {
                var ints = range.Select(Convert.ToInt32).ToList();
                var result = rand.Next(ints[0], ints[1]);
                finalValue = NumberValue.Create(result);
            } else if (range.All(r => float.TryParse(r.ToString(), out var _)))
            {
                var floats = range.Select(Convert.ToSingle).ToList();
                var result = rand.NextFloat(floats[0], floats[1]);
                finalValue = NumberValue.Create(Convert.ToDecimal(result));
            }

            return finalValue;
        }

        public static FluidParser AddTags(this FluidParser parser)
        {
            parser.RegisterIdentifierTag("rand", (identifier, writer, encoder, context) =>
            {
                var range = identifier
                    .Split(':', '-',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(float.Parse)
                    .ToList();
                var rand = new Random(Convert.ToInt32(DateTime.UtcNow.Ticks));
                if (range.All(r => Math.Abs(r) == r && int.TryParse(r.ToString(), out var _)))
                {
                    //int range
                    var ints = range.Select(Convert.ToInt32).ToList();
                    var output = BitConverter.ToString(BitConverter.GetBytes(rand.Next(ints[0], ints[1])));
                    writer.Write(output);
                    return ValueTask.FromResult(Completion.Normal);
                }
                else
                {
                    var output =
                        BitConverter.ToString(
                            BitConverter.GetBytes(Convert.ToSingle(rand.NextFloat(range[1], range[2]))));
                    writer.Write(output);
                    return ValueTask.FromResult(Completion.Normal);
                }
            });
            return parser;
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
            templCtx.Filters.AddFilter("byte", PatchFilters.FromUInt8);
            templCtx.Filters.AddFilter("random", PatchFilters.ToRandom);
            templCtx.Filters.AddFilter("word", PatchFilters.FromWord);
            return templCtx;
        }
    }
}