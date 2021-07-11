using System;
using BuildEngine;
using BuildEngine.Scripts;
using HexPatch;
using HexPatch.Build;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SicarioPatch.Core;
using SicarioPatch.Integration;
using SicarioPatch.Templating;
using Spectre.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using UnPak.Core;
using UnPak.Core.Crypto;

namespace SicarioPatch.Loader
{
    public static class Startup
    {
        internal static IServiceCollection GetServices() {
            var services = new ServiceCollection();
            services.AddSingleton<Integration.GameFinder>()
                .AddSingleton<IGameSource, ConfigurationGameSource>()
                .AddSingleton<SkinSlotLoader>()
                .AddSingleton<GameArchiveFileService>()
                .AddSingleton<PresetFileLoader>()
                .AddSingleton<MergeLoader>()
                .AddCoreServices()
                .AddUnPak()
                .AddMediatR(mc => mc.AsScoped(),
                    typeof(Startup), typeof(PatchRequest))
                .AddBehaviours()
                .AddTemplating()
                .AddConfiguration()
                .AddLogging();
            return services;
        }

        internal static CommandApp GetApp() {
            var app = new CommandApp(new DependencyInjectionRegistrar(GetServices()));
            app.SetDefaultCommand<BuildCommand>();
            app.Configure(c =>
            {
                c.AddCommand<BuildCommand>("build");
            });
            return app;
        }

        internal static IServiceCollection AddUnPak(this IServiceCollection services) {
            return services.AddSingleton<IPakFormat, PakVersion3Format>()
                .AddSingleton<IPakFormat, PakVersion8Format>()
                .AddSingleton<IFooterLayout, DefaultFooterLayout>()
                .AddSingleton<IFooterLayout, PaddedFooterLayout>()
                .AddSingleton<IHashProvider, NativeHashProvider>()
                .AddSingleton<PakFileProvider>();
        }

        internal static IServiceCollection AddConfiguration(this IServiceCollection services) {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
                
            return services.AddSingleton<IConfigurationRoot>(config).AddSingleton<IConfiguration>(config);
        }

        internal static IServiceCollection AddLogging(this IServiceCollection services) {
            return services.AddLogging(logging => {
                var level = GetLogLevel();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddInlineSpectreConsole(c => {
                    c.LogLevel = level;
                });
                // AddFileLogging(logging, level);
            });
        }

        internal static IServiceCollection AddCoreServices(this IServiceCollection services) {
            return services
                .AddSingleton<WingmanPatchServiceBuilder>()
                .AddSingleton<ISourceFileService, GameArchiveFileService>()
                .AddSingleton<ModParser>()
                .AddSingleton<FilePatcher>()
                // .AddSingleton<ModFileLoader<WingmanMod>>()
                // .AddSingleton<WingmanModLoader>()
                .AddSingleton<BuildContextFactory>()
                .AddSingleton<IAppInfoProvider, AppInfoProvider>()
                .AddSingleton<AppInfoProvider>()
                .AddSingleton<IScriptService, PythonScriptDownloadService>()
                .AddAssetServices();
        }
        
        internal static LogLevel GetLogLevel() {
            var envVar = System.Environment.GetEnvironmentVariable("SICARIO_DEBUG");
            if (System.IO.File.Exists(System.IO.Path.Combine(Environment.CurrentDirectory, "sicario-debug.txt"))) envVar = "trace";
            if (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory, "sicario-debug.txt"))) envVar = "trace";
            return string.IsNullOrWhiteSpace(envVar)
                ? LogLevel.Information
                :  envVar.ToLower() == "trace"
                    ? LogLevel.Trace
                    : LogLevel.Debug;
        }
    }
}