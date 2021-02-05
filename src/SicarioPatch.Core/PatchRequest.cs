using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediatR;

namespace SicarioPatch.Core
{
    public class PatchRequest : IRequest<FileInfo>
    {
        public PatchRequest(Dictionary<string, WingmanMod> mods)
        {
            Mods = mods.Values.ToList();
        }

        public PatchRequest(IEnumerable<WingmanMod> mods)
        {
            Mods = mods.ToList();
        }

        public List<WingmanMod> Mods { get; }
        public bool PackResult { get; init; } = false;
        
        public string Name { get; init; }
        public Dictionary<string, string> TemplateInputs { get; set; }
    }
}