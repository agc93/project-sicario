using System;
using Microsoft.Extensions.DependencyInjection;
using SicarioPatch.Engine.Fragments;
using Xunit.DependencyInjection.Logging;

namespace SicarioPatch.Engine.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) {
            services.Scan(scan =>
                scan.FromAssemblyOf<IAssetParserFragment>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IAssetParserFragment))).AsImplementedInterfaces().WithSingletonLifetime()
                    .AddClasses(classes => classes.AssignableTo(typeof(ITemplateProvider))).AsImplementedInterfaces().WithSingletonLifetime()
                    .AddClasses(cls => cls.AssignableTo<IAssetTypeLoader>()).AsImplementedInterfaces().WithSingletonLifetime()
                    .AddClasses(cls => cls.AssignableTo<IAssetPatchType>()).AsImplementedInterfaces().WithSingletonLifetime()
                
            ).AddTemplates().AddTypeLoaders();
        }

        public void Configure(IServiceProvider provider) {
            XunitTestOutputLoggerProvider.Register(provider);
        }
    }
}