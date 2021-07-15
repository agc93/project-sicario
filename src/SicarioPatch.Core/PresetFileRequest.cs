using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using MediatR;

namespace SicarioPatch.Core
{
    public class PresetFileRequest : IRequest<FileInfo>
    {
        public PresetFileRequest(IEnumerable<WingmanMod> mods) {
            Mods = mods.ToList().RebuildModList();
        }
        
        [JsonInclude]
        public List<WingmanMod> Mods { get; private set; }
        
        public Dictionary<string, string> TemplateInputs { get; init; }
        public int? Version { get; init; }
        public string PresetName { get; init; }
    }
}