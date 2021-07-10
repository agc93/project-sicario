using System.Collections.Generic;
using System.IO;
using System.Linq;
using HexPatch;
using SicarioPatch.Assets;
using SicarioPatch.Core;
using UnPak.Core;

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
            if (pakPath != null) {
                var pakFiles = new FileInfo(pakPath).Directory!.EnumerateFiles("*_P.pak", SearchOption.AllDirectories);
                foreach (var file in pakFiles) {
                    var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var reader = _pakFileProvider.GetReader(fs);
                    var pakFile = reader.ReadFile();
                    var addSkins = pakFile.Records.Select(a => a.GetVirtualPath(pakFile)).Where(r =>
                        r != null && r.StartsWith("ProjectWingman/Content/Assets/Skins")).Cast<string>();
                    return addSkins.GroupBy(a => new FileInfo(a).Directory.Name).ToDictionary(g => g.Key, g => g.ToList());
                }
            }

            return new Dictionary<string, List<string>>();
        }

        public IEnumerable<AssetPatchSet> GetSlotPatches() {
            var skins = GetSkinPaths();
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

        public WingmanMod GetSlotMod() {
            return new() {
                Id = "skinSlots",
                FilePatches = new Dictionary<string, List<PatchSet>>(),
                AssetPatches = new Dictionary<string, List<AssetPatchSet>> {
                    ["ProjectWingman/Content/ProjectWingman/Blueprints/Data/AircraftData/DB_Aircraft.uexp"] =
                        GetSlotPatches().ToList()
                }
            };
        }
    }
}