using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Haus.Zigbee.Host.Tests.Support;

public static class ConfigurationFactory
{
    public const string DefaultHausEventsTopic = Defaults.HausOptions.EventsTopic;
    public const string DefaultHausCommandsTopic = Defaults.HausOptions.CommandsTopic;
    public const string DefaultHausUnknownTopic = Defaults.HausOptions.UnknownTopic;
    public const string DefaultHausHealthTopic = Defaults.HausOptions.HealthTopic;
    public const string DefaultHausZigbeeTopic = Defaults.HausOptions.ZigbeeTopic;

    public static IConfiguration CreateConfig(
        string hausEventsTopic = DefaultHausEventsTopic,
        string hausCommandsTopic = DefaultHausCommandsTopic,
        string hausUnknownTopic = DefaultHausUnknownTopic,
        string hausHealthTopic = DefaultHausHealthTopic,
        string hausZigbeeTopic = DefaultHausZigbeeTopic,
        string? hausServer = null
    )
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new List<KeyValuePair<string, string?>>
                {
                    new("Zigbee:SerialPort", "/dev/ttyACM0"),
                    new("Haus:Server", hausServer ?? "mqtt://localhost:1883"),
                    new("Haus:EventsTopic", hausEventsTopic),
                    new("Haus:CommandsTopic", hausCommandsTopic),
                    new("Haus:UnknownTopic", hausUnknownTopic),
                    new("Haus:Health", hausHealthTopic),
                    new("Haus:ZigbeeTopic", hausZigbeeTopic),
                }.AsReadOnly()
            )
            // Lets CI's disposable-broker port (exported as Haus__Server, see Makefile's test-unit
            // target) override the in-memory default below, the same way Haus.Web.Host.Tests picks
            // up Mqtt__Server through its WebApplicationFactory's standard configuration layering.
            .AddEnvironmentVariables()
            .Build();
    }
}
