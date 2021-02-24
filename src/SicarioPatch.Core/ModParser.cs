using System.Text.Json;
using HexPatch;

namespace SicarioPatch.Core
{
    public class ModParser
    {
        private readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
        public string ToJson(Mod mod)
        {
            return JsonSerializer.Serialize(mod, _jsonOpts);
        }

        public WingmanMod ParseMod(string rawJson)
        {
            return JsonSerializer.Deserialize<WingmanMod>(rawJson, _jsonOpts);
        }

        public JsonSerializerOptions Options => _jsonOpts;
    }
}