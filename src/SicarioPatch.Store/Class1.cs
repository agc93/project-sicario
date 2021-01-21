using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SicarioPatch.Core;

namespace SicarioPatch.Store
{
    public class ModIndexHandler : IPipelineBehavior<ModUploadRequest, string>,
        IPipelineBehavior<ModDeleteRequest, bool>,
        IPipelineBehavior<ModsRequest, Dictionary<string, Mod>>
    {
        private IFileIndexService _index;

        public ModIndexHandler(IFileIndexService indexService)
        {
            _index = indexService;
        }
        public async Task<string> Handle(ModUploadRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<string> next)
        {
            var result = await next();
            if (!string.IsNullOrWhiteSpace(result))
            {
                _index.AddFile(result, result);
            }
            return result;
        }

        public async Task<bool> Handle(ModDeleteRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<bool> next)
        {
            var result = await next();
            if (result)
            {
                var fileEntry = _index.GetFileByName(request.FileName);
                _index.RemoveFile(fileEntry.Id);
            }
            return result;
        }

        public async Task<Dictionary<string, Mod>> Handle(ModsRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Dictionary<string, Mod>> next)
        {
            var allMods = await next();
            if (!request.IncludeBeta && _index.GetIndex() is var index && index.Any())
            {
                // var index = _index.GetIndex();
                allMods = allMods.Where(mf => index.Values.Any(v => v.Name == Path.GetFileName(mf.Key))).ToDictionary();
            }

            return allMods;
        }
    }
}
