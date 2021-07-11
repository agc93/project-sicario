using System.Text.Json.Serialization;
using SicarioPatch.Core;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace SicarioPatch.Integration
{
    public class EmbeddedRequest
    {
        [JsonPropertyName("request")]
        public PatchRequest? Request { get; set; }
    }
}