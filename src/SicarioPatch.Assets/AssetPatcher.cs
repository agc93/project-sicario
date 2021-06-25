using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SicarioPatch.Assets.Fragments;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets
{
    public class AssetPatchSet {
        public string Name {get;set;}
        public List<AssetPatch> Patches {get;set;}
    }
    public class AssetPatch
    {
        public int Version { get; set; } = 1;
        public string Description {get;set;}
        public string Template {get;set;}
        public string Value {get;set;}
        public string Type {get;set;} = string.Empty;
    }
    public class AssetPatcher
    {
        private readonly IEnumerable<IAssetParserFragment> _fragments;
        private readonly IEnumerable<IAssetTypeLoader> _typeLoaders;
        private readonly IEnumerable<IAssetPatchType> _patchTypes;
        private readonly IEnumerable<ITemplateProvider> _templates;

        public AssetPatcher(IEnumerable<IAssetParserFragment> fragments, IEnumerable<IAssetTypeLoader> typeLoaders, IEnumerable<IAssetPatchType> patchTypes, IEnumerable<ITemplateProvider> templates) {
            _fragments = fragments;
            _typeLoaders = typeLoaders;
            _patchTypes = patchTypes;
            _templates = templates;
        }

        public async Task<FileInfo> RunPatch(string sourcePath, IEnumerable<AssetPatchSet> sets) {
            sourcePath = Path.ChangeExtension(sourcePath, "uasset");
            var fi = new FileInfo(sourcePath);
            foreach (var set in sets) {
                var y = new AssetWriter(fi.FullName, null);
                var newData = new List<PropertyData>();
                foreach (var assetPatch in set.Patches) {
                    var parser = new TemplateParser(_typeLoaders, _templates);
                    var (loader, fragments) = parser.ParseTemplate(assetPatch.Template);
                    var data = loader.LoadData(y.data);
                    var matchedData = fragments.Aggregate(data, (datas, fragment) => fragment.Match(datas));
                    var patchType = _patchTypes.FirstOrDefault(pt => pt.Type == assetPatch.Type);
                    // y.data.AddHeaderReference("EUFB");
                    var newRecords = patchType?.RunPatch(matchedData, assetPatch.Value);
                    if (newRecords != null) {
                        y = loader.AddData(y, newRecords);
                    }
                }
                y.Write(fi.FullName);
            }
            return fi;
        }
    }
}