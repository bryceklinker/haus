using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.DeviceSimulator;

public static class DeviceSimulatorFeature
{
    public const string EnabledConfigurationKey = "DeviceSimulator:Enabled";

    public static bool IsEnabled(IConfiguration configuration, IHostEnvironment environment)
    {
        var explicitValue = configuration.GetValue<bool?>(EnabledConfigurationKey);
        return explicitValue ?? IsEnabledByDefault(environment);
    }

    private static bool IsEnabledByDefault(IHostEnvironment environment)
    {
        return environment.IsDevelopment() || environment.IsAcceptance();
    }
}
