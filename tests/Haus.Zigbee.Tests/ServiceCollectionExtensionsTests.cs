using Haus.Zigbee.Coordinator;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void WhenAddingHausZigbeeThenTheCoordinatorCanBeResolved()
    {
        using var provider = BuildProvider();

        var coordinator = provider.GetRequiredService<IZigbeeCoordinator>();

        Assert.IsType<ZigbeeCoordinator>(coordinator);
    }

    [Fact]
    public void WhenResolvingTheCoordinatorTwiceThenItIsTheSameSingletonInstance()
    {
        using var provider = BuildProvider();

        var first = provider.GetRequiredService<IZigbeeCoordinator>();
        var second = provider.GetRequiredService<IZigbeeCoordinator>();

        Assert.Same(first, second);
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.Configure<ZigbeeConnectionOptions>(options =>
        {
            options.SerialPort = "/dev/ttyACM0";
            options.BaudRate = 38400;
        });
        services.AddHausZigbee();
        return services.BuildServiceProvider();
    }
}
