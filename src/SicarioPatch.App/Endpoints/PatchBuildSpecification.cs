using System.Collections.Generic;
using SicarioPatch.Core;

namespace SicarioPatch.App.Endpoints
{
    public class PatchBuildSpecification
    {
        public Dictionary<string, string> InputParameters { get; set; }
        public List<WingmanMod> Mods { get; set; }
        public string OutputName { get; set; }
        public List<string> IncludedMods { get; set; }
        
    }
}