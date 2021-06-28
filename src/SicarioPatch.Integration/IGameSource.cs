using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SicarioPatch.Integration
{
    public interface IGameSource
    {
        string? GetGamePath();
        string? GetGamePakPath();
    }

    public class ConfigurationGameSource : IGameSource
    {
        private readonly IConfigurationSection _config;

        public ConfigurationGameSource(IConfigurationSection config) {
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