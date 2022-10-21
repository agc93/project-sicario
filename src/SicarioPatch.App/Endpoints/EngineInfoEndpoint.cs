using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SicarioPatch.Engine;

namespace SicarioPatch.App.Endpoints
{
    public class EngineInfoEndpoint : BaseAsyncEndpoint.WithoutRequest.WithResponse<EngineInfoEndpoint.EngineInfoResponse>
    {
        private readonly IEngineInfoProvider _engineInfoProvider;
        private readonly JsonSerializerOptions _jsonOpts = new() {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        public EngineInfoEndpoint(IEngineInfoProvider engineInfoProvider) {
            _engineInfoProvider = engineInfoProvider;
        }
        public class EngineInfoResponse
        {
            public string EngineVersion { get; init; }
        }

        [HttpGet("/engineInfo")]
        public override async Task<ActionResult<EngineInfoResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken()) {
            return new JsonResult(new EngineInfoResponse
                { EngineVersion = _engineInfoProvider.GetEngineVersion() ?? "unknown" }, _jsonOpts);
        }
    }
}