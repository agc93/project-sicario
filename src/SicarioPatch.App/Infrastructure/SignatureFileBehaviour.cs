using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using SicarioPatch.App.Shared;
using SicarioPatch.Core;

namespace SicarioPatch.App.Infrastructure
{
    public class SignatureFileBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
    {
        private readonly List<string> _files;
        private readonly ModParser _parser;
        private readonly AppInfoProvider _info;
        private readonly BrandProvider _brand;
        private readonly bool _enableRequestEmbed;

        public SignatureFileBehaviour(IConfiguration config, ModParser parser, BrandProvider brand, AppInfoProvider info)
        {
            var section = config.GetSection("SignatureFiles");
            var requestEmbed = config.GetValue<bool>("RequestEmbed", true);
            _files = section.Exists() ? section.Get<List<string>>() : new List<string>();
            _enableRequestEmbed = requestEmbed;
            _parser = parser;
            _brand = brand;
            _info = info;
        }
        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<FileInfo> next)
        {
            var summaryFile = new FileInfo(Path.Combine(Path.GetTempPath(), $"{request.Id}.json"));
            var extraFiles = new Dictionary<string, FileInfo>();
            if (_files.Any())
            {
                var availableFiles = _files.Select(f => new FileInfo(f)).Where(f => f.Exists);
                foreach (var availableFile in availableFiles)
                {
                    extraFiles.Add("_meta/sicario", availableFile);
                }
            }

            if (_enableRequestEmbed) {
                try {
                    var (appName, appVersion) = _info.GetAppInfo();
                    var metaFile = new
                        {app = new {owner = _brand.OwnerName, name = appName, version = appVersion}, request = request};
                    var json = JsonSerializer.Serialize(metaFile, _parser.RelaxedOptions);
                    if (!string.IsNullOrWhiteSpace(json)) {
                        await File.WriteAllTextAsync(summaryFile.FullName, json, cancellationToken);
                        if (summaryFile.Exists) {
                            extraFiles.Add("_meta/sicario", summaryFile);
                        }
                    }
                }
                catch {
                    //ignored
                }
            }


            request.AdditionalFiles = extraFiles;
            var result = await next();
            return result;
        }
    }
}