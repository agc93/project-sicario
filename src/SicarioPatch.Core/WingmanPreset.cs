using System;
using System.Collections.Generic;

namespace SicarioPatch.Core
{
    public class WingmanPreset
    {
        public int Version { get; set; } = 1;
        public string? EngineVersion { get; set; }
        public Dictionary<string, string> ModParameters { get; set; }
        public List<WingmanMod> Mods { get; set; }
    }
}