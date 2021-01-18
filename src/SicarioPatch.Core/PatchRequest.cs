using System.Collections.Generic;
using System.IO;
using HexPatch;
using MediatR;

namespace SicarioPatch.Core
{
    public class PatchRequest : IRequest<FileInfo>
    {
        public PatchRequest(Dictionary<string, Mod> mods)
        {
            Mods = mods;
        }

        public Dictionary<string, Mod> Mods { get; }
        public bool PackResult { get; set; } = false;
        
        public string Name { get; set; }
    }
}