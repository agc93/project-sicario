using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using BuildEngine;
using BuildEngine.Scripts;
using HexPatch;
using HexPatch.Build;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SicarioPatch.App.Infrastructure;
using SicarioPatch.App.Shared;
using SicarioPatch.Core;

namespace SicarioPatch.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(
                mc => mc.AsScoped(),
                typeof(Startup), typeof(PatchRequest));
            // services.AddSingleton<IPipelineBehavior<ModUploadRequest, string>, ModIndexHandler>();
            services.AddLogging();
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
            
            services
                .AddSingleton<WingmanPatchServiceBuilder>()
                .AddSingleton<SourceFileService>()
                .AddSingleton<FilePatcher>()
                .AddSingleton<ModFileLoader>()
                .AddSingleton<BuildContextFactory>()
                .AddSingleton<IAppInfoProvider, AppInfoProvider>()
                .AddSingleton<AppInfoProvider>()
                .AddSingleton<IScriptService, PythonScriptDownloadService>()
                ;
            services.AddAuthentication(opts =>
                {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddDiscord(Configuration.GetSection("Discord"));
            services
                .AddSingleton<BrandProvider>()
                .AddSingleton<ModParser>();
            services.AddBlazorise(opts =>
                {
                    opts.ChangeTextOnKeyPress = true;
                })
                .AddMaterialProviders()
                .AddMaterialIcons();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.ApplicationServices.UseMaterialProviders().UseMaterialIcons();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
