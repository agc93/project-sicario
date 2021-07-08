using System.Collections.Generic;

namespace SicarioPatch.Core
{
    public class WingmanPreset
    {
        public int Version { get; set; } = 1;
        public Dictionary<string, string> ModParameters { get; set; }
        public List<WingmanMod> Mods { get; set; }
    }
}