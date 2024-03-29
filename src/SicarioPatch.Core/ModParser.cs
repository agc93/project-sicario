﻿using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using ModEngine.Core;

namespace SicarioPatch.Core
{
    public class ModParser
    {
        private readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        public string ToJson(Mod mod)
        {
            return JsonSerializer.Serialize(mod, _jsonOpts);
        }

        public WingmanMod ParseMod(string rawJson)
        {
            return JsonSerializer.Deserialize<WingmanMod>(rawJson, _jsonOpts);
        }

        public bool IsValid(WingmanMod mod) {
            return mod is {FilePatches: { }} jsonMod && (jsonMod.FilePatches.Any() ||
                                                         (jsonMod.AssetPatches != null && jsonMod.AssetPatches.Any()));
        }

        public JsonSerializerOptions Options => _jsonOpts;

        public JsonSerializerOptions RelaxedOptions => new(_jsonOpts) {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}