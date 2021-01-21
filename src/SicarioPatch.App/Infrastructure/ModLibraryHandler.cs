using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HexPatch;
using HexPatch.Build;
using MediatR;
using SicarioPatch.App.Shared;
using SicarioPatch.Core;

namespace SicarioPatch.App.Infrastructure
{
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