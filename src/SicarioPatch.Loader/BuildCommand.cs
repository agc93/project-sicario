using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SicarioPatch.Core;
using SicarioPatch.Integration;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Extensions.Logging;

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
        private readonly SpectreConsoleLoggerConfiguration _loggerConfiguration;
#pragma warning disable 8618
        public class Settings : CommandSettings
        {
            [CommandOption(("-r|--run"))]

            public FlagValue<bool> RunAfterBuild { get; set; }

            [CommandOption("--installPath")]
            public string? InstallPath { get; set; }
            
            [CommandArgument(0, "[presetPaths]")]
            public string[]? PresetPaths { get; init; }
            
            [CommandOption("--no-clean")]
            public FlagValue<bool> SkipTargetClean { get; init; }
            [CommandOption("--outputPath")]
            public string? OutputPath { get; set; }
            
            [CommandOption("-q|--quiet")]
            public FlagValue<bool> Quiet { get; set; }
            
            [CommandOption("--report")]
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

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
            void LogConsole(string message) {
                if (settings.Quiet.IsSet && settings.Quiet.Value) {
                    return;
                }
                _console.MarkupLine(message);
            }
            if (string.IsNullOrWhiteSpace(settings.InstallPath)) {
                var install = _gameFinder.GetGamePath();
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

            var presetSearchPaths = new List<string>(settings.PresetPaths ?? Array.Empty<string>()) {
                Path.Join(settings.InstallPath, "ProjectWingman", "Content", "Presets"),
                Path.Join(paksRoot, "~mods")
            };
            
            _logger.LogDebug($"Searching {presetSearchPaths.Count} paths for loose presets");

            var existingMods = _mergeLoader.GetSicarioMods(out List<string> inputMods).ToList();
            var mergedInputs = existingMods
                .Select(m => m.TemplateInputs)
                .Aggregate(new Dictionary<string, string>(), (total, next) => next.MergeLeft(total));
            
            LogConsole($"[dodgerblue2]Loaded [bold]{existingMods.Count}[/] Sicario mods for rebuild[/]");
            report.RebuildMods = inputMods;
            

            var embeddedPresets = _mergeLoader.LoadPresetsFromMods().ToList();
            var embeddedInputs = embeddedPresets
                .Select(m => m.ModParameters)
                .Aggregate(mergedInputs,
                    (total, next) => total.MergeLeft(next)
                );
            
            LogConsole($"[dodgerblue2]Loaded [bold]{embeddedPresets.Count}[/] embedded presets from installed mods[/]");
            
            var presetPaths = presetSearchPaths
                    .Where(Directory.Exists)
                    .SelectMany(d => Directory.EnumerateFiles(d, "*.dtp", SearchOption.AllDirectories))
                    .ToList();
            var presets = _presetLoader.LoadFromFiles(presetPaths).ToList();
            
            LogConsole($"[dodgerblue2]Loaded [bold]{presets.Count}[/] loose presets from file.[/]");
            report.PresetPaths = presetPaths;

            var mergedPresetInputs = presets
                .Select(p => p.ModParameters)
                .Aggregate(embeddedInputs,
                (total, next) => total.MergeLeft(next)
                );

            LogConsole($"Final mod will be built with [dodgerblue2]{mergedPresetInputs.Keys.Count}[/] parameters");
            report.InputParameters = mergedPresetInputs;

            var skinPaths = _slotLoader.GetSkinPaths();
            var slotLoader = _slotLoader.GetSlotMod(_slotLoader.GetSlotPatches(skinPaths));
            report.AdditionalSkins = skinPaths;
            
            LogConsole($"[dodgerblue2]Successfully compiled skin merge with [bold]{slotLoader.GetPatchCount()}[/] patches.[/]");

            var allMods = existingMods.SelectMany(m => m.Mods).ToList();
            allMods.AddRange(embeddedPresets.SelectMany(p => p.Mods));
            allMods.AddRange(presets.SelectMany(p => p.Mods));
            allMods.Add(slotLoader);
            
            LogConsole($"[bold darkblue]Queuing mod build with {allMods.Count} mods[/]");
            

            var req = new PatchRequest(allMods) {
                PackResult = true,
                TemplateInputs = mergedPresetInputs,
                Name = "SicarioMerge",
                UserName = $"loader:{Environment.MachineName}"
            };
            var resp = await _mediator.Send(req);
            string targetPath;
            if (string.IsNullOrWhiteSpace(settings.OutputPath)) {
                LogConsole(
                    $"[green][bold]Success![/] Your merged mod has been built and is now being installed to the game folder[/]");
                var isVortexManaged = CheckForDeploymentManifest(paksRoot);
                if (isVortexManaged) {
                    LogConsole("[orange3][bold]Warning![/] Your mods folder appears to be Vortex-managed![/]");
                    LogConsole("We recommend using Vortex's PSM integration to manage your merged mod automatically.");
                    var toContinue = (!settings.Quiet.IsSet || !settings.Quiet.Value) && _console.Prompt(new ConfirmationPrompt("Do you want to continue with this build anyway?"));
                    if (!toContinue) {
                        return 204;
                    }
                }
                targetPath = Path.Join(paksRoot, "~sicario");
                if (!Directory.Exists(targetPath)) {
                    Directory.CreateDirectory(targetPath);
                }

                if (Directory.GetFiles(targetPath).Any() &&
                    !(settings.SkipTargetClean.IsSet && settings.SkipTargetClean.Value)) {
                    foreach (var file in Directory.GetFiles(targetPath)) {
                        File.Delete(file);
                    }
                }

                resp.MoveTo(Path.Join(targetPath, resp.Name), true);
                LogConsole($"[dodgerblue2]Your merged mod is installed and you can start the game.[/]");
            }
            else {
                targetPath = settings.OutputPath;
                LogConsole(
                    $"[green][bold]Success![/] Your merged mod has been built and is now being installed to the specified output folder[/]");
                if (!Directory.Exists(targetPath)) {
                    Directory.CreateDirectory(targetPath);
                }

                if (Directory.GetFiles(targetPath).Any() &&
                    !(settings.SkipTargetClean.IsSet && settings.SkipTargetClean.Value)) {
                    foreach (var file in Directory.GetFiles(targetPath)) {
                        File.Delete(file);
                    }
                }

                resp.MoveTo(Path.Join(targetPath, resp.Name), true);
                LogConsole($"[dodgerblue2]Your merged mod is built in the [grey]'{targetPath}'[/] directory.[/]");
            }

            if (!string.IsNullOrWhiteSpace(settings.ReportFile)) {
                //build report
                if (!Path.IsPathRooted(settings.ReportFile)) {
                    settings.ReportFile = Path.Join(targetPath, settings.ReportFile);
                }
                var opts = new JsonSerializerOptions(_parser.Options) {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(report, opts);
                await File.WriteAllTextAsync(settings.ReportFile, json);
            }

            if (settings.RunAfterBuild.IsSet && settings.RunAfterBuild.Value) {
                //run game here
            }

            return 0;
        }

        private static bool CheckForDeploymentManifest(string paksPath) {
            var files = new DirectoryInfo(paksPath).EnumerateFiles("vortex.deployment.json",
                SearchOption.AllDirectories);
            return files.Any(f => f.Length > 0);
        }
    }
}