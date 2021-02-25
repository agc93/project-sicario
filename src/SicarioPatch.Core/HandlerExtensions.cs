using System;
using System.IO;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace SicarioPatch.Core
{
    public static class HandlerExtensions
    {
        public static IServiceCollection AddBehaviours(this IServiceCollection services)
        {
            return services
                .AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, FileRenameBehaviour>()
                .AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, BuildLogBehaviour>();
        }

        public static IServiceCollection AddPatchBehaviour<T>(this IServiceCollection services) where T : class, IPipelineBehavior<PatchRequest, FileInfo>
        {
            return services.AddSingleton<IPipelineBehavior<PatchRequest, FileInfo>, T>();
        }
    }
}