using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildEngine;
using HexPatch;
using HexPatch.Build;
using MediatR;

namespace SicarioPatch.Core
{
    public class ModsRequest : IRequest<Dictionary<string, Mod>>
    {
        
    }
    
    public class PatchRequest : IRequest<FileInfo>
    {
        public PatchRequest(Dictionary<string, Mod> mods)
        {
            Mods = mods;
        }

        public Dictionary<string, Mod> Mods { get; }
        public bool PackResult { get; set; } = false;
        
        public string Name { get; set; }
    }

    public class PatchRequestHandler : IRequestHandler<PatchRequest, FileInfo>
    {
        private readonly ModPatchServiceBuilder _builder;

        public PatchRequestHandler(ModPatchServiceBuilder servBuilder)
        {
            _builder = servBuilder;
        }

        private Dictionary<string, IEnumerable<string>> _sideCars = new Dictionary<string, IEnumerable<string>> {
            [".uexp"] = new string[] {".uasset"}
        };
        
        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken)
        {
            using var mpServ = await _builder.GetPatchService(request.Mods, request.Name);
            await mpServ.LoadFiles(HexPatch.Build.FileSelectors.SidecarFiles(_sideCars)).RunPatches();
            (bool Success, FileInfo Result)? result;
            if (request.PackResult)
            {
                result = mpServ.RunBuild(ctx =>
                    new FileInfo(
                        Path.Combine(ctx.BuildScript.WorkingDirectory, $"merged-{DateTime.UtcNow.Ticks}_P.pak")));
            }
            else
            {
                result = mpServ.RunAction(ctx => ctx.WorkingDirectory.ToZipFile());
            }
            if (result != null && result.Value.Success) {
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), result.Value.Result.Name));
                result.Value.Result.MoveTo(tempFi.FullName);
                return tempFi.Exists ? tempFi : null;
            }
            return null;

            // return result != null ? (result.Value.Success ? result.Value.Result : null) : null;
        }
    }

    /* public class FileMoveBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
        {

            public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<FileInfo> next)
            {
                
                var response = await next();
                var dFi = new FileInfo(Path.Combine(Path.GetTempPath(), response.Name));
                response.CopyTo(dFi.FullName);
                return dFi;
            }
        } */
}
