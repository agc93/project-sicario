using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HexPatch;

namespace SicarioPatch.Core
{
    public class WingmanMod : Mod
    {
        [JsonPropertyName("_id")] 
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("_sicario")] 
        public SicarioMetadata ModInfo { get; set; } = new SicarioMetadata();

        [JsonPropertyName("_inputs")]
        public List<PatchParameter> Parameters { get; set; } = new List<PatchParameter>();

        [JsonPropertyName("_vars")]
        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();
    }

    public class SicarioMetadata
    {
        [JsonPropertyName("private")]
        public bool Private { get; set; }
        
        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("enableSteps")]
        public Dictionary<string, string> StepsEnabled { get; set; } = new Dictionary<string, string>();
    }

    public class PatchParameter
    {
        public string Id { get; set; }
        public ParameterType Type { get; set; }
        public string Message { get; set; }
        public string Default { get; set; }
    }

    public enum ParameterType
    {
        String,
        Number,
        Boolean
    }
}