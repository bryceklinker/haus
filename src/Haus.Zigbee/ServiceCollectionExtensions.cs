using System;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHausZigbee(this IServiceCollection services)
    {
        return services
            .AddSingleton<Func<ISerialTransport>>(provider => () => CreateTransport(provider))
            .AddSingleton<IZigbeeCoordinator>(provider =>
            {
                var transportFactory = provider.GetRequiredService<Func<ISerialTransport>>();
                var loggerFactory = provider.GetService<ILoggerFactory>();
                var retryOptions = provider.GetService<IOptions<CommandRetryOptions>>()?.Value;
                return new ZigbeeCoordinator(transportFactory, loggerFactory, retryOptions: retryOptions);
            });
    }

    private static ISerialTransport CreateTransport(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<ZigbeeConnectionOptions>>().Value;
        var loggerFactory = provider.GetService<ILoggerFactory>();
        return string.IsNullOrEmpty(options.TcpHost)
            ? new SerialPortTransport(
                options.SerialPort,
                options.BaudRate,
                loggerFactory?.CreateLogger<SerialPortTransport>()
            )
            : new TcpSerialTransport(
                options.TcpHost,
                options.TcpPort,
                loggerFactory?.CreateLogger<TcpSerialTransport>()
            );
    }
}
