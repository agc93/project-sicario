using System.IO;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SicarioPatch.Core;

namespace SicarioPatch.Templating
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddTemplating(this IServiceCollection services)
        {
            services.AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, PatchTemplateBehaviour>();
            return services;
        }
    }
}