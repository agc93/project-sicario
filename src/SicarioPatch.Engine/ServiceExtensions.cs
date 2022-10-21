using Microsoft.Extensions.DependencyInjection;
using SicarioPatch.Engine;
using SicarioPatch.Engine.Patches;
using SicarioPatch.Engine.Templates;
using SicarioPatch.Engine.TypeLoaders;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddTemplates(this IServiceCollection services) {
            services.AddSingleton<ITemplateProvider, ArrayTemplateProvider>();
            services.AddSingleton<ITemplateProvider, EnumTemplateProvider>();
            services.AddSingleton<ITemplateProvider, PropertyValueTemplateProvider>();
            services.AddSingleton<ITemplateProvider, StructTemplateProvider>();
            return services;
        }

        public static IServiceCollection AddPatchTypes(this IServiceCollection services) {
            services.AddSingleton<IAssetPatchType, PropertyValuePatchType>();
            services.AddSingleton<IAssetPatchType, ArrayPropertyPatchType>();
            services.AddSingleton<IAssetPatchType, ModifyPropertyValuePatchType>();
            services.AddSingleton<IAssetPatchType, DuplicatePropertyPatchType>();
            services.AddSingleton<IAssetPatchType, DuplicateItemPatchType>();
            services.AddSingleton<IAssetPatchType, TextPropertyValuePatchType>();
            services.AddSingleton<IAssetPatchType, ObjectRefPatchType>();
            services.AddSingleton<IAssetPatchType, DuplicateArrayItemPatchType>();
            return services;
        }

        public static IServiceCollection AddTypeLoaders(this IServiceCollection services) {
            services.AddSingleton<IAssetTypeLoader, DataTableTypeLoader>();
            services.AddSingleton<IAssetTypeLoader, RawTypeLoader>();
            return services;
        }

        public static IServiceCollection AddAssetServices(this IServiceCollection services) {
            services.AddSingleton<AssetPatcher>();
            // services.AddSingleton<EngineInfoProvider>();
            services.AddSingleton<IEngineInfoProvider, EngineInfoProvider>();
            services.AddTypeLoaders().AddPatchTypes().AddTemplates();
            return services;
        }
    }
}