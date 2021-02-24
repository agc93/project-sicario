using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SicarioPatch.Core
{
    public class BuildLogBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
    {
        private readonly IBuildLog _log;

        public BuildLogBehaviour()
        {
            
        }

        public BuildLogBehaviour(IBuildLog log)
        {
            _log = log;
        }
        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<FileInfo> next)
        {
            var requestId = request.Id;
            var result = await next();
            _log?.SaveRequest(new PatchRequestSummary(requestId)
            {
                Inputs = request.TemplateInputs,
                FileName = result.Name,
                IncludedPatches = request.Mods.Select(m => m.GetLabel()).ToList()
            });
            return result;
        }
    }
}