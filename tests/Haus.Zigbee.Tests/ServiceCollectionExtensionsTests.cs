using System;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Transport;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void WhenAddingHausZigbeeThenTheCoordinatorCanBeResolved()
    {
        using var provider = BuildProvider(options =>
        {
            options.SerialPort = "/dev/ttyACM0";
            options.BaudRate = 38400;
        });

        var coordinator = provider.GetRequiredService<IZigbeeCoordinator>();

        Assert.IsType<ZigbeeCoordinator>(coordinator);
    }

    [Fact]
    public void WhenResolvingTheCoordinatorTwiceThenItIsTheSameSingletonInstance()
    {
        using var provider = BuildProvider(options => options.SerialPort = "/dev/ttyACM0");

        var first = provider.GetRequiredService<IZigbeeCoordinator>();
        var second = provider.GetRequiredService<IZigbeeCoordinator>();

        Assert.Same(first, second);
    }

    [Fact]
    public void WhenTcpHostIsNotConfiguredThenTheSerialPortTransportIsUsed()
    {
        using var provider = BuildProvider(options => options.SerialPort = "/dev/ttyACM0");

        var transport = provider.GetRequiredService<ISerialTransport>();

        Assert.IsType<SerialPortTransport>(transport);
    }

    [Fact]
    public void WhenTcpHostIsConfiguredThenTheTcpSerialTransportIsUsed()
    {
        using var provider = BuildProvider(options =>
        {
            options.TcpHost = "fake-deconz";
            options.TcpPort = 4901;
        });

        var transport = provider.GetRequiredService<ISerialTransport>();

        Assert.IsType<TcpSerialTransport>(transport);
    }

    private static ServiceProvider BuildProvider(Action<ZigbeeConnectionOptions> configure)
    {
        var services = new ServiceCollection();
        services.Configure(configure);
        services.AddHausZigbee();
        return services.BuildServiceProvider();
    }
}
