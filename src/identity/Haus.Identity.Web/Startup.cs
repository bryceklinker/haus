using Haus.Identity.Web.Common.Storage;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Identity.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private string ConnectionString => Configuration["DB_CONNECTION_STRING"];

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHausIdentityWeb(ConnectionString);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.SeedDatabaseAsync().Wait();

            app.UseHttpsRedirection()
                .UseRouting()
                .UseIdentityServer()
                .UseAuthorization()
                .UseEndpoints(endPoints => { endPoints.MapControllers(); })
                .UseStaticFiles()
                .UseSpaStaticFiles();
            app.Use(async (context, next) =>
                {
                    if (!context.User.IsAuthenticated())
                    {
                        await context.ChallengeAsync();
                    }
                    else
                    {
                        await next();
                    }
                })
                .UseSpa(spa =>
                {
                    spa.Options.SourcePath = "client-app";
                    if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
                });
        }
    }
}