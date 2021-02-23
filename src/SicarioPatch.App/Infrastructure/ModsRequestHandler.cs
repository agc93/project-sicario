﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HexPatch;
using MediatR;
using SicarioPatch.Core;

namespace SicarioPatch.App.Infrastructure
{

    public record ModUploadRequest : IRequest<string>
    {
        public string FileName { get; init; }
        public Mod Mod { get; init; }
        public string UploadTarget { get; init; }
    }

    public record ModDeleteRequest : IRequest<bool>
    {
        public string FileName { get; init; }
    }

    public class ModsRequestHandler : IRequestHandler<ModsRequest, Dictionary<string, WingmanMod>>
    {
        private WingmanModLoader _loader;
        private ModLoadOptions _fileOpts;

        public ModsRequestHandler(WingmanModLoader loader, ModLoadOptions loadOpts)
        {
            _loader = loader;
            _fileOpts = loadOpts;
        }

        public async Task<Dictionary<string, WingmanMod>> Handle(ModsRequest request, CancellationToken cancellationToken)
        {
            var allFiles = new List<string>();
            foreach (var sourcePath in _fileOpts?.Sources ?? new List<string>())
            {
                var localFiles = Directory.EnumerateFiles(sourcePath, _fileOpts.Filter, SearchOption.TopDirectoryOnly);
                allFiles.AddRange(localFiles);
            }
            var fileMods = _loader.LoadFromFiles(allFiles).ToDictionary(k => Path.GetFileName(k.Key) ?? k.Key, v => v.Value);
            var allMods = (request.OnlyOwnMods && !string.IsNullOrWhiteSpace(request.UserName))
                ? fileMods.Where(m => !m.Value.ModInfo.Private).Where(MatchesAuthor(request.UserName))
                : fileMods.Where(m => !m.Value.ModInfo.Private);
            if (request.IncludePrivate && !string.IsNullOrWhiteSpace(request.UserName))
            {
                var privateMods = fileMods.Where(m => m.Value.ModInfo.Private).Where(MatchesAuthor(request.UserName));
                allMods = allMods.Concat(privateMods);
            }
            return allMods.ToDictionary();
        }

        private static Func<KeyValuePair<string, WingmanMod>, bool> MatchesAuthor(string author)
        {
            return (modPair) =>
            {
                var mod = modPair.Value;
                return mod?.Metadata != null && mod.Metadata.Author == author;
            };
        }
    }
}