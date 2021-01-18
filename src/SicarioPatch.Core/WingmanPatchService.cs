using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using HexPatch;
using HexPatch.Build;
using Microsoft.Extensions.Logging;

namespace SicarioPatch.Core
{
    public class WingmanPatchService : ModPatchService
    {
        private Dictionary<string, int> OriginalFileSize { get; init; } = new Dictionary<string, int>();
        protected internal WingmanPatchService(FilePatcher patcher, SourceFileService fileService, BuildContext context, List<KeyValuePair<string, Mod>> mods, ILogger<ModPatchService> logger) : base(patcher, fileService, context, mods, logger)
        {
        }

        public override async Task<ModPatchService> RunPatches()
        {
            foreach (var (dtmFile, mod) in Mods)
            {
                var modifiedFiles = new List<FileInfo>();
                _logger?.LogInformation($"Running patches for {mod.GetLabel(dtmFile)}");
                foreach (var (targetFile, patchSets) in mod.FilePatches)
                {
                    var srcPath = Path.Join(_ctx.WorkingDirectory.FullName, targetFile);
                    OriginalFileSize[srcPath] = (int) new FileInfo(srcPath).Length;
                    _logger?.LogDebug($"Patching {Path.GetFileName(targetFile)}...");
                    var finalFile = await _patcher.RunPatch(srcPath, patchSets);
                    modifiedFiles.Add(finalFile);
                }
                _logger?.LogDebug($"Modified {modifiedFiles.Count} files: {string.Join(", ", modifiedFiles.Select(f => f.Name))}");
                foreach (var (srcPath, origLength) in OriginalFileSize)
                {
                    var fi = new FileInfo(srcPath);
                    if (fi.Extension == ".uexp" && fi.Length != origLength)
                    {
                        _logger.LogWarning($"Size change detected in {fi.Name}: {origLength} -> {fi.Length}");
                        var uaFile = new FileInfo(fi.FullName.Replace(fi.Extension, ".uasset"));
                        if (uaFile.Exists)
                        {
                            _logger.LogDebug("Detected matching uasset file, attempting to patch length");
                            var lengthBytes = BitConverter.ToString(BitConverter.GetBytes(((int) origLength - 4))).Replace("-", string.Empty);
                            var correctedBytes = BitConverter.ToString(BitConverter.GetBytes(((int) fi.Length - 4))).Replace("-", string.Empty);
                            var lPatch = new PatchSet()
                            {
                                Name = "Length auto-correct",
                                Patches = new List<Patch>
                                {
                                    new Patch
                                    {
                                        Description = "uexp Length",
                                        Template = lengthBytes,
                                        Substitution = correctedBytes,
                                        Type = SubstitutionType.InPlace
                                    }
                                }
                            };
                            var finalFile = await _patcher.RunPatch(uaFile.FullName, new List<PatchSet>{lPatch});
                        }
                    } 
                }
            }
            return this;
        }
    }

        /// <summary>
        /// This builder exists only as a very shitty wrapper over the ModPatchService to make it more DI-friendly.
        /// Inject a *Builder then use that to create however many services you need.
        /// It's shit. I know.
        /// </summary>
        public class WingmanPatchServiceBuilder
        {
            private readonly SourceFileService _fileService;
            private readonly FilePatcher _filePatcher;
            private readonly BuildContextFactory _ctxFactory;
            private readonly ILogger<ModPatchService> _tgtLogger;

            public WingmanPatchServiceBuilder(SourceFileService sourceFileService, FilePatcher filePatcher, BuildContextFactory contextFactory, ILogger<ModPatchService> logger)
            {
                _fileService = sourceFileService;
                _filePatcher = filePatcher;
                _ctxFactory = contextFactory;
                _tgtLogger = logger;
            }

            public async Task<WingmanPatchService> GetPatchService(IEnumerable<KeyValuePair<string, Mod>> modCollection, string ctxName = null)
            {
                var mods = modCollection.ToList();
                var ctx = await _ctxFactory.Create(ctxName);
                return new WingmanPatchService(_filePatcher, _fileService, ctx, mods, _tgtLogger);

            }
        }
}