using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly Dictionary<string, IEnumerable<string>> _sideCars = new() {
            [".uexp"] = new string[] {".uasset"},
            [".uasset"] = new [] {".uexp"},
            [".umap"] = new[] {".uexp"}
        };
        
        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken)
        {
            using var mpServ = await _builder.GetPatchService(request.Mods, request.Name);
            if (request.AdditionalFiles.Any())
            {
                mpServ.PreBuildAction = context =>
                {
                    foreach (var (relPath, file) in request.AdditionalFiles)
                    {
                        context.AddFile(Path.Combine(context.WorkingDirectory.GetDirectories().First().Name, relPath), file);
                    }
                    return null;
                };
            }

            mpServ.LoadAssetFiles(FileSelectors.SidecarFiles(_sideCars)).LoadFiles(FileSelectors.SidecarFiles(_sideCars));
            await mpServ.RunPatches();
            await mpServ.RunAssetPatches();
            (bool Success, FileInfo Result)? result;
            if (request.PackResult)
            {
                result = await mpServ.RunBuild($"merged-{DateTime.UtcNow.Ticks}_P.pak");
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