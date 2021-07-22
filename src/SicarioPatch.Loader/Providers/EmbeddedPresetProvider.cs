using System.Collections.Generic;
using System.Linq;
using SicarioPatch.Core;
using SicarioPatch.Integration;

namespace SicarioPatch.Loader.Providers
{
    public class EmbeddedResourceProvider : IMergeProvider
    {
        private readonly MergeLoader _mergeLoader;

        public EmbeddedResourceProvider(MergeLoader mergeLoader) {
            _mergeLoader = mergeLoader;
        }
        public string Name => "embeddedResources";
        public IEnumerable<MergeComponent> GetMergeComponents(List<string>? searchPaths) {
            var components = new List<MergeComponent>();
            var embeddedPresets = new Dictionary<string, WingmanPreset>();
            var requests = new Dictionary<string, PatchRequest>();
            var embeddedResources = _mergeLoader.GetEmbeddedAssets();
            foreach (var (filePath, embeddedResource) in embeddedResources) {
                if (embeddedResource.Preset != null) {
                    embeddedPresets.Add(filePath, embeddedResource.Preset);
                } else if (embeddedResource.Request != null) {
                    requests.Add(filePath, embeddedResource.Request);
                }
            }
            
            var embeddedInputs = embeddedPresets
                .Select(m => m.Value.ModParameters)
                .Aggregate(new Dictionary<string, string>(),
                    (total, next) => total.MergeLeft(next)
                );
            components.Add(new MergeComponent {
                Name = "embeddedPresets",
                Message =
                    $"[dodgerblue2]Loaded [bold]{embeddedPresets.Count}[/] embedded presets from installed mods[/]",
                Mods = embeddedPresets.SelectMany(ep => ep.Value.Mods),
                MergedResources = embeddedPresets.ToDictionary(em => em.Key, 
                    v => string.Join(";", v.Value.Mods.Select(m => m.GetLabel()))),
                Parameters = embeddedInputs,
                Priority = 1
            });
            var mergedInputs = requests
                .Select(m => m.Value.TemplateInputs)
                .Aggregate(new Dictionary<string, string>(), (total, next) => next.MergeLeft(total));
            components.Add(new MergeComponent {
                Name = "sicarioRequests",
                Mods = requests.SelectMany(m => m.Value.Mods),
                Parameters = mergedInputs,
                Priority = 3,
                Message = $"[dodgerblue2]Loaded [bold]{requests.Count}[/] Sicario mods for rebuild[/]",
                MergedResources = requests.ToDictionary(em => em.Key, 
                    v => string.Join(";", v.Value.Mods.Select(m => m.GetLabel())))
            });
            return components;
        }
    }
}