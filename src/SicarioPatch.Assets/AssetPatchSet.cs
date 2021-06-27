using System.Collections.Generic;

namespace SicarioPatch.Assets
{
    public class AssetPatchSet {
        public string Name {get;set;}
        public List<AssetPatch> Patches {get;set;}
    }
}