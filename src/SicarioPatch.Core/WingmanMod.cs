using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HexPatch;

namespace SicarioPatch.Core
{
    public class WingmanMod : Mod
    {
        [JsonPropertyName("_group")]
        [Obsolete("Group is now part of Sicario metadata", true)]
        public string Group { get; set; }
        
        [JsonPropertyName("_private")]
        [Obsolete("Private is now part of Sicario metadata", true)]
        public bool Private { get; set; }

        [JsonPropertyName("_sicario")] 
        public SicarioMetadata ModInfo { get; set; } = new SicarioMetadata();

        [JsonPropertyName("_inputs")]
        public List<PatchParameter> Parameters { get; set; } = new List<PatchParameter>();
    }

    public class SicarioMetadata
    {
        [JsonPropertyName("private")]
        public bool Private { get; set; }
        
        [JsonPropertyName("group")]
        public string Group { get; set; }
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
        Number
    }
}