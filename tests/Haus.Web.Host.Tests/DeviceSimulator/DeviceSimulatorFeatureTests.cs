using System.Collections.Generic;
using Haus.Web.Host.DeviceSimulator;
using Haus.Web.Host.Tests.Support;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Haus.Web.Host.Tests.DeviceSimulator;

public class DeviceSimulatorFeatureTests
{
    [Theory]
    [InlineData("Development")]
    [InlineData("Acceptance")]
    public void WhenNoExplicitValueConfiguredAndEnvironmentDefaultsToEnabledThenIsEnabled(string environmentName)
    {
        var isEnabled = DeviceSimulatorFeature.IsEnabled(BuildConfiguration(), BuildEnvironment(environmentName));

        Assert.True(isEnabled);
    }

    [Fact]
    public void WhenNoExplicitValueConfiguredAndEnvironmentIsProductionThenIsDisabled()
    {
        var isEnabled = DeviceSimulatorFeature.IsEnabled(BuildConfiguration(), BuildEnvironment("Production"));

        Assert.False(isEnabled);
    }

    [Fact]
    public void WhenExplicitlyEnabledInConfigurationThenOverridesProductionDefault()
    {
        var configuration = BuildConfiguration(
            new Dictionary<string, string?> { ["DeviceSimulator:Enabled"] = "true" }
        );

        var isEnabled = DeviceSimulatorFeature.IsEnabled(configuration, BuildEnvironment("Production"));

        Assert.True(isEnabled);
    }

    [Fact]
    public void WhenExplicitlyDisabledInConfigurationThenOverridesDevelopmentDefault()
    {
        var configuration = BuildConfiguration(
            new Dictionary<string, string?> { ["DeviceSimulator:Enabled"] = "false" }
        );

        var isEnabled = DeviceSimulatorFeature.IsEnabled(configuration, BuildEnvironment("Development"));

        Assert.False(isEnabled);
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?>? values = null)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(values ?? []).Build();
    }

    private static IHostEnvironment BuildEnvironment(string environmentName)
    {
        return new StubHostEnvironment(environmentName);
    }
}
