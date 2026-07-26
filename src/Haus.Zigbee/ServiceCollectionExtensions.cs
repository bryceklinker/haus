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
        return services
            .AddSingleton<ISerialTransport>(CreateSerialPortTransport)
            .AddSingleton<IZigbeeCoordinator, ZigbeeCoordinator>();
    }

    private static SerialPortTransport CreateSerialPortTransport(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ZigbeeConnectionOptions>>().Value;
        return new SerialPortTransport(options.SerialPort, options.BaudRate);
    }
}
