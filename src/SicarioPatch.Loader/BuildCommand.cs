using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SicarioPatch.Core;
using SicarioPatch.Core.Diagnostics;
using SicarioPatch.Integration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SicarioPatch.Loader
{
    public class BuildCommand : AsyncCommand<BuildCommand.Settings>
    {
        private readonly IAnsiConsole _console;
        private readonly SkinSlotLoader _slotLoader;
        private readonly IMediator _mediator;
        private readonly PresetFileLoader _presetLoader;
        private readonly MergeLoader _mergeLoader;
        private readonly IConfiguration _config;
        private readonly Integration.GameFinder _gameFinder;
        private readonly ILogger<BuildCommand> _logger;
        private readonly ModParser _parser;
        
#pragma warning disable 8618
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class Settings : CommandSettings
        {
            [CommandOption(("-r|--run"))]
            [Description("Attempts to launch the game after completing the merge build.")]
            public FlagValue<bool> RunAfterBuild { get; set; }

            [CommandOption("--installPath")]
            [Description("Sets the game install path. Overrides the automatic detection.")]
            public string? InstallPath { get; set; }
            
            [CommandArgument(0, "[presetPaths]")]
            public string[]? PresetPaths { get; init; }
            
            [CommandOption("--no-clean")]
            [Description("Do not remove existing files in the merge output directory.")]
            public FlagValue<bool> SkipTargetClean { get; init; }
            [CommandOption("--outputPath")]
            [Description("Set the path to write the merged mod to.")]
            public string? OutputPath { get; set; }
            
            [CommandOption("-q|--quiet")]
            [Description("Reduce the amount of information written to the console.")]
            public FlagValue<bool> Quiet { get; set; }
            
            [CommandOption("--non-interactive")]
            [Description("Ensures that there are no prompts or confirmations while building.")]
            public bool NonInteractive { get; set; }
            
            [CommandOption("--report")]
            [Description("Writes a report file with the given name with details of the merged mod build. Format subject to change.")]
            public string? ReportFile { get; set; }
        }
#pragma warning restore 8618

        public BuildCommand(IAnsiConsole console, SkinSlotLoader slotLoader, IMediator mediator,
            PresetFileLoader presetLoader, MergeLoader mergeLoader, IConfiguration config,
            Integration.GameFinder gameFinder, ILogger<BuildCommand> logger, ModParser parser) {
            _console = console;
            _slotLoader = slotLoader;
            _mediator = mediator;
            _presetLoader = presetLoader;
            _mergeLoader = mergeLoader;
            _config = config;
            _gameFinder = gameFinder;
            _logger = logger;
            _parser = parser;
        }

        internal record MergePart
        {
             internal Dictionary<string, string> Parameters { get; init; } = new();
             internal IEnumerable<WingmanMod> Mods { get; init; } = new List<WingmanMod>();
             internal int Priority { get; init; } = 10;
             internal string? Message { get; init; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
            void BuildTargetPath(string s) {
                if (!Directory.Exists(s)) {
                    Directory.CreateDirectory(s);
                }

                if (Directory.GetFiles(s).Any() &&
                    !(settings.SkipTargetClean.IsSet && settings.SkipTargetClean.Value)) {
                    foreach (var file in Directory.GetFiles(s)) {
                        File.Delete(file);
                    }
                }
            }

            void LogConsole(string message) {
                if (settings.Quiet.IsSet && settings.Quiet.Value) {
                    return;
                }
                _console.MarkupLine(message);
            }
            if (string.IsNullOrWhiteSpace(settings.InstallPath)) {
                var install = _gameFinder.GetGamePath();
                install ??= new LocalGameFinder().GetGamePath() ?? new LocalGameFinder(Environment.CurrentDirectory).GetGamePath();
                if (install == null) {
                    LogConsole("[bold red]Error![/] [orange3]Could not locate game install folder![/]");
                    return 412;
                }
                settings.InstallPath = install;
            }
            
            if (!Directory.Exists(settings.InstallPath)) {
                LogConsole($"[red][bold]Install not found![/] The game install directory doesn't exist.[/]");
                return 404;
            }

            var paksRoot = Path.Join(settings.InstallPath, "ProjectWingman", "Content", "Paks");
            var report = new MergeReport(settings.InstallPath);

            _config["GamePath"] = settings.InstallPath;
            _config["GamePakPath"] = Path.Join(paksRoot, "ProjectWingman-WindowsNoEditor.pak");

            var partsList = new List<MergePart>();
            
            

            var presetSearchPaths = new List<string>(settings.PresetPaths ?? Array.Empty<string>()) {
                Path.Join(settings.InstallPath, "ProjectWingman", "Content", "Presets"),
                Path.Join(paksRoot, "~mods")
            };

            _logger.LogDebug($"Searching {presetSearchPaths.Count} paths for loose presets");

            //existing mods
            
            var existingMods = _mergeLoader.GetSicarioMods(out List<string> inputMods).ToList();
            var mergedInputs = existingMods
                .Select(m => m.TemplateInputs)
                .Aggregate(new Dictionary<string, string>(), (total, next) => next.MergeLeft(total));
            
            partsList.Add(new MergePart {
                Mods = existingMods.SelectMany(m => m.Mods),
                Parameters = mergedInputs,
                Priority = 3
            });
            
            LogConsole($"[dodgerblue2]Loaded [bold]{existingMods.Count}[/] Sicario mods for rebuild[/]");
            report.RebuildMods = inputMods;
            
            
            
            //embedded presets

            var embeddedPresets = _mergeLoader.LoadPresetsFromMods().ToList();
            var embeddedInputs = embeddedPresets
                .Select(m => m.ModParameters)
                .Aggregate(new Dictionary<string, string>(),
                    (total, next) => total.MergeLeft(next)
                );
            
            LogConsole($"[dodgerblue2]Loaded [bold]{embeddedPresets.Count}[/] embedded presets from installed mods[/]");

            partsList.Add(new MergePart {
                Mods = embeddedPresets.SelectMany(ep => ep.Mods),
                Parameters = embeddedInputs,
                Priority = 1
            });
            
            //loose presets
            
            var presetPaths = presetSearchPaths
                    .Where(Directory.Exists)
                    .SelectMany(d => Directory.EnumerateFiles(d, "*.dtp", SearchOption.AllDirectories))
                    .OrderBy(f => f)
                    .ToList();
            var presets = _presetLoader.LoadFromFiles(presetPaths).ToList();
            
            LogConsole($"[dodgerblue2]Loaded [bold]{presets.Count}[/] loose presets from file.[/]");
            report.PresetPaths = presetPaths;

            var loosePresetInputs = presets
                .Select(p => p.ModParameters)
                .Aggregate(new Dictionary<string, string>(),
                (total, next) => total.MergeLeft(next)
                );
            
            partsList.Add(new MergePart {
                Mods = presets.SelectMany(p => p.Mods),
                Parameters = loosePresetInputs,
                Priority = 2
            });
            
            //skin merge
            
            var skinPaths = _slotLoader.GetSkinPaths();
            var slotPatches = _slotLoader.GetSlotPatches(skinPaths).ToList();
            var slotLoader = _slotLoader.GetSlotMod(slotPatches);
            report.AdditionalSkins = skinPaths
                .ToDictionary(k => k.Key, v => v.Value.Select(p => Path.ChangeExtension(p, null)).Distinct().ToList())
                .ToDictionary(k => k.Key, v => v.Value);
            LogConsole($"[dodgerblue2]Successfully compiled skin merge with [bold]{slotLoader.GetPatchCount()}[/] patches.[/]");
            partsList.Add(new MergePart {
                Mods = new []{slotLoader},
            });
            
            //final merge

            var inputParameterList = partsList
                .OrderByDescending(p => p.Priority)
                .Select(p => p.Parameters)
                .Aggregate(new Dictionary<string, string>(), 
                    (total, next) => total.MergeLeft(next)
                );

            LogConsole($"Final mod will be built with [dodgerblue2]{inputParameterList.Keys.Count}[/] parameters");
            report.InputParameters = inputParameterList;

            var modList = partsList
                .OrderBy(p => p.Priority)
                .SelectMany(p => p.Mods)
                .ToList();

            LogConsole($"[bold darkblue]Queuing mod build with {modList.Count} mods[/]");
            
            var targetPath = string.IsNullOrWhiteSpace(settings.OutputPath)
                ? Path.Join(paksRoot, "~sicario")
                : settings.OutputPath;
            
            if (!string.IsNullOrWhiteSpace(settings.ReportFile)) {
                //build report
                await WriteReport(settings.ReportFile, targetPath, report);
            }

            var req = new PatchRequest(modList) {
                PackResult = true,
                TemplateInputs = inputParameterList,
                Name = "SicarioMerge",
                UserName = $"loader:{Environment.MachineName}"
            };
            try {
                var resp = await _mediator.Send(req);
                if (string.IsNullOrWhiteSpace(settings.OutputPath)) {
                    LogConsole(
                        $"[green][bold]Success![/] Your merged mod has been built and is now being installed to the game folder[/]");
                    var isVortexManaged = CheckForDeploymentManifest(paksRoot);
                    if (isVortexManaged) {
                        LogConsole("[orange3][bold]Warning![/] Your mods folder appears to be Vortex-managed![/]");
                        LogConsole(
                            "We recommend using Vortex's PSM integration to manage your merged mod automatically.");
                        var toContinue = settings.NonInteractive || (!settings.Quiet.IsSet || !settings.Quiet.Value) &&
                                         _console.Prompt(
                                             new ConfirmationPrompt("Do you want to continue with this build anyway?"));
                        if (!toContinue) {
                            return 204;
                        }
                    }

                    BuildTargetPath(targetPath);

                    resp.MoveTo(Path.Join(targetPath, resp.Name), true);
                    LogConsole($"[dodgerblue2]Your merged mod is installed and you can start the game.[/]");
                }
                else {
                    LogConsole(
                        $"[green][bold]Success![/] Your merged mod has been built and is now being installed to the specified output folder[/]");
                    BuildTargetPath(targetPath);

                    resp.MoveTo(Path.Join(targetPath, resp.Name), true);
                    LogConsole($"[dodgerblue2]Your merged mod is built in the [grey]'{targetPath}'[/] directory.[/]");
                }
            }
            catch (SourceFileNotFoundException sEx) {
                _logger.LogError(sEx.Message);
                return 412;
            }
            catch (Assets.AssetInstructionException aEx) {
                _logger.LogError(aEx,
                    $"Error while running asset instructions, this is likely a bad patch or incorrect duplication.");
                return 422;
            }
            catch (Exception e) {
                _logger.LogError(e, "An unhandled error was encountered while building the mod file.");
                return 400;
            }

            if (settings.RunAfterBuild.IsSet && settings.RunAfterBuild.Value) {
                var launcher = new GameLauncher(settings.InstallPath);
                launcher.RunGame();
                // because we're using an ancient version of ExecEngine
                // this will actually wait for the game to exit.
                // not ideal, but not a deal-breaker imo
            }

            return 0;
        }

        private async Task WriteReport(string reportFilePath, string? targetPath, MergeReport report) {
            targetPath ??= AppContext.BaseDirectory;
            if (!Path.IsPathRooted(reportFilePath)) {
                reportFilePath = Path.Join(targetPath, reportFilePath);
            }

            var opts = new JsonSerializerOptions(_parser.Options) {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(report, opts);
            await File.WriteAllTextAsync(reportFilePath, json);
        }

        private static bool CheckForDeploymentManifest(string paksPath) {
            var files = new DirectoryInfo(paksPath).EnumerateFiles("vortex.deployment.json",
                SearchOption.AllDirectories);
            return files.Any(f => f.Length > 0);
        }
    }
}