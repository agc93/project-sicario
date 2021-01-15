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
    public class ModRequest : IRequest<FileInfo>
    {
        public ModRequest(Dictionary<string, Mod> mods)
        {
            Mods = mods;
        }

        public Dictionary<string, Mod> Mods { get; }
        public bool PackResult { get; set; } = false;
    }

    public class PatchRequestHandler : IRequestHandler<ModRequest, FileInfo>
    {
        private readonly ModPatchServiceBuilder _builder;

        public PatchRequestHandler(ModPatchServiceBuilder servBuilder)
        {
            _builder = servBuilder;
        }
        
        public async Task<FileInfo> Handle(ModRequest request, CancellationToken cancellationToken)
        {
            var mpServ = await _builder.GetPatchService(request.Mods);
            await mpServ.LoadFiles().RunPatches();
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

            return result != null ? (result.Value.Success ? result.Value.Result : null) : null;
        }
    }
}
