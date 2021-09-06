using System;
using System.Collections.Generic;
using System.Linq;
using SicarioPatch.Assets.Patches;
using UAssetAPI.PropertyTypes;
using Xunit;

namespace SicarioPatch.Assets.Tests
{
    public class ModifyPropertyValuePatchTests
    {
        private readonly ModifyPropertyValuePatchType _modifyPatchType;

        public ModifyPropertyValuePatchTests() {
            _modifyPatchType = new ModifyPropertyValuePatchType();
        }
        [Fact]
        public void BasicParser() {
            var parsed = _modifyPatchType.ValueParser.Parse("IntProperty:+2");
            
            Assert.NotNull(parsed);
            Assert.Equal("IntProperty",parsed.ValueType );
            Assert.Null(parsed.ValueRange);

            var result = parsed.RunValueChange(2);
            Assert.Equal(4, result);
        }

        [Fact]
        public void UntypedValue() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var parsed = modifyPatchType.ValueParser.Parse("*:-2");
            
            Assert.NotNull(parsed);
            Assert.Null(parsed.ValueType);
            Assert.Null(parsed.ValueRange);

            var result = parsed.RunValueChange(6);
            Assert.Equal(4, result);
        }

        [Fact]
        public void TypedValueWithRange() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var parsed = modifyPatchType.ValueParser.Parse("FloatProperty:+2.5(0-10)");
            
            Assert.NotNull(parsed);
            Assert.Equal("FloatProperty", parsed.ValueType);
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
            Assert.Equal("FloatProperty", parsed.ValueType);
            Assert.NotNull(parsed.ValueRange);
            Assert.Equal(new Range(0,10), parsed.ValueRange);

            var fProp = new FloatPropertyData() { Value = (float)16 };
            var result = modifyPatchType.RunPatch(new[] { fProp }, template);
            Assert.Equal(10, fProp.Value);
        }
        
        [Fact]
        public void UnTypedValueInsideRange() {
            var modifyPatchType = new ModifyPropertyValuePatchType();
            var template = "IntProperty:+2(10-20)";
            var parsed = modifyPatchType.ValueParser.Parse(template);
            
            Assert.NotNull(parsed);
            Assert.Equal("IntProperty", parsed.ValueType);
            Assert.NotNull(parsed.ValueRange);
            Assert.Equal(new Range(10,20), parsed.ValueRange);

            var iProp = new IntPropertyData { Value = 10 };
            var result = modifyPatchType.RunPatch(new[] { iProp }, template);
            Assert.Equal(12, iProp.Value);
        }
    }
}