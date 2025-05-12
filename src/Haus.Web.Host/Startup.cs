using System.IO;
using Haus.Core.Models;
using Haus.Hosting;
using Haus.Web.Host.Common.Events;
using Haus.Web.Host.Common.SignalR;
using Haus.Web.Host.DeviceSimulator;
using Haus.Web.Host.Diagnostics;
using Haus.Web.Host.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host;

public class Startup
{
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
            .AddHausHealthChecks();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseHausRequestLogging().UseCors();
        if (!env.IsAcceptance())
            app.UseHttpsRedirection();

        app.UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DiagnosticsHub>(HausRealtimeSources.Diagnostics);
                endpoints.MapHub<DeviceSimulatorHub>(HausRealtimeSources.DeviceSimulator);
                endpoints.MapHub<EventsHub>(HausRealtimeSources.Events);
                endpoints.MapHub<HealthHub>(HausRealtimeSources.Health);
                endpoints.MapControllers();
            });
    }
}
