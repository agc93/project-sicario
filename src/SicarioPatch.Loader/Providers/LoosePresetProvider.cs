using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using SicarioPatch.Assets;
using SicarioPatch.Core;
using SicarioPatch.Integration;

namespace SicarioPatch.Loader.Providers
{
    public class LoosePresetProvider : IMergeProvider
    {
        private readonly PresetFileLoader _presetLoader;
        private readonly IEngineInfoProvider _engineInfo;
        private readonly ILogger<LoosePresetProvider> _logger;

        public LoosePresetProvider(PresetFileLoader presetLoader, IEngineInfoProvider engineInfo, ILogger<LoosePresetProvider> logger) {
            _presetLoader = presetLoader;
            _engineInfo = engineInfo;
            _logger = logger;
        }
        public string Name => "loosePresets";
        public IEnumerable<MergeComponent> GetMergeComponents(List<string>? searchPaths) {
            var presetPaths = (searchPaths ?? new List<string>())
                .Where(Directory.Exists)
                .SelectMany(d => Directory.EnumerateFiles(d, "*.dtp", SearchOption.AllDirectories))
                .OrderBy(f => f)
                .ToList();
            var presets = _presetLoader.LoadFromFiles(presetPaths) ?? new Dictionary<string, WingmanPreset>();
            presets = presets.Where(p =>
            {
                var supported = p.Value.IsSupportedBy(_engineInfo);
                if (!supported) {
                    _logger.LogWarning("[bold]Incompatible embed![/] This preset is not supported by the current engine version and will not be loaded!");
                }
                return supported;
            }).ToDictionary();

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