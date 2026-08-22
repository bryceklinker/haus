using System;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausZigbee(this IServiceCollection services)
    {
        // ZigbeeCoordinator calls this factory again every time it rebuilds after a fatal
        // transport failure, so reconnecting always gets a fresh instance instead of the same
        // broken one. Registering only the factory (not ISerialTransport itself) also keeps
        // ZigbeeCoordinator's constructor unambiguous for the DI container.
        return services
            .AddSingleton<Func<ISerialTransport>>(provider => () => CreateTransport(provider))
            .AddSingleton<IZigbeeCoordinator, ZigbeeCoordinator>();
    }

    private static ISerialTransport CreateTransport(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ZigbeeConnectionOptions>>().Value;
        return string.IsNullOrEmpty(options.TcpHost)
            ? new SerialPortTransport(options.SerialPort, options.BaudRate)
            : new TcpSerialTransport(options.TcpHost, options.TcpPort);
    }
}
