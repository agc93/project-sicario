using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HexPatch.Build;
using MediatR;

namespace SicarioPatch.Core
{
    public class PatchRequestHandler : IRequestHandler<PatchRequest, FileInfo>
    {
        private readonly WingmanPatchServiceBuilder _builder;

        public PatchRequestHandler(WingmanPatchServiceBuilder servBuilder)
        {
            _builder = servBuilder;
        }

        private readonly Dictionary<string, IEnumerable<string>> _sideCars = new Dictionary<string, IEnumerable<string>> {
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
        }
    }
}