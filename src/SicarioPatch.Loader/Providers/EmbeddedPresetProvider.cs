using System.Collections.Generic;
using System.Linq;
using SicarioPatch.Integration;

namespace SicarioPatch.Loader.Providers
{
    public class EmbeddedPresetProvider : IMergeProvider
    {
        private readonly MergeLoader _mergeLoader;

        public EmbeddedPresetProvider(MergeLoader mergeLoader) {
            _mergeLoader = mergeLoader;
        }

        public string Name => "embeddedPresets";
        public IEnumerable<MergeComponent> GetMergeComponents(List<string>? searchPaths) {
            var components = new List<MergeComponent>();
            var embeddedPresets = _mergeLoader.LoadPresetsFromMods().ToList();
            var embeddedInputs = embeddedPresets
                .Select(m => m.ModParameters)
                .Aggregate(new Dictionary<string, string>(),
                    (total, next) => total.MergeLeft(next)
                );
            components.Add(new MergeComponent {
                Name = "embeddedPresets",
                Message =
                    $"[dodgerblue2]Loaded [bold]{embeddedPresets.Count}[/] embedded presets from installed mods[/]",
                Mods = embeddedPresets.SelectMany(ep => ep.Mods),
                MergedResources = new Dictionary<string, string> {
                    ["_all"] = string.Join(";",
                        embeddedPresets.SelectMany(ep => ep.Mods).Select(m => m.GetLabel()).ToList())
                },
                Parameters = embeddedInputs,
                Priority = 1
            });
            var existingMods = _mergeLoader.GetSicarioMods();
            var mergedInputs = existingMods
                .Select(m => m.Value.TemplateInputs)
                .Aggregate(new Dictionary<string, string>(), (total, next) => next.MergeLeft(total));
            components.Add(new MergeComponent {
                Name = "sicarioRequests",
                Mods = existingMods.SelectMany(m => m.Value.Mods),
                Parameters = mergedInputs,
                Priority = 3,
                Message = $"[dodgerblue2]Loaded [bold]{existingMods.Count}[/] Sicario mods for rebuild[/]",
                MergedResources = existingMods.ToDictionary(em => em.Key, 
                    v => string.Join(";", v.Value.Mods.Select(m => m.GetLabel())))
            });
            return components;
        }
    }
}