using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ModEngine.Templating;
using HexPatch;
using MediatR;
using ModEngine.Core;
using SicarioPatch.Core;

namespace SicarioPatch.Templating
{
    public class PatchTemplateBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
    {
        private readonly TemplateService _template;


        public PatchTemplateBehaviour() {
            _template = new TemplateService();
        }

        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<FileInfo> next)
        {
            request.TemplateInputs ??= new Dictionary<string, string>();
            //var templateObj = DictionaryToObject(request.TemplateInputs.ToDictionary(k => k.Key, v => (object)v.Value));
            
            foreach (var mod in request.Mods)
            {
                var modelVars = _template.RenderVariables(request.TemplateInputs, mod.Variables);
                var dict = RenderFilePatchTemplates(request.TemplateInputs, mod, modelVars);
                
                mod.FilePatches = dict.Where(kvp => kvp.Value.Any()).ToDictionary(k => k.Key, v =>
                {
                    return v.Value.Select(ps => new FilePatchSet { Name = ps.Name, Patches = ps.Patches }).ToList();
                });
                var assetDict = RenderAssetPatchTemplates(request, mod, modelVars);
                mod.AssetPatches = assetDict;
            }
            return await next();
        }

        private Dictionary<string, List<PatchSet<Patch>>> RenderFilePatchTemplates(Dictionary<string, string> templateInputs, WingmanMod mod, Dictionary<string, string> modelVars) {
            var dict = mod.FilePatches.ToDictionary(k => k.Key, kvp =>
            {
                var finalPatches = kvp.Value.Where(psList =>
                {
                    
                    // REMEMBER: this is to keep the step, so have to return false to skip it
                    if (mod.ModInfo.StepsEnabled.ContainsKey(psList.Name ?? string.Empty) &&
                        _template.TryRender(mod.ModInfo.StepsEnabled[psList.Name], templateInputs, modelVars, out var rendered)) {
                        var result = !bool.TryParse(rendered, out var skip) || skip;
                        // var result = bool.TryParse(rendered, out var skip) || skip;
                        // do NOT invert result: result *is* inverted
                        return result;
                    }

                    return true;
                }).Select(psList => _template.RenderPatch(new PatchSet<Patch> {Name = psList.Name, Patches = psList.Patches}, templateInputs, modelVars)).ToList();
                return finalPatches;
            });
            return dict;
        }
        
        private Dictionary<string, List<PatchSet<Patch>>> RenderAssetPatchTemplates(PatchRequest request, WingmanMod mod, Dictionary<string, string> modelVars) {
            var dict = mod.AssetPatches.ToDictionary(k => k.Key, kvp =>
            {
                var finalPatches = kvp.Value.Where(psList =>
                {
                    // REMEMBER: this is to keep the step, so have to return false to skip it
                    if (psList.Name != null && mod.ModInfo.StepsEnabled.ContainsKey(psList.Name) &&
                        _template.TryRender(mod.ModInfo.StepsEnabled[psList.Name], request.TemplateInputs, modelVars, out var rendered)) {
                        var result = !bool.TryParse(rendered, out var skip) || skip;
                        // var result = bool.TryParse(rendered, out var skip) || skip;
                        // do NOT invert result: result *is* inverted
                        return result;
                    }

                    return true;
                }).Select(psList => _template.RenderPatch(psList, request.TemplateInputs, modelVars)).ToList();
                return finalPatches;
            });
            return dict;
        }

        
    }
}