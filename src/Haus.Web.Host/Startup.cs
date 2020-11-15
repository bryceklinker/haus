using Haus.Hosting;
using Haus.Web.Host.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host
{
    public class Startup
    {
        private const string ClientAppRoot = "./client-app";
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSpaStaticFiles(spa => spa.RootPath = ClientAppRoot);
            services.AddControllers();
            services.Configure<AuthOptions>(_configuration.GetSection("Auth"));
            services.AddCors(opts => opts.AddDefaultPolicy(
                b => b.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin())
            );
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseHausRequestLogging()
                .UseCors()
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSpaStaticFiles();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = ClientAppRoot;
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer("start");
                }
            });
        }
    }
}