using System.Collections.Generic;
using System.IO;
using System.Linq;
using SicarioPatch.Core;
using SicarioPatch.Integration;

namespace SicarioPatch.Loader.Providers
{
    public class SkinMergeProvider : IMergeProvider
    {
        private readonly SkinSlotLoader _slotLoader;

        public SkinMergeProvider(SkinSlotLoader slotLoader) {
            _slotLoader = slotLoader;
        }
        public string Name => "customSkins";
        public IEnumerable<MergeComponent> GetMergeComponents(List<string>? searchPaths) {
            var skinPaths = _slotLoader.GetSkinPaths();
            var slotPatches = _slotLoader.GetSlotPatches(skinPaths).ToList();
            var slotLoader = _slotLoader.GetSlotMod(slotPatches);
            if (slotLoader != null) {
                var skinSet = skinPaths
                    .ToDictionary(k => k.Key,
                        v => v.Value.Select(p => Path.ChangeExtension(p, null)).Distinct().ToList())
                    .ToDictionary(k => k.Key, v => string.Join(";", v.Value));
                return new[] {
                    new MergeComponent {
                        Name = "customSkins",
                        Mods = new[] { slotLoader },
                        MergedResources = skinSet,
                        Message =
                            $"[dodgerblue2]Successfully compiled skin merge with [bold]{slotLoader.GetPatchCount()}[/] patches.[/]"
                    }
                };
            }
            return System.Array.Empty<MergeComponent>();
        }
    }
}