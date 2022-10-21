using System.Reflection;
using Microsoft.Extensions.Configuration;
using SicarioPatch.Engine;

namespace SicarioPatch.App.Infrastructure
{
    public class ConfigurationEngineInfoProvider : IEngineInfoProvider
    {
        private readonly IConfiguration _config;

        public ConfigurationEngineInfoProvider(IConfiguration config) {
            _config = config;
        }
        public string? GetEngineVersion() {
            var requestEmbed = _config.GetValue("EngineVersion", GetFallbackVersion() ?? string.Empty);
            return requestEmbed;
        }

        private string? GetFallbackVersion() {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            if (entryAssembly != null) {
                var infoVersion = entryAssembly.
                    GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                    .InformationalVersion;
                return infoVersion;
            }
            return  null;
        }
    }
}