using System.Collections.Generic;

namespace SicarioPatch.Loader
{
    public class MergeReport
    {
        public MergeReport(string installPath) {
            InstallPath = installPath;
        }
        public string InstallPath { get; }

        public Dictionary<string, string> InputParameters { get; set; } = new();
        public Dictionary<string, List<string>> AdditionalSkins { get; set; } = new();
        public List<string> RebuildMods { get; set; } = new();
        public List<string> PresetPaths { get; set; } = new();
    }
}