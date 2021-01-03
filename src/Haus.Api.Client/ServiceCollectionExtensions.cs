using System;
using Haus.Api.Client.Lighting;
using Haus.Api.Client.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Api.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHausApiClient(this IServiceCollection services, Action<HausApiClientSettings> configureSettings)
        {
            services.AddOptions<HausApiClientSettings>()
                .Configure(configureSettings);
            
            return services.AddHttpClient()
                .AddSingleton<IHausApiClientFactory, HausApiClientFactory>()
                .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateDeviceClient())
                .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateDiagnosticsClient())
                .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateRoomsClient())
                .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateDeviceSimulatorClient())
                .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateLightingApiClient())
                .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().Create());
        }
    }
}