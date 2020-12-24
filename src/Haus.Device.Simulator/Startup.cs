using Haus.Device.Simulator.Devices;
using Haus.Device.Simulator.Devices.Services;
using Haus.Hosting;
using Haus.Mqtt.Client;
using Haus.Mqtt.Client.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Device.Simulator
{
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
                .AddSingleton<IDevicesStore, DevicesStore>()
                .AddTransient<IDevicesService, DevicesService>()
                .AddTransient<IDevicesHubService, DevicesHubService>()
                .AddHostedService<RealtimeDevicesService>()
                .AddHausMqtt()
                .Configure<HausMqttSettings>(_configuration.GetSection("Mqtt"))
                .AddControllers();
            services.AddSignalR(opts =>
            {
                opts.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHausRequestLogging()
                .UseCors()
                .UseHttpsRedirection();

            app.UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<DevicesHub>("/hubs/devices");
                    endpoints.MapControllers();
                });
        }
    }
}