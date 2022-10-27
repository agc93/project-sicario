using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using Microsoft.Extensions.Logging;
using ModEngine.Build;
using ModEngine.Core;

namespace SicarioPatch.Core
{
    public class WingmanPatchEngine : ModPatchService<WingmanMod, DirectoryBuildContext>
    {
        // private readonly List<PatchEngineDefinition<Patch>> _patchEngines = new();
        // private IEnumerable<PatchEngineDefinition<Patch>> PatchEngines => _patchEngines.OrderBy(p => p.Priority);

        public WingmanPatchEngine(List<WingmanMod> mods, DirectoryBuildContext context, ISourceFileService fileService, IModBuilder? modBuilder, IEnumerable<PatchEngineDefinition<WingmanMod, Patch>> patchEngineDefinitions, ILogger<WingmanPatchEngine>? logger) : base(mods, context, fileService, modBuilder, patchEngineDefinitions, logger) {
        }

        public override async Task<ModPatchService<WingmanMod, DirectoryBuildContext>> RunPatches() {
            foreach (var patchEngine in PatchEngines) {
                foreach (var mod in Mods) {
                    var modifiedFiles = new List<FileInfo>();
                    Logger?.LogInformation($"Running patches for {mod.GetLabel()}");
                    var patches = patchEngine.PatchSelector(mod);
                    foreach (var (targetFile, patchSets) in patches) {
                        var srcFile = new SourceFile(targetFile);
                        try {
                            var realFile = BuildContext.GetFile(targetFile);
                            if (realFile != null) {
                                srcFile.File = realFile;
                            }
                        }
                        catch {
                            // ignored
                        }
                        var patchSetList = patchSets.ToList();
                        Logger?.LogDebug($"Patching {Path.GetFileName(targetFile)}...");
                        var fi = await patchEngine.Engine.RunPatch(srcFile, patchSetList);
                        modifiedFiles.AddRange(fi);
                    }
                    Logger?.LogDebug($"Modified {modifiedFiles.Count} files: {string.Join(", ", modifiedFiles.Select(f => f.Name))}");
                }
            }

            return this;
        }
    }
}