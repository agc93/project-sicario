using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SicarioPatch.App
{
    public static class StartupExtensions
    {
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
    }
}