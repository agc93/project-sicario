using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SicarioPatch.Integration
{
    public class ConfigurationGameSource : IGameSource
    {
        private readonly IConfiguration _config;

        public ConfigurationGameSource(IConfiguration config) {
            _config = config;
        }
        public string? GetGamePath() {
            return _config.GetChildren().FirstOrDefault(c => c.Key == "GamePath")?.Value;
        }

        public string? GetGamePakPath() {
            return _config.GetChildren().FirstOrDefault(c => c.Key == "GamePakPath")?.Value;
        }
    }
}