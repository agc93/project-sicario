using System;
using System.Collections.Generic;
using System.Linq;
using SicarioPatch.Assets.Patches;
using UAssetAPI.PropertyTypes;
using Xunit;

namespace SicarioPatch.Assets.Tests
{
    public class ModifyPropertyValueFragmentTests
    {
        private readonly ModifyPropertyValuePatchType _modifyPatchType;

        public ModifyPropertyValueFragmentTests() {
            _modifyPatchType = new ModifyPropertyValuePatchType();
        }
        [Fact]
        public void BasicParser() {
            var parsed = _modifyPatchType.ValueParser.Parse("IntProperty:+2");
            
            Assert.NotNull(parsed);
            Assert.Equal(parsed.ValueType ,"IntProperty");
            Assert.Null(parsed.ValueRange);

            var result = parsed.RunValueChange(2);
            Assert.Equal(result, 4);
        }

        [Fact]
        public void UntypedValue() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var parsed = modifyPatchType.ValueParser.Parse("*:-2");
            
            Assert.NotNull(parsed);
            Assert.Null(parsed.ValueType);
            Assert.Null(parsed.ValueRange);

            var result = parsed.RunValueChange(6);
            Assert.Equal(result, 4);
        }

        [Fact]
        public void TypedValueWithRange() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var parsed = modifyPatchType.ValueParser.Parse("FloatProperty:+2.5(0-10)");
            
            Assert.NotNull(parsed);
            Assert.Equal(parsed.ValueType, "FloatProperty");
            Assert.NotNull(parsed.ValueRange);
            Assert.Equal(parsed.ValueRange, new Range(0, 10));

            var result = parsed.RunValueChange(2);
            Assert.Equal(result, (decimal)4.5);
        }
        
        [Fact]
        public void TypedValueOutsideRange() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var template = "FloatProperty:+12.5(0-10)";
            var parsed = modifyPatchType.ValueParser.Parse(template);
            
            Assert.NotNull(parsed);
            Assert.Equal(parsed.ValueType, "FloatProperty");
            Assert.NotNull(parsed.ValueRange);
            Assert.Equal(parsed.ValueRange, new Range(0, 10));

            var fProp = new FloatPropertyData() { Value = (float)16 };
            var result = modifyPatchType.RunPatch(new[] { fProp }, template);
            Assert.Equal(fProp.Value, (float)10);
        }
        
        [Fact]
        public void UnTypedValueInsideRange() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var template = "IntProperty:+2(10-20)";
            var parsed = modifyPatchType.ValueParser.Parse(template);
            
            Assert.NotNull(parsed);
            Assert.Equal(parsed.ValueType, "IntProperty");
            Assert.NotNull(parsed.ValueRange);
            Assert.Equal(parsed.ValueRange, new Range(10, 20));

            var iProp = new IntPropertyData { Value = 10 };
            var result = modifyPatchType.RunPatch(new[] { iProp }, template);
            Assert.Equal(12, iProp.Value);
        }
    }
}