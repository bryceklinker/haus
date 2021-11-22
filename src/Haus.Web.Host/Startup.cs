using System.IO;
using Haus.Hosting;
using Haus.Web.Host.Auth;
using Haus.Web.Host.Common.Events;
using Haus.Web.Host.Common.Mqtt;
using Haus.Web.Host.Common.SignalR;
using Haus.Web.Host.DeviceSimulator;
using Haus.Web.Host.Diagnostics;
using Haus.Web.Host.Health;
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
                .AddHausLogging()
                .AddHausWebHost(_configuration)
                .AddHausAuthentication(_configuration)
                .AddHausRestApi()
                .AddHausRealtimeApi()
                .AddHausSpa(Path.Combine(ClientAppRoot, "dist"))
                .AddHausHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHausRequestLogging()
                .UseHttpsRedirection()
                .UseCors()
                .UseStaticFiles()
                .UseSpaStaticFiles();

            app.UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<DiagnosticsHub>(HubPatterns.DiagnosticsHub);
                    endpoints.MapHub<DeviceSimulatorHub>(HubPatterns.DeviceSimulatorHub);
                    endpoints.MapHub<EventsHub>(HubPatterns.EventsHub);
                    endpoints.MapHub<HealthHub>(HubPatterns.HealthHub);
                    endpoints.MapControllers();
                });
            
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = ClientAppRoot;
                if (env.IsDevelopment()) spa.UseAngularCliServer("start");
            });
        }
    }
}