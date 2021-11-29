using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using HexPatch;
using Microsoft.Extensions.Logging;
using ModEngine.Build;
using ModEngine.Build.Diagnostics;
using SicarioPatch.Assets;
using Patch = ModEngine.Core.Patch;
using PatchSet = ModEngine.Core.PatchSet;

namespace SicarioPatch.Core
{
    public class WingmanPatchService : ModPatchService<WingmanMod, DirectoryBuildContext>
    {
        private readonly AssetPatcher _assetPatcher;
        private readonly FilePatcher _filePatcher;
        private Dictionary<string, int> OriginalFileSize { get; init; } = new Dictionary<string, int>();

        protected internal WingmanPatchService(AssetPatcher aPatcher, FilePatcher patcher, ISourceFileService fileService, DirectoryBuildContext context, IModBuilder modBuilder, List<WingmanMod> mods, ILogger<ModPatchService<WingmanMod, DirectoryBuildContext>> logger) : base(context, fileService, modBuilder, logger) {
            _filePatcher = patcher;
            Mods.AddRange(mods);
            _assetPatcher = aPatcher;
        }

        public async Task<ModPatchService<WingmanMod, DirectoryBuildContext>> RunAssetPatches() {
            foreach (var mod in Mods) {
                foreach (var (targetAssetKey, assetPatchSets) in mod.AssetPatches) {
                    var targetAsset = targetAssetKey;
                    string targetAssetName = null;
                    if (targetAsset.Contains('>')) {
                        //fancy rewrite incoming
                        var assetSplit = targetAsset.Split('>');
                        targetAsset = assetSplit.First();
                        targetAssetName = assetSplit.Last();
                    }
                    var srcPath = Path.Join(Context.WorkingDirectory.FullName, targetAsset);
                    Logger?.LogDebug($"Running asset patches for {Path.GetFileName(targetAsset)}");
                    var _ = await _assetPatcher.RunPatch(srcPath, assetPatchSets, targetAssetName);
                }
            }

            return this;
        }

        public async Task<ModPatchService<WingmanMod, DirectoryBuildContext>> RunAllPatches()
        {
            foreach (var mod in Mods)
            {
                var modifiedFiles = new List<FileInfo>();
                Logger?.LogInformation($"Running patches for {mod.GetLabel(mod.Id ?? "Unknown mod")}");
                // _logger?.LogInformation($"Running patches for {mod.Id}");
                foreach (var (targetFile, patchSets) in mod.FilePatches)
                {
                    var srcPath = Path.Join(Context.WorkingDirectory.FullName, targetFile);
                    OriginalFileSize[srcPath] = (int) new FileInfo(srcPath).Length;
                    Logger?.LogDebug($"Patching {Path.GetFileName(targetFile)}...");
                    var finalFile = await _filePatcher.RunPatch(srcPath, patchSets);
                    modifiedFiles.Add(finalFile);
                }
                Logger?.LogDebug($"Modified {modifiedFiles.Count} files: {string.Join(", ", modifiedFiles.Select(f => f.Name))}");
                foreach (var (srcPath, origLength) in OriginalFileSize)
                {
                    var fi = new FileInfo(srcPath);
                    if (fi.Extension == ".uexp" && fi.Length != origLength)
                    {
                        Logger.LogWarning($"Size change detected in {fi.Name}: {origLength} -> {fi.Length}");
                        var uaFile = new FileInfo(fi.FullName.Replace(fi.Extension, ".uasset"));
                        if (uaFile.Exists)
                        {
                            Logger.LogDebug("Detected matching uasset file, attempting to patch length");
                            var lengthBytes = BitConverter.ToString(BitConverter.GetBytes(((int) origLength - 4))).Replace("-", string.Empty);
                            var correctedBytes = BitConverter.ToString(BitConverter.GetBytes(((int) fi.Length - 4))).Replace("-", string.Empty);
                            var lPatch = new FilePatchSet()
                            {
                                Name = "Length auto-correct",
                                Patches = new List<FilePatch>
                                {
                                    new FilePatch
                                    {
                                        Description = "uexp Length",
                                        Template = lengthBytes,
                                        Substitution = correctedBytes,
                                        Type = "inPlace"
                                    }
                                }
                            };
                            var finalFile = await _filePatcher.RunPatch(uaFile.FullName, new List<FilePatchSet>{lPatch});
                        }
                    } 
                }
                
            }
            return this;
        }
        
        public WingmanPatchService LoadAssetFiles(Func<string, IEnumerable<string>> extraFileSelector = null)
        {
            var requiredFiles = this.Mods
                .SelectMany(em => em.AssetPatches)
                .GroupBy(fp => fp.Key)
                .Where(g => g.Any())
                .Select(g => g.Key)
                .Select(g => g.Split('>').FirstOrDefault())
                .Where(g => g != null)
                .Distinct()
                .ToList();
            foreach (var file in requiredFiles)
            {
                var srcFile = FileService.LocateFile(Path.GetFileName(file));
                if (srcFile == null) throw new SourceFileNotFoundException(Path.GetFileName(file)); 
                this.BuildContext.AddFile(Path.GetDirectoryName(file), srcFile);
                if (extraFileSelector != null) {
                    var extraFiles = extraFileSelector.Invoke(file) ?? new List<string>();
                    foreach (var eFile in extraFiles) {
                        var exFile = FileService.LocateFile(Path.GetFileName(eFile));
                        this.BuildContext.AddFile(Path.GetDirectoryName(eFile), exFile);
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
            private readonly ISourceFileService _fileService;
            private readonly FilePatcher _filePatcher;
            private readonly AssetPatcher _assetPatcher;
            private readonly DirectoryBuildContextFactory _ctxFactory;
            private readonly IModBuilder _modBuilder;
            private readonly ILogger<ModPatchService<WingmanMod, DirectoryBuildContext>> _tgtLogger;

            public WingmanPatchServiceBuilder(ISourceFileService sourceFileService, FilePatcher filePatcher, AssetPatcher assetPatcher, DirectoryBuildContextFactory contextFactory, IModBuilder modBuilder, ILogger<ModPatchService<WingmanMod, DirectoryBuildContext>> logger)
            {
                _fileService = sourceFileService;
                _filePatcher = filePatcher;
                _assetPatcher = assetPatcher;
                _ctxFactory = contextFactory;
                _modBuilder = modBuilder;
                _tgtLogger = logger;
            }

            public async Task<WingmanPatchService> GetPatchService(IEnumerable<WingmanMod> modCollection, string ctxName = null)
            {
                var mods = modCollection.ToList();
                var ctx = _ctxFactory.CreateContext(ctxName);
                return new WingmanPatchService(_assetPatcher, _filePatcher, _fileService, ctx, _modBuilder, mods, _tgtLogger);
            }
        }
}