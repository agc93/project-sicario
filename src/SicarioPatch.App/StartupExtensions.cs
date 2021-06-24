using System.Collections.Generic;
using HexPatch.Build;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SicarioPatch.App.Infrastructure;
using SicarioPatch.App.Shared;
using SicarioPatch.Assets;
using SicarioPatch.Assets.Fragments;
using SicarioPatch.Assets.TypeLoaders;
using SicarioPatch.Core;
using SicarioPatch.Templating;

namespace SicarioPatch.App
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddConfigOptions(this IServiceCollection services)
        {
            services.AddSingleton<SourceFileOptions>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                return config.GetSection("Files").Get<SourceFileOptions>();
            });
            services.AddSingleton<ModLoadOptions>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                return config.GetSection("Mods").Get<ModLoadOptions>();
            });
            return services;
        }

        public static IServiceCollection AddBrandProvider(this IServiceCollection services, string sectionName = "AppDetails")
        {
            services.AddSingleton<BrandProvider>(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                if (config.GetSection(sectionName) is var details && details.Exists())
                {
                    return details.Get<BrandProvider>();
                }

                return new BrandProvider();
            });
            return services;
        }

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, IConfigurationSection config)
        {
            return builder.AddDiscord(opts =>
            {
                opts.ClientId = config.GetValue<string>("ClientId");
                opts.ClientSecret = config.GetValue<string>("ClientSecret");
                foreach (var scope in config.GetValue<List<string>>("Scopes", new List<string> {"identify", "email"}))
                {
                    opts.Scope.Add(scope);
                }
            });
        }

        public static IServiceCollection AddAuthHandlers(this IServiceCollection services, IConfigurationSection config)
        {
            services.AddAuthorization(opts => opts.UseUserPolicies(config));
            return services.AddSingleton<IAuthorizationHandler, UserAccessHandler>()
                .AddSingleton<IAuthorizationHandler, UploadAccessHandler>();
        }

        public static AuthorizationOptions UseUserPolicies(this AuthorizationOptions opts, IConfigurationSection config)
        {
            var conf = config.Get<AccessOptions>();
            opts.AddPolicy(Policies.IsUser, policy => policy.Requirements.Add(new UserRequirement(conf)));
            opts.AddPolicy(Policies.IsUploader, policy => policy.Requirements.Add(new UploaderRequirement(conf)));
            return opts;
        }

        public static IServiceCollection AddConfigTemplating(this IServiceCollection services)
        {
            // services.AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, PatchTemplateBehaviour>()
            services.AddSingleton<ITemplateModelProvider, ConfigModelProvider>();
            services.AddSingleton<ITemplateModelProvider, IniModelProvider>();
            return services;
        }

        public static IServiceCollection AddAssetServices(this IServiceCollection services) {
            services.AddSingleton<AssetPatcher>();
            services.AddSingleton<IAssetPatchType, PropertyValuePatchType>();
            services.AddSingleton<IAssetPatchType, ArrayPropertyPatchType>();
            services.AddSingleton<IAssetPatchType, DuplicatePropertyPatchType>();
            services.AddSingleton<IAssetPatchType, DuplicateItemPatchType>();
            services.AddSingleton<IAssetTypeLoader, DataTableTypeLoader>();
            services.AddSingleton<IAssetParserFragment, StructPropertyFragment>();
            services.AddSingleton<IAssetParserFragment, ArrayPropertyFragment>();
            services.AddSingleton<IAssetParserFragment, ArrayFragment>();
            services.AddSingleton<IAssetParserFragment, StructFragment>();
            return services;
        }
    }
}