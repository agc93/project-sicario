using System.Collections.Generic;
using System.Linq;
using HexPatch.Build;

namespace SicarioPatch.Core
{
    public class WingmanModLoader : IModFileLoader<WingmanMod>
    {
        public WingmanModLoader(ModFileLoader<WingmanMod> loader)
        {
            _modFileLoader = loader;
        }
        private readonly IModFileLoader<WingmanMod> _modFileLoader;
        public Dictionary<string, WingmanMod> LoadFromFiles(IEnumerable<string> filePaths)
        {
            var loadedMods = _modFileLoader.LoadFromFiles(filePaths);
            foreach (var (fileName, mod) in loadedMods)
            {
                mod.Id = System.IO.Path.GetFileName(fileName);
                if (mod.GetLabel(string.Empty).Contains("[TEST]"))
                {
                    mod.ModInfo.Unstable = true;
                }
            }

            return loadedMods;
        }

        /*public Dictionary<string, Dictionary<string, WingmanMod>> LoadAllMods(IEnumerable<string> filePaths)
        {
            var allMods = LoadFromFiles(filePaths);
            return allMods.GroupBy(d => d.Value.Group ?? ".").ToDictionary(g => g.Key, g => g.ToDictionary());
        }*/
    }
}