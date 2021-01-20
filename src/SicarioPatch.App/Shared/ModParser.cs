using System.Text.Json;
using HexPatch;

namespace SicarioPatch.App.Shared
{
    public class ModParser
    {
        private JsonSerializerOptions _jsonOpts = new JsonSerializerOptions {
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

        public Mod ParseMod(string rawJson)
        {
            return JsonSerializer.Deserialize<Mod>(rawJson, _jsonOpts);
        }
    }
}