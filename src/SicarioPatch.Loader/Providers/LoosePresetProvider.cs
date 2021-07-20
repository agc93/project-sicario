using System.Collections.Generic;
using System.IO;
using System.Linq;
using SicarioPatch.Core;

namespace SicarioPatch.Loader.Providers
{
    public class LoosePresetProvider : IMergeProvider
    {
        private readonly PresetFileLoader _presetLoader;

        public LoosePresetProvider(PresetFileLoader presetLoader) {
            _presetLoader = presetLoader;
        }
        public string Name => "loosePresets";
        public IEnumerable<MergeComponent> GetMergeComponents(List<string>? searchPaths) {
            var presetPaths = (searchPaths ?? new List<string>())
                .Where(Directory.Exists)
                .SelectMany(d => Directory.EnumerateFiles(d, "*.dtp", SearchOption.AllDirectories))
                .OrderBy(f => f)
                .ToList();
            var presets = _presetLoader.LoadFromFiles(presetPaths) ?? new Dictionary<string, WingmanPreset>();

            var loosePresetInputs = presets
                .Select(p => p.Value.ModParameters)
                .Aggregate(new Dictionary<string, string>(),
                    (total, next) => total.MergeLeft(next)
                );
            yield return new MergeComponent {
                Name = "loosePresets",
                Priority = 2,
                Parameters = loosePresetInputs,
                Message = $"[dodgerblue2]Loaded [bold]{presets.Count}[/] loose presets from file.[/]",
                Mods = presets.SelectMany(p => p.Value.Mods),
                MergedResources = presets.ToDictionary(k => k.Key,
                    v => string.Join(";", v.Value.Mods.Select(m => m.GetLabel())))
            };
        }
    }
}