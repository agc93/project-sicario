using System.Collections.Generic;

namespace SicarioPatch.Core
{
    public interface IPresetLoader
    {
        IEnumerable<WingmanPreset> LoadPresets();
    }
}