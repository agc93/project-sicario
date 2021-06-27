using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets
{
    internal class PatchRunContext
    {
        internal IAssetTypeLoader Loader { get; init; }
        internal IAssetPatchType? PatchType { get; init; }
        internal IEnumerable<PropertyData> MatchedData { get; set; }

        internal bool IsValid => PatchType != null && Loader != null;

        internal PatchRunContext RunMatch() {
            MatchedData = MatchedData.ToList();
            return this;
        }

    }
}