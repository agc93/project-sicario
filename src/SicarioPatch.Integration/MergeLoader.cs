using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SicarioPatch.Core;
using UnPak.Core;

namespace SicarioPatch.Integration
{
    /// <summary>
    /// Rebuilds pre-existing Sicario-packed mods using their embedded request files
    /// </summary>
    public class MergeLoader
    {
        private readonly PakFileProvider _pakFileProvider;
        private readonly IEnumerable<IGameSource> _gameSources;
        private readonly ModParser _parser;

        public MergeLoader(PakFileProvider pakFileProvider, IEnumerable<IGameSource> gameSources, ModParser parser) {
            _pakFileProvider = pakFileProvider;
            _gameSources = gameSources;
            _parser = parser;
        }

        private IEnumerable<FileInfo> GetAllMods() {
            var pakPath = _gameSources.Select(gs => gs.GetGamePakPath())
                    .FirstOrDefault(gp => !string.IsNullOrWhiteSpace(gp));
            if (pakPath != null) {
                var pakRootPath = new FileInfo(pakPath).GetParentDirectoryPath();
                var allPaks = Directory.EnumerateFiles(pakRootPath, "*.pak", SearchOption.AllDirectories);
                return allPaks.Where(p => Path.GetFileNameWithoutExtension(p) != "ProjectWingman-WindowsNoEditor")
                    .Where(p => new FileInfo(p).Directory?.Name != "~sicario").Select(p => new FileInfo(p)).OrderBy(f => Path.GetRelativePath(pakPath, f.FullName));
            }

            return new List<FileInfo>();
        }

        private static Record? GetPreset(PakFile file) {
            return file?.Records.FirstOrDefault(r => r.GetVirtualPath(file).Contains("sicario") && Path.GetExtension(r.GetVirtualPath(file)) == ".dtp");
        }

        private static Record? GetRequest(PakFile file) {
            return file?.Records.FirstOrDefault(r => r.GetVirtualPath(file).Contains("_meta/sicario") && Path.GetExtension(r.GetVirtualPath(file)) == ".json");
        }

        private PakFile? ReadPakFile(FileInfo pakFileInfo) {
            try {
                var reader = _pakFileProvider.GetReader(pakFileInfo.OpenRead());
                var file = reader.ReadFile();
                return file?.FileStream == null ? null : file;
            }
            catch (Exception e) {
                Console.WriteLine(e);
                //ignored
            }

            return null;
        }

        public Dictionary<string, PatchRequest> GetSicarioMods() {
            var allPaks = GetAllMods().ToList();
            var builtMods = new Dictionary<string, PatchRequest>();
            foreach (var pakFileInfo in allPaks) {
                try {

                    var file = ReadPakFile(pakFileInfo);
                    if (file == null) continue;
                    var presetFile = GetPreset(file);
                    if (presetFile != null) continue; //skip if the file also has a preset
                    var requestFile = GetRequest(file);
                    if (file.FileStream == null || requestFile == null) continue;
                    var outSt = requestFile.Unpack(file.FileStream);
                    var request = new StreamReader(outSt).ReadToEnd();
                    var embed = JsonSerializer.Deserialize<EmbeddedRequest>(request, _parser.RelaxedOptions);
                    if (embed?.Request?.Mods != null && embed.Request.Mods.Any()) {
                        builtMods.Add(pakFileInfo.FullName, embed.Request);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    //ignored
                }
            }
            return builtMods;
        }
        
        
        public Dictionary<string, WingmanPreset> LoadPresetsFromMods() {
            var allPaks = GetAllMods();
            var builtMods = new Dictionary<string, WingmanPreset>();
            foreach (var pakFileInfo in allPaks) {
                try {
                    var file = ReadPakFile(pakFileInfo);
                    if (file == null) continue;
                    var requestFile = GetPreset(file);
                    if (requestFile == null || file.FileStream == null) continue;
                    var outSt = requestFile.Unpack(file.FileStream);
                    var request = new StreamReader(outSt).ReadToEnd();
                    var embed = JsonSerializer.Deserialize<WingmanPreset>(request, _parser.RelaxedOptions);
                    if (embed?.Mods != null && embed.Mods.Any()) {
                        builtMods.Add(pakFileInfo.FullName, embed);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    //ignored
                }
            }
            return builtMods;
        }

        public Dictionary<string, EmbeddedResources> GetEmbeddedAssets() {
            var allPaks = GetAllMods().ToList();
            var builtMods = new Dictionary<string, EmbeddedResources>();
                foreach (var pakFileInfo in allPaks) {
                    try {

                        var file = ReadPakFile(pakFileInfo);
                        if (file == null) continue;
                        var presetFile = GetPreset(file);
                        if (presetFile != null && file.FileStream != null) {
                            var embedPreset = ReadEmbeddedResource<WingmanPreset>(presetFile, file);
                            if (embedPreset?.Mods != null && embedPreset.Mods.Any()) {
                                builtMods.Add(pakFileInfo.FullName, new EmbeddedResources {Preset = embedPreset});
                            }
                            continue;
                        }
                        var requestFile = GetRequest(file);
                        if (requestFile != null && file.FileStream != null) {
                            var embed = ReadEmbeddedResource<EmbeddedRequest>(requestFile, file);
                            if (embed?.Request?.Mods != null && embed.Request.Mods.Any()) {
                                builtMods.Add(pakFileInfo.FullName, new EmbeddedResources {Request = embed.Request});
                            }
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine($"Error encountered loading presets from {pakFileInfo.Name}. You may need to rebuild/re-download this mod: {e.GetType()}");
                        // Console.WriteLine(e);
                        //ignored
                    }
                }
                return builtMods;
        }

        private TResource? ReadEmbeddedResource<TResource>(Record requestFile, PakFile file) {
            var outSt = requestFile.Unpack(file.FileStream);
            var request = new StreamReader(outSt).ReadToEnd();
            // Console.WriteLine($"Deserializing '{request.Length}' of {typeof(TResource).Name} from '{requestFile.FileName}'");
            var embed = JsonSerializer.Deserialize<TResource>(request, _parser.RelaxedOptions);
            return embed;
        }
    }

    public record EmbeddedResources
    {
        public WingmanPreset? Preset { get; init; }
        public PatchRequest? Request { get; init; }
    }
}