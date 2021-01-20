using System.Collections.Generic;
using System.Linq;
using HexPatch;

namespace SicarioPatch.App.Shared
{
    public static class DisplayExtensions
    {
        public static int ToFileCount(this IEnumerable<Mod> mods)
        {
            return mods.SelectMany(m =>
                m.FilePatches.Keys.ToList()).Distinct().Count();
        }
    }
}