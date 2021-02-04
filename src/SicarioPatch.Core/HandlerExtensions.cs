using System.IO;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SicarioPatch.Core
{
    public static class HandlerExtensions
    {
        public static IServiceCollection AddBehaviours(this IServiceCollection services)
        {
            // services.AddScoped<WingmanModLoader>();
            return services
                .AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, PatchTemplateBehaviour>()
                .AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, FileRenameBehaviour>();
        }
    }
}