using System.Diagnostics;
using System.Reflection;

namespace SicarioPatch.Assets
{
    public interface IEngineInfoProvider
    {
        string? GetEngineVersion();
    }

    public class EngineInfoProvider : IEngineInfoProvider
    {
        public string? GetEngineVersion() {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            if (entryAssembly != null) {
                var infoVersion = entryAssembly.
                    GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion;
                return infoVersion;
            }

            return null;
        }
        
        
    }
}