using System;
using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.Extensions.DependencyInjection;

namespace SicarioPatch.Components
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddComponents(this IServiceCollection services, Action<BlazoriseOptions> config) {
            services.AddBlazorise(opts =>
                {
                    config?.Invoke(opts);
                    opts.DelayTextOnKeyPress = true;
                    opts.ChangeTextOnKeyPress = true;
                })
                .AddMaterialProviders()
                .AddMaterialIcons();
            return services;
        }

        public static IServiceCollection AddComponents(this IServiceCollection services) {
            return services.AddComponents(null);
        }
    }
}