using System.Collections.Generic;
using System.IO;
using System.Linq;
using SicarioPatch.Assets;
using SicarioPatch.Core;
using UnPak.Core;
using FilePatchSet = HexPatch.FilePatchSet;

namespace SicarioPatch.Integration
{
    public class SkinSlotLoader
    {
        private readonly IEnumerable<IGameSource> _gameSources;
        private readonly PakFileProvider _pakFileProvider;

        public SkinSlotLoader(IEnumerable<IGameSource> gameSources, PakFileProvider pakFileProvider) {
            _gameSources = gameSources;
            _pakFileProvider = pakFileProvider;
        }

        public Dictionary<string, List<string>> GetSkinPaths() {
            var pakPath = _gameSources.GetGamePakPath();
            var additionalSkins = new List<string>();
            if (pakPath != null) {
                var pakFiles = new FileInfo(pakPath).Directory!.EnumerateFiles("*_P.pak", SearchOption.AllDirectories);
                foreach (var file in pakFiles) {
                    try {
                        var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        var reader = _pakFileProvider.GetReader(fs);
                        var pakFile = reader.ReadFile();
                        var addSkins = pakFile.Records.Select(a => a.GetVirtualPath(pakFile)).Where(r =>
                            r.StartsWith("ProjectWingman/Content/Assets/Skins"));
                        additionalSkins.AddRange(addSkins);
                    }
                    catch {
                        //ignored
                    }
                }
            }

            return additionalSkins.Any()
                ? additionalSkins.GroupBy(a => new FileInfo(a).Directory?.Name ?? string.Empty)
                    .ToDictionary(g => g.Key, g => g.ToList())
                : new Dictionary<string, List<string>>();
        }

        public IEnumerable<AssetPatchSet> GetSlotPatches(Dictionary<string, List<string>>? skinPaths = null) {
            var skins = skinPaths ?? GetSkinPaths();
            foreach (var (aircraft, paths) in skins) {
                var assetPaths = paths.Where(p => Path.GetExtension(p) == ".uasset").ToList();
                yield return new AssetPatchSet() {
                    Name = $"Add {assetPaths.Count} {aircraft}",
                    Patches = assetPaths.Select(p => new AssetPatch {
                        Type = "objectRef",
                        Template = $"datatable:['{aircraft}'].{{'SkinLibraryLegacy*'}}",
                        Value = $"'{Path.GetFileNameWithoutExtension(p)}':'/Game/{Path.ChangeExtension(p.TrimPathTo("Assets"), null)}'"
                    }).ToList()
                };
            }
        }

        public WingmanMod? GetSlotMod(IEnumerable<AssetPatchSet>? patchSets = null) {
            var assetPatches = (patchSets ?? GetSlotPatches()).ToList();
            return assetPatches.Any()
                ? new() {
                    Id = "skinSlots",
                    FilePatches = new Dictionary<string, List<FilePatchSet>>(),
                    AssetPatches = new Dictionary<string, List<AssetPatchSet>> {
                        ["ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp"] =
                            (patchSets ?? GetSlotPatches()).ToList()
                    }
                }
                : null;
        }
    }
}