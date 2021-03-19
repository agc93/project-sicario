using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SicarioPatch.App.Infrastructure
{
    public static class MiddlewareExtensions
    {
        public static IEndpointConventionBuilder MapSchema(this IEndpointRouteBuilder endpoints, string pattern)
        {
            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware<SchemaMiddleware>()
                .Build();

            return endpoints.Map(pattern, pipeline).WithDisplayName("JSON Schema");
        }
    }
}