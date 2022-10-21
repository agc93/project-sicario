using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HexPatch;
using ModEngine.Core;

namespace SicarioPatch.Core
{
    public class SicarioMod : Mod
    {
        [JsonPropertyName("_id")] 
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("_sicario")] 
        public SicarioMetadata ModInfo { get; set; } = new SicarioMetadata();

        [JsonPropertyName("_inputs")]
        public List<PatchParameter> Parameters { get; set; } = new List<PatchParameter>();

        [JsonPropertyName("_vars")]
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("assetPatches")]
        public Dictionary<string, List<PatchSet<Patch>>> AssetPatches { get; set; } =
            new();
    }
    public class WingmanMod : SicarioMod
    {
        public Dictionary<string, List<FilePatchSet>> FilePatches { get; set; } = new();
    }

    public class SicarioMetadata
    {
        [JsonPropertyName("private")]
        public bool Private { get; set; }
        
        [JsonPropertyName("group")]
        public string Group { get; set; }
        
        [JsonPropertyName("preview")]
        public bool Unstable { get; set; }
        [JsonPropertyName("overwrites")]
        public bool CanOverwrite { get; set; }

        [JsonPropertyName("enableSteps")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string> StepsEnabled { get; set; } = new Dictionary<string, string>();
    }

    public class PatchParameter
    {
        public string Id { get; set; }
        public ParameterType Type { get; set; }
        public string Message { get; set; }
        public string Default { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Range { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Pattern { get; set; }
    }

    public enum ParameterType
    {
        String,
        Number,
        Boolean
    }
}