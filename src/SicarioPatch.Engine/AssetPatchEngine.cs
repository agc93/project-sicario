#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ModEngine.Core;

namespace SicarioPatch.Engine
{
    public class AssetPatchEngine : IPatchEngine<Patch>
    {
        private readonly AssetPatcher _assetPatcher;
        private readonly ILogger<AssetPatchEngine>? _logger;

        public AssetPatchEngine(AssetPatcher assetPatcher, ILogger<AssetPatchEngine>? logger) {
            _assetPatcher = assetPatcher;
            _logger = logger;
        }
        public async Task<IEnumerable<FileInfo>> RunPatch(SourceFile sourceKey, IEnumerable<PatchSet<Patch>> sets, string? targetName = null) {
            var targetAsset = sourceKey;
            if (targetAsset.Key.Contains('>')) {
                //fancy rewrite incoming
                var assetSplit = targetAsset.Key.Split('>');
                targetAsset.Key = assetSplit.First();
                targetAsset.Target = assetSplit.Last();
            }
            // var srcPath = Path.Join(Context.WorkingDirectory.FullName, targetAsset);
            var srcPath = targetAsset;
            _logger?.LogDebug($"Running asset patches for {Path.GetFileName(targetAsset.Key)}");
            
            var resultFile = await _assetPatcher.RunPatch(targetAsset?.File?.FullName ?? srcPath.Key, sets, targetAsset?.Target);
            return new[] { resultFile };
        }
    }
}