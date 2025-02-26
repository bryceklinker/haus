using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Haus.Zigbee.Host.Tests.Support;

public static class ConfigurationFactory
{
    public const string DefaultZigbeeBaseTopic = Defaults.ZigbeeOptions.BaseTopic;
    public const string DefaultHausEventsTopic = Defaults.HausOptions.EventsTopic;
    public const string DefaultHausCommandsTopic = Defaults.HausOptions.CommandsTopic;
    public const string DefaultHausUnknownTopic = Defaults.HausOptions.UnknownTopic;
    public const string DefaultHausHealthTopic = Defaults.HausOptions.HealthTopic;

    public static IConfiguration CreateConfig(
        string zigbeeBaseTopic = DefaultZigbeeBaseTopic,
        string hausEventsTopic = DefaultHausEventsTopic,
        string hausCommandsTopic = DefaultHausCommandsTopic,
        string hausUnknownTopic = DefaultHausUnknownTopic,
        string hausHealthTopic = DefaultHausHealthTopic
    )
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new List<KeyValuePair<string, string?>>
                {
                    new("Zigbee:OverwriteConfig", "true"),
                    new("Zigbee:ConfigFile", "./zigbee_data/configuration.yaml"),
                    new("Zigbee:DataDirectory", "./zigbee_data"),
                    new("Zigbee:Config:Mqtt:BaseTopic", zigbeeBaseTopic),
                    new("Haus:Server", "mqtt://localhost:1883"),
                    new("Haus:EventsTopic", hausEventsTopic),
                    new("Haus:CommandsTopic", hausCommandsTopic),
                    new("Haus:UnknownTopic", hausUnknownTopic),
                    new("Haus:Health", hausHealthTopic),
                }.AsReadOnly()
            )
            .Build();
    }
}
