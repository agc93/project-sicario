using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SicarioPatch.Core;

namespace SicarioPatch.App.Endpoints
{
    public class PatchListEndpoint : BaseAsyncEndpoint.WithoutRequest.WithResponse<PatchListEndpoint.PatchListResponse>
    {
        public class PatchListResponse
        {
            public IEnumerable<WingmanModRecord> Mods { get; set; }
        }
        
        private readonly IMediator _mediator;
        private readonly JsonSerializerOptions _jsonOpts = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        public PatchListEndpoint(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet("/mods")]
        public override async Task<ActionResult<PatchListResponse>> HandleAsync(CancellationToken cancellationToken = new CancellationToken()) {
            var req = new ModsRequest() {IncludePrivate = false, OnlyOwnMods = false};
            var res = await _mediator.Send(req, cancellationToken);
            var patches = res.Values.Select(v => new WingmanModRecord(v)).ToList();
            return new JsonResult(new PatchListResponse {Mods = patches}, _jsonOpts);
        }
    }
}