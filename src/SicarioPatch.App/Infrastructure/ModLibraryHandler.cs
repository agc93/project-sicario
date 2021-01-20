using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileStorEngine.Services;
using HexPatch;
using HexPatch.Build;
using MediatR;
using SicarioPatch.App.Shared;
using SicarioPatch.Core;

namespace SicarioPatch.App.Infrastructure
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
    public class ModLibraryHandler : IRequestHandler<ModUploadRequest, string>, IRequestHandler<ModDeleteRequest, bool>
    {
        private ModFileLoader _loader;
        private ModLoadOptions _fileOpts;
        private ModParser _parser;

        public ModLibraryHandler(ModFileLoader loader, ModLoadOptions loadOpts, ModParser parser)
        {
            _loader = loader;
            _fileOpts = loadOpts;
            _parser = parser;
            // _index = index;
        }
        
        public async Task<string> Handle(ModUploadRequest request, CancellationToken cancellationToken)
        {
            var src = _fileOpts?.Sources?.FirstOrDefault();
            if (src == null)
            {
                throw new Exception("No available storage configured!");
            }
            var di = new DirectoryInfo(src);
            var fi = new FileInfo(Path.Combine(di.FullName, request.FileName));
            if (fi.Exists)
            {
                throw new Exception("File with that name already exists. Change the name and try saving again!");
            }
            else
            {
                await File.WriteAllTextAsync(fi.FullName, _parser.ToJson(request.Mod), Encoding.UTF8, cancellationToken);
            }
            return fi.Exists ? fi.Name : fi.Name;
        }

        public async Task<bool> Handle(ModDeleteRequest request, CancellationToken cancellationToken)
        {
            var allSources = _fileOpts?.Sources ?? new List<string>();
            foreach (var sourcePath in allSources)
            {
                var localFi = new FileInfo(Path.Combine(sourcePath, request.FileName));
                if (localFi.Exists)
                {
                    localFi.Delete();
                }
            }

            return !allSources.Any(f =>
                new DirectoryInfo(f).GetFiles(Path.GetFileName(request.FileName), SearchOption.TopDirectoryOnly).Any());
        }
    }
}