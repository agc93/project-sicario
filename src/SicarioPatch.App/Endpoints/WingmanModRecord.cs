using System.Collections.Generic;
using System.Text.Json.Serialization;
using HexPatch;
using SicarioPatch.Core;

namespace SicarioPatch.App.Endpoints
{
    public class WingmanModRecord
    {
        public WingmanModRecord(WingmanMod mod) {
            Id = mod.Id;
            ModInfo = mod.ModInfo;
            ModInfo.StepsEnabled = null;
            Metadata = mod.Metadata;
            Parameters = mod.Parameters;
        }
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        
        [JsonPropertyName("_meta")]
        public SetMetadata Metadata { get; set; }
        
        [JsonPropertyName("_sicario")] 
        public SicarioMetadata ModInfo { get; set; }
        
        [JsonPropertyName("_inputs")]
        public List<PatchParameter> Parameters { get; set; }
    }
}