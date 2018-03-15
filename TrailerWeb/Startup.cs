using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ITOps.Composition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrailerWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            RegisterDataProviders(services.AddMvc());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        private void RegisterDataProviders(IMvcBuilder builder)
        {
            builder.Services.AddSingleton<RequestComposer>();

            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, "*.dll");
            foreach (var assemblyFilename in assemblies)
            {
                var dataProviders = Assembly.LoadFrom(assemblyFilename).GetTypes()
                    .Where(t =>
                    {
                        var typeInfo = t.GetTypeInfo();

                        return !typeInfo.IsInterface && typeof(IProvideData).IsAssignableFrom(t);
                    });

                foreach (var provider in dataProviders)
                {
                    builder.Services.Add(new ServiceDescriptor(typeof(IProvideData), provider, ServiceLifetime.Transient));
                }
            }
        }
    }
}
