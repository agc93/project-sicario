using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SicarioPatch.Core;
using SicarioPatch.Engine;
using SicarioPatch.Integration;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SicarioPatch.Loader
{
    public class PresetPackCommand : AsyncCommand<PresetPackCommand.Settings>
    {
        private readonly ILogger<PresetPackCommand> _logger;
        private readonly Integration.GameFinder _gameFinder;
        private readonly IAnsiConsole _console;
        private readonly IConfiguration _config;
        private readonly IEngineInfoProvider _engineInfo;
        private readonly PresetFileLoader _presetFileLoader;
        private readonly IMediator _mediator;

        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "[presetPaths]")]
            public string[]? PresetPaths { get; init; }
            
            [CommandOption("-n|--name")]
            public string? OutputFileName { get; init; }
            
            [CommandOption("--installPath")]
            [Description("Sets the game install path. Overrides the automatic detection.")]
            public string? InstallPath { get; set; }
        }

        public PresetPackCommand(ILogger<PresetPackCommand> logger, Integration.GameFinder gameFinder, IAnsiConsole console, IConfiguration config, IEngineInfoProvider engineInfo, PresetFileLoader presetFileLoader, IMediator mediator) {
            _logger = logger;
            _gameFinder = gameFinder;
            _console = console;
            _config = config;
            _engineInfo = engineInfo;
            _presetFileLoader = presetFileLoader;
            _mediator = mediator;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
            void LogConsole(string message) {
                
                _console.MarkupLine(message);
            }
            var pakOutputPath = Environment.CurrentDirectory;
            
            
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

            _config["GamePath"] = settings.InstallPath;
            _config["GamePakPath"] = Path.Join(paksRoot, "ProjectWingman-WindowsNoEditor.pak");
            _config["RequestEmbed"] = false.ToString();
            
            _logger.LogInformation($"Running engine version {_engineInfo.GetEngineVersion() ?? "unknown"}");

            var presets = _presetFileLoader.LoadFromFiles(settings.PresetPaths) ?? new Dictionary<string, WingmanPreset>();
            presets = presets.Where(p =>
            {
                var supported = p.Value.IsSupportedBy(_engineInfo);
                if (!supported) {
                    _logger.LogWarning("[bold]Incompatible embed![/] This preset is not supported by the current engine version and will not be loaded!");
                }
                return supported;
            }).ToDictionary();

            LogConsole($"[bold darkblue]Queuing mod build for {presets.Count} presets[/]");
            var nameIdx = 0;

            string GetOutputName(int index) {
                var nameRoot = string.IsNullOrWhiteSpace(settings.OutputFileName)
                    ? "SicarioPresetMerge"
                    : settings.OutputFileName;
                return presets.Count > 1
                    ? $"{nameRoot}_{index}"
                    : nameRoot;
            }
            foreach (var (fileName, preset) in presets) {
                var modList = preset.Mods;
                var inputs = preset.ModParameters;
                var req = new PatchRequest(modList) {
                    PackResult = true,
                    TemplateInputs = inputs,
                    AdditionalFiles = new Dictionary<string, FileInfo>() {
                        ["Content/sicario"] = new(fileName)
                    },
                    Name = GetOutputName(nameIdx),
                    UserName = $"pack:{Path.GetFileNameWithoutExtension(fileName)}"
                };
                nameIdx++;
                var res = await _mediator.Send(req);
                var targetPakPath = Path.Combine(pakOutputPath, res.Name);
                res.MoveTo(targetPakPath, true);
                LogConsole($"'{Path.GetFileNameWithoutExtension(fileName)}' built to '{targetPakPath}'");
            }

            return 0;
        }
    }
}