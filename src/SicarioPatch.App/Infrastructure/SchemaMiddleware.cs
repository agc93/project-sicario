using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NJsonSchema;
using NJsonSchema.Generation;

namespace SicarioPatch.App.Infrastructure
{
    public class SchemaMiddleware {
        readonly RequestDelegate _next;
        public SchemaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            var settings = new JsonSchemaGeneratorSettings();
            //settings.SerializerOptions = _jsonOpts;
            settings.SerializerSettings = SystemTextJsonUtilities.ConvertJsonOptionsToNewtonsoftSettings(_jsonOpts);            
            var schema = JsonSchema.FromType<Core.WingmanMod>(settings);
            
            var schemeJson = schema.ToJson();
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(schemeJson);
        }

        private readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };
    }
}