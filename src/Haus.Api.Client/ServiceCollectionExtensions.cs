using System;
using System.Net.Http;
using Haus.Api.Client.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Api.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddHausApiClient(
        this IServiceCollection services,
        Action<HausApiClientSettings> configureSettings
    )
    {
        services.AddOptions<HausApiClientSettings>().Configure(configureSettings);
        services
            .AddTransient<IHausApiClientFactory, HausApiClientFactory>()
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateDeviceClient())
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateDiagnosticsClient())
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateRoomsClient())
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateDeviceSimulatorClient())
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateApplicationClient())
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().Create());
        return services.AddHttpClient(HausApiClientNames.Default);
    }
}
