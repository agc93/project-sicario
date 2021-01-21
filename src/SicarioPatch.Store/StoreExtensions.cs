using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SicarioPatch.Store
{
    public static class StoreExtensions
    {
        public static IServiceCollection AddIndex(this IServiceCollection services, IConfigurationSection config)
        {
            services.AddSingleton<StoreOptions>(provider => {
                var config = provider.GetService<IConfiguration>();
                return config.Get<StoreOptions>();
            });
            services.AddSingleton<IFileIndexService, LocalFileIndexService>();
        }
        public static IServiceCollection AddStoreServices(this IServiceCollection services, IConfiguration config)
        {
            
            services.AddFileStore(c =>
            {
                c.AddLocalIndex().AddLocalStorage().ConfigureOptions(config.GetSection("Store"));
            });
            services.AddSingleton<StoreModLoader>();
            return services;
        }
    }
}