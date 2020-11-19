using Haus.Hosting;
using Haus.Web.Host.Auth;
using Haus.Web.Host.Common.Mqtt;
using Haus.Web.Host.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.Client.Options;

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
            services
                .AddHausWebHost(_configuration)
                .AddAuthenticatedUserRequired(_configuration)
                .AddRestApi()
                .AddSignalR(opts => { opts.EnableDetailedErrors = true; });
            services.AddSpaStaticFiles(spa => spa.RootPath = ClientAppRoot);
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHausRequestLogging()
                .UseCors()
                .UseHttpsRedirection()
                .UseStaticFiles();

            app.UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<DiagnosticsHub>("/hubs/diagnostics");
                    endpoints.MapControllers();
                });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = ClientAppRoot;
                if (env.IsDevelopment()) spa.UseAngularCliServer("start");
            });

            app.UseSpaStaticFiles();
        }
    }
}