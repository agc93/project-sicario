using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;

namespace SicarioPatch.Core
{
    public class PatchRequest : IRequest<FileInfo>
    {
        public PatchRequest(IEnumerable<WingmanMod> mods)
        {
            Mods = RebuildModList(mods.ToList());
            Id = Guid.NewGuid().ToString("N");
        }

        [Obsolete("Only used for deserialization", true)]
        public PatchRequest() {
            
        }
        
        //this is a horrible hack but otherwise the build process mutates the original mod selection
        private static List<WingmanMod> RebuildModList(List<WingmanMod> sourceList)
        {
            var json = JsonSerializer.Serialize(sourceList);
            return JsonSerializer.Deserialize<List<WingmanMod>>(json);
        }
        
        public string Id { get; }

        [JsonInclude]
        public List<WingmanMod> Mods { get; private set; }
        public bool PackResult { get; init; } = false;
        
        public string Name { get; init; }
        public Dictionary<string, string> TemplateInputs { get; set; }

        public Dictionary<string, FileInfo> AdditionalFiles { get; set; } = new Dictionary<string, FileInfo>();
        
        public string UserName { get; init; }
    }
}