using System.Collections.Generic;
using System.Linq;
using SicarioPatch.Engine.Patches;
using UAssetAPI.PropertyTypes;
using Xunit;

namespace SicarioPatch.Engine.Tests
{
    public class ArrayPropertyValuePatchTests
    {
        [Fact]
        public void IntegerItemsParser() {
            var patch = new ArrayPropertyPatchType();
            var parsed = patch.ValueParser.Parse("IntProperty:[2,3]");
            
            Assert.NotNull(parsed);
            Assert.NotNull(parsed.ItemValueType);
            Assert.NotNull(parsed.Value);
            Assert.Equal(ArrayModification.Operation.Replace, parsed.Type);
            Assert.Equal("IntProperty",parsed.ItemValueType);
            Assert.Equal(2, parsed.Value.Count);
            Assert.Equal(new List<string> {"2","3"}, parsed.Value);
        }

        [Fact]
        public void StringItemsParser() {
            var patch = new ArrayPropertyPatchType();
            var parsed = patch.ValueParser.Parse("StrProperty:[\'something\']");
            
            Assert.Equal("StrProperty", parsed.ItemValueType);
            Assert.Single(parsed.Value);
            Assert.Equal("something", parsed.Value.First());
        }

        [Fact]
        public void IntegerItemsPatch() {
            var patch = new ArrayPropertyPatchType();
            var patchTemplate = "IntProperty:[10,11,12]";

            var input = new ArrayPropertyData() {
                ArrayType = "IntProperty",
                Value = new PropertyData[] { new IntPropertyData() { Value = 1 }, new IntPropertyData() { Value = 2 } }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Equal(3, input.Value.Length);
            Assert.Equal(10, (input.Value.First() as IntPropertyData)?.Value);
        }

        [Fact]
        public void StringItemsRemoveParser() {
            var patch = new ArrayPropertyPatchType();
            var parsed = patch.ValueParser.Parse("StrProperty:-[\'else\']");
            
            Assert.NotNull(parsed);
            Assert.Equal(ArrayModification.Operation.Remove, parsed.Type);
            Assert.Equal("StrProperty", parsed.ItemValueType);
            Assert.Single(parsed.Value);
            Assert.Equal("else", parsed.Value.First());
        }

        [Fact]
        public void StringItemsRemovePatch() {
            var patch = new ArrayPropertyPatchType();
            var patchTemplate = "StrProperty:-['else']";
            
            var input = new ArrayPropertyData() {
                ArrayType = "StrProperty",
                Value = new PropertyData[] { new StrPropertyData() { Value = "something" }, new StrPropertyData() { Value = "else" } }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Single(input.Value);
            Assert.Equal("something", (input.Value.First() as StrPropertyData)?.Value);
        }

        [Fact]
        public void StringItemsAddPatch() {
            var patch = new ArrayPropertyPatchType();
            const string patchTemplate = "StrProperty:+['entirely']";
            
            var input = new ArrayPropertyData() {
                ArrayType = "StrProperty",
                Value = new PropertyData[] { new StrPropertyData() { Value = "something" }, new StrPropertyData() { Value = "else" } }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Equal(3, input.Value.Length);
            Assert.Equal("something", (input.Value.First() as StrPropertyData)?.Value);
            Assert.Equal("entirely", (input.Value.Last() as StrPropertyData)?.Value);
        }
        
        [Fact]
        public void StringItemsReplacePatch() {
            var patch = new ArrayPropertyPatchType();
            const string patchTemplate = "StrProperty:['different','values','again']";
            
            var input = new ArrayPropertyData() {
                ArrayType = "StrProperty",
                Value = new PropertyData[] { new StrPropertyData() { Value = "something" }, new StrPropertyData() { Value = "else" } }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Equal(3, input.Value.Length);
            Assert.Equal("different", (input.Value.First() as StrPropertyData)?.Value);
            Assert.Equal("again", (input.Value.Last() as StrPropertyData)?.Value);
        }
        
        [Fact]
        public void IntegerItemsRemovePatch() {
            var patch = new ArrayPropertyPatchType();
            const string patchTemplate = "IntProperty:-[2]";
            
            var input = new ArrayPropertyData() {
                ArrayType = "IntProperty",
                Value = new PropertyData[] { new IntPropertyData() { Value = 1 }, new IntPropertyData() { Value = 2 }, new IntPropertyData {Value = 3} }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Equal(2, input.Value.Length);
            Assert.Equal(1, (input.Value.First() as IntPropertyData)?.Value);
            Assert.Equal(3, (input.Value.Last() as IntPropertyData)?.Value);
        }

        [Fact]
        public void IntegerItemsAddPatch() {
            var patch = new ArrayPropertyPatchType();
            const string patchTemplate = "IntProperty:+[4]";
            
            var input = new ArrayPropertyData() {
                ArrayType = "IntProperty",
                Value = new PropertyData[] { new IntPropertyData() { Value = 1 }, new IntPropertyData() { Value = 2 }, new IntPropertyData {Value = 3} }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Equal(4, input.Value.Length);
            Assert.Equal(1, (input.Value.First() as IntPropertyData)?.Value);
            Assert.Equal(4, (input.Value.Last() as IntPropertyData)?.Value);
        }
        
        [Fact]
        public void IntegerItemsReplacePatch() {
            var patch = new ArrayPropertyPatchType();
            var patchTemplate = "IntProperty:[10,11,12]";
            
            var input = new ArrayPropertyData() {
                ArrayType = "IntProperty",
                Value = new PropertyData[] { new IntPropertyData() { Value = 1 }, new IntPropertyData() { Value = 2 }, new IntPropertyData {Value = 3} }
            };
            patch.RunPatch(new[] { input }, patchTemplate);
            Assert.Equal(3, input.Value.Length);
            Assert.Equal(10, (input.Value.First() as IntPropertyData)?.Value);
            Assert.Equal(12, (input.Value.Last() as IntPropertyData)?.Value);
        }
    }
}