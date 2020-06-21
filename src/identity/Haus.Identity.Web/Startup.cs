using Haus.Identity.Core;
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.SeedDatabaseAsync().Wait();

            app.UseHttpsRedirection()
                .UseRouting()
                .UseIdentityServer()
                .UseAuthorization()
                .UseEndpoints(endPoints => { endPoints.MapControllers(); })
                .UseStaticFiles()
                .UseSpaStaticFiles();
            app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "client-app";
                    if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
                });
        }
    }
}