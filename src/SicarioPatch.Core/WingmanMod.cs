using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HexPatch;
using ModEngine.Core;
using SicarioPatch.Engine;

namespace SicarioPatch.Core
{
    public class WingmanMod : SicarioMod
    {
        public Dictionary<string, List<FilePatchSet>> FilePatches { get; set; } = new();
        
        [JsonPropertyName("_inputs")]
        public List<PatchParameter> Parameters { get; set; } = new();
    }

    
}