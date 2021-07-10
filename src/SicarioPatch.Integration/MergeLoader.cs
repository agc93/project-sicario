using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SicarioPatch.Core;
using UnPak.Core;

namespace SicarioPatch.Integration
{
    public class EmbeddedRequest
    {
        [JsonPropertyName("request")]
        public PatchRequest Request { get; set; }
    }
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
                var pakRootPath = new FileInfo(pakPath).Directory.FullName;
                var allPaks = Directory.EnumerateFiles(pakRootPath, "*.pak", SearchOption.AllDirectories);
                return allPaks.Where(p => Path.GetFileNameWithoutExtension(p) != "ProjectWingman-WindowsNoEditor")
                    .Where(p => new FileInfo(p).Directory?.Name != "~sicario").Select(p => new FileInfo(p));
            }

            return new List<FileInfo>();
        }

        public IEnumerable<PatchRequest> GetSicarioMods() {
            var allPaks = GetAllMods();
            var builtMods = new List<PatchRequest>();
            foreach (var pakFileInfo in allPaks) {
                try {
                    var reader = _pakFileProvider.GetReader(pakFileInfo.OpenRead());
                    var file = reader.ReadFile();
                    var requestFile =
                        file.Records.FirstOrDefault(r => r.GetVirtualPath(file).Contains("_meta/sicario") && Path.GetExtension(r.GetVirtualPath(file)) == ".json");
                    if (requestFile == null) continue;
                    var outSt = requestFile.Unpack(file.FileStream);
                    var request = new StreamReader(outSt).ReadToEnd();
                    var embed = JsonSerializer.Deserialize<EmbeddedRequest>(request, _parser.Options);
                    if (embed?.Request?.Mods != null && embed.Request.Mods.Any()) {
                        builtMods.Add(embed.Request);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    //ignored
                }
            }
            return builtMods;
        }
        
        public IEnumerable<WingmanPreset> LoadPresetsFromMods() {
            var allPaks = GetAllMods();
            var builtMods = new List<WingmanPreset>();
            foreach (var pakFileInfo in allPaks) {
                try {
                    var reader = _pakFileProvider.GetReader(pakFileInfo.OpenRead());
                    var file = reader.ReadFile();
                    var requestFile =
                        file.Records.FirstOrDefault(r => r.GetVirtualPath(file).Contains("sicario") && Path.GetExtension(r.GetVirtualPath(file)) == ".dtp");
                    if (requestFile == null) continue;
                    var outSt = requestFile.Unpack(file.FileStream);
                    var request = new StreamReader(outSt).ReadToEnd();
                    var embed = JsonSerializer.Deserialize<WingmanPreset>(request, _parser.Options);
                    if (embed?.Mods != null && embed.Mods.Any()) {
                        builtMods.Add(embed);
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    //ignored
                }
            }
            return builtMods;
        }
    }
}