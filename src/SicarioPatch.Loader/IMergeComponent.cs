using System.Collections.Generic;
using SicarioPatch.Core;

namespace SicarioPatch.Loader
{
    public interface IMergeProvider
    {
        
        public string Name { get; }
        public IEnumerable<MergeComponent> GetMergeComponents(List<string>? searchPaths);
    }
    public record MergeComponent
    {
        public string? Name { get; set; }
        public Dictionary<string, string> Parameters { get; init; } = new();
        public IEnumerable<WingmanMod> Mods { get; init; } = new List<WingmanMod>();
        public string? Message { get; init; }
        public Dictionary<string, string>? MergedResources { get; init; }
        public int Priority { get; init; } = 10;
    }
}