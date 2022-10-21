using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ModEngine.Core;
using SicarioPatch.Core;

namespace SicarioPatch.Components
{
    
    
    public interface IRequestProvider
    {
        Task<FileInfo> BuildMod<TMod>(IEnumerable<TMod> mods, Dictionary<string, string> parameters, ModBuildOptions opts) where TMod : Mod;

        Task<FileInfo> BuildPreset<TMod>(IEnumerable<TMod> mods, Dictionary<string, string> parameters, ModBuildOptions opts) where TMod : Mod;
    }
}