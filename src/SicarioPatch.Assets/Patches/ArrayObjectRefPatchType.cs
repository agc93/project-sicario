using System;
using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Patches
{
    [Obsolete("This is replaced by the array support in ObjectRefPatchType")]
    public class ArrayObjectRefPatchType : AssetPatchType<ObjectRefPatchType.ObjectReference>
    {
        public override string Type { get; }
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, ObjectRefPatchType.ObjectReference parsedValue) {
            throw new NotImplementedException();
        }

        protected override Parser<ObjectRefPatchType.ObjectReference> ValueParser { get; }
    }
}