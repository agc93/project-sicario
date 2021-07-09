using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using SicarioPatch.Core;
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
        }
#pragma warning restore 8618

        public BuildCommand(IAnsiConsole console, SkinSlotLoader slotLoader, IMediator mediator, PresetFileLoader presetLoader, MergeLoader mergeLoader, IConfiguration config, Integration.GameFinder gameFinder) {
            _console = console;
            _slotLoader = slotLoader;
            _mediator = mediator;
            _presetLoader = presetLoader;
            _mergeLoader = mergeLoader;
            _config = config;
            _gameFinder = gameFinder;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
            if (string.IsNullOrWhiteSpace(settings.InstallPath)) {
                var install = _gameFinder.GetGamePath();
                if (install == null) {
                    _console.MarkupLine("[bold red]Error![/] [orange]Could not locate game install folder![/]");
                    return 412;
                }
                settings.InstallPath = install;
            }
            
            if (!Directory.Exists(settings.InstallPath)) {
                _console.MarkupLine($"[red][bold]Install not found![/] The game install directory doesn't exist.[/]");
                return 404;
            }

            var paksRoot = Path.Join(settings.InstallPath, "ProjectWingman", "Content", "Paks");

            _config["GamePath"] = settings.InstallPath;
            _config["GamePakPath"] = Path.Join(paksRoot, "ProjectWingman-WindowsNoEditor.pak");

            var presetSearchPaths = new List<string>(settings.PresetPaths ?? Array.Empty<string>()) {
                Path.Join(settings.InstallPath, "ProjectWingman", "Content", "Presets"),
                Path.Join(paksRoot, "~mods")
            };
            
            var existingMods = _mergeLoader.GetSicarioMods().ToList();
            var mergedInputs = existingMods
                .Select(m => m.TemplateInputs)
                .Aggregate(new Dictionary<string, string>(), (total, next) => next.MergeLeft(total));
            
            var presetPaths = presetSearchPaths
                    .Where(Directory.Exists)
                    .SelectMany(d => Directory.EnumerateFiles(d, "*.dtp", SearchOption.AllDirectories));
            var presets = _presetLoader.LoadFromFiles(presetPaths).ToList();

            var mergedPresetInputs = presets
                .Select(p => p.ModParameters)
                .Aggregate(mergedInputs,
                (total, next) => total.MergeLeft(next)
                );

            


            var slotLoader = _slotLoader.GetSlotMod();

            var allMods = existingMods.SelectMany(m => m.Mods).ToList();
            allMods.AddRange(presets.SelectMany(p => p.Mods));
            allMods.Add(slotLoader);

            var req = new PatchRequest(allMods) {
                PackResult = true,
                TemplateInputs = mergedPresetInputs,
                Name = "SicarioMaster",
                UserName = $"loader:{Environment.MachineName}"
            };
            var resp = await _mediator.Send(req);
            var targetPath = Path.Join(paksRoot, "~sicario");
            if (!Directory.Exists(targetPath)) {
                Directory.CreateDirectory(targetPath);
            }

            if (Directory.GetFiles(targetPath).Any() && !(settings.SkipTargetClean.IsSet && settings.SkipTargetClean.Value)) {
                foreach (var file in Directory.GetFiles(targetPath)) {
                    File.Delete(file);
                }
            }
            resp.MoveTo(Path.Join(targetPath, resp.Name));
            return 0;
        }
    }
}