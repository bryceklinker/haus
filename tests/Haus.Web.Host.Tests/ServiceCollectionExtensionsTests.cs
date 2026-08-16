using System.Collections.Generic;
using System.Threading.Tasks;
using Haus.Web.Host.DeviceSimulator;
using Haus.Web.Host.Tests.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Haus.Web.Host.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public async Task WhenDeviceSimulatorIsEnabledThenStatePublisherHostedServiceIsRegistered()
    {
        await using var provider = BuildProvider("Development");

        var hostedServices = provider.GetServices<IHostedService>();

        Assert.Contains(hostedServices, service => service is DeviceSimulatorStatePublisher);
    }

    [Fact]
    public async Task WhenDeviceSimulatorIsDisabledThenStatePublisherHostedServiceIsNotRegistered()
    {
        await using var provider = BuildProvider("Production");

        var hostedServices = provider.GetServices<IHostedService>();

        Assert.DoesNotContain(hostedServices, service => service is DeviceSimulatorStatePublisher);
    }

    private static ServiceProvider BuildProvider(string environmentName)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["Database:ConnectionString"] = "Data Source=:memory:" }
            )
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHausWebHost(configuration, new StubHostEnvironment(environmentName));
        return services.BuildServiceProvider();
    }
}
