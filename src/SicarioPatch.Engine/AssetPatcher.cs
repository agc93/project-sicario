using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModEngine.Core;
using UAssetAPI;

namespace SicarioPatch.Engine
{
    public class AssetPatcher
    {
        private readonly IEnumerable<IAssetTypeLoader> _typeLoaders;
        private readonly IEnumerable<IAssetPatchType> _patchTypes;
        private readonly IEnumerable<ITemplateProvider> _templates;

        public AssetPatcher(IEnumerable<IAssetTypeLoader> typeLoaders, IEnumerable<IAssetPatchType> patchTypes, IEnumerable<ITemplateProvider> templates) {
            _typeLoaders = typeLoaders;
            _patchTypes = patchTypes;
            _templates = templates;
        }

        public async Task<FileInfo> RunPatch(string sourcePath, IEnumerable<PatchSet<Patch>> sets, string targetName = null) {
            sourcePath = Path.GetExtension(sourcePath) == ".uexp" ? Path.ChangeExtension(sourcePath, "uasset") : sourcePath;
            var fi = new FileInfo(sourcePath);
            foreach (var set in sets) {
                var y = new AssetWriter(fi.FullName, null);
                var matchedPatches = set.Patches.ToDictionary(k => k, assetPatch =>
                {
                    var parser = new TemplateParser(_typeLoaders, _templates);
                    var templateCtx = parser.ParseTemplate(assetPatch.Template);
                    var loader = templateCtx.TypeLoader;
                    var fragments = templateCtx.Fragments;
                    var data = loader.LoadData(y.data, templateCtx.LoaderParameter);
                    var matchedData = fragments.Aggregate(data, (datas, fragment) => fragment.Match(datas));
                    var patchType = _patchTypes.FirstOrDefault(pt => pt.Type == assetPatch.Type);
                    return new PatchRunContext {Loader = loader, MatchedData = matchedData, PatchType = patchType}.RunMatch();
                }).Where(p => p.Value.IsValid);
                foreach (var (patch, ctx) in matchedPatches) {
                    var newRecords = ctx.PatchType?.RunPatch(ctx.MatchedData, patch.Value);
                    if (newRecords != null) {
                        try {
                            y = ctx.Loader.RunInstructions(y, newRecords);
                        }
                        catch (Exception e) {
                            throw new AssetInstructionException(ctx, e);
                        }
                    }
                }
                var targetAssetPath = targetName == null ? fi.FullName : Path.ChangeExtension(Path.Join(Path.GetDirectoryName(fi.FullName), targetName), "uasset");
                y.Write(targetAssetPath);
            }
            return fi;
        }
    }
}