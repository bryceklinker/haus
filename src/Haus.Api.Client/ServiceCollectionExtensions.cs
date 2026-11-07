using System;
using System.Net.Http;
using Haus.Api.Client.Options;
using Haus.Api.Client.Zigbee;
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
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().CreateLogsClient())
            .AddTransient(p => p.GetRequiredService<IHausApiClientFactory>().Create());
        return services.AddHttpClient(HausApiClientNames.Default);
    }

    public static IHttpClientBuilder AddZigbeeHostApiClient(
        this IServiceCollection services,
        Action<ZigbeeHostApiClientSettings> configureSettings
    )
    {
        services.AddOptions<ZigbeeHostApiClientSettings>().Configure(configureSettings);
        services
            .AddTransient<IZigbeeHostApiClientFactory, ZigbeeHostApiClientFactory>()
            .AddTransient(p => p.GetRequiredService<IZigbeeHostApiClientFactory>().Create());
        return services.AddHttpClient(ZigbeeHostApiClientNames.Default);
    }
}
