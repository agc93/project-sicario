using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MediatR;

namespace SicarioPatch.Core
{
    public class PatchRequest : IRequest<FileInfo>
    {
        public PatchRequest(Dictionary<string, WingmanMod> mods)
        {
            Mods = RebuildModList(mods.Values.ToList());
            Id = Guid.NewGuid().ToString("N");
        }

        public PatchRequest(IEnumerable<WingmanMod> mods)
        {
            Mods = RebuildModList(mods.ToList());
            Id = Guid.NewGuid().ToString("N");
        }
        
        //this is a horrible hack but otherwise the build process mutates the original mod selection
        private static List<WingmanMod> RebuildModList(List<WingmanMod> sourceList)
        {
            var json = JsonSerializer.Serialize(sourceList);
            return JsonSerializer.Deserialize<List<WingmanMod>>(json);
        }
        
        public string Id { get; }

        public List<WingmanMod> Mods { get; }
        public bool PackResult { get; init; } = false;
        
        public string Name { get; init; }
        public Dictionary<string, string> TemplateInputs { get; set; }

        public Dictionary<string, FileInfo> AdditionalFiles { get; set; } = new Dictionary<string, FileInfo>();
    }
}