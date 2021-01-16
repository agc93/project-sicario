using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HexPatch;
using HexPatch.Build;
using MediatR;
using SicarioPatch.Core;

namespace SicarioPatch.App.Common
{
    public class ModsRequestHandler : IRequestHandler<ModsRequest, Dictionary<string, Mod>>
    {
        private ModFileLoader _loader;
        private ModLoadOptions _fileOpts;

        public ModsRequestHandler(ModFileLoader loader, ModLoadOptions loadOpts)
        {
            _loader = loader;
            _fileOpts = loadOpts;
        }

        public Task<Dictionary<string, Mod>> Handle(ModsRequest request, CancellationToken cancellationToken)
        {
            var allFiles = new List<string>();
            foreach (var sourcePath in _fileOpts?.Sources ?? new List<string>())
            {
                var localFiles = Directory.EnumerateFiles(sourcePath, "*.dtm", SearchOption.TopDirectoryOnly);
                allFiles.AddRange(localFiles);
            }
            var fileMods = _loader.LoadFromFiles(allFiles);
            return Task.FromResult(fileMods);
        }
    }
}