using System.Collections.Generic;
using System.Linq;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Engine.Fragments
{
    public class ArrayFlattenFragment : IAssetParserFragment
    {
        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            return initialInput.Where(pd => pd.Type == "ArrayProperty" && pd is ArrayPropertyData)
                .Cast<ArrayPropertyData>().SelectMany(pd => pd.Value);
            /*foreach (var arrayPropertyData in initialInput.Where(i => i.Type == "ArrayProperty" && i is ArrayPropertyData).Cast<ArrayPropertyData>()) {
                return arrayPropertyData.Value;
            }*/
            // return new List<PropertyData>();
        }
    }
}