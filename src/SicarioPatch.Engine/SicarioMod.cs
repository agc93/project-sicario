using System.Collections.Generic;
using System.Text.Json.Serialization;
using ModEngine.Core;

namespace SicarioPatch.Engine;

public class SicarioMod : Mod
{
    [JsonPropertyName("_id")] 
    public string Id { get; set; } = string.Empty;
        
    [JsonPropertyName("_sicario")] 
    public SicarioMetadata ModInfo { get; set; } = new();

    [JsonPropertyName("_vars")]
    public Dictionary<string, string> Variables { get; set; } = new();

    [JsonPropertyName("assetPatches")]
    public Dictionary<string, List<PatchSet<Patch>>> AssetPatches { get; set; } =
        new();
}

public class SicarioMetadata
{
    [JsonPropertyName("private")]
    public bool Private { get; set; }
        
    [JsonPropertyName("overwrites")]
    public bool CanOverwrite { get; set; }
    [JsonPropertyName("group")]
    public string Group { get; set; }
        
    [JsonPropertyName("preview")]
    public bool Unstable { get; set; }
    

    [JsonPropertyName("enableSteps")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string> StepsEnabled { get; set; } = new();
}

