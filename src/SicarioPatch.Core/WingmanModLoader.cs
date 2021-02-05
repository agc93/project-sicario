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
            return _modFileLoader.LoadFromFiles(filePaths);
        }

        /*public Dictionary<string, Dictionary<string, WingmanMod>> LoadAllMods(IEnumerable<string> filePaths)
        {
            var allMods = LoadFromFiles(filePaths);
            return allMods.GroupBy(d => d.Value.Group ?? ".").ToDictionary(g => g.Key, g => g.ToDictionary());
        }*/
    }
}