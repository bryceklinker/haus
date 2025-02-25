using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Tests.Support;

public static class OptionsFactory
{
    public static IOptions<ZigbeeOptions> CreateZigbeeOptions(string baseTopic = Defaults.ZigbeeOptions.BaseTopic)
    {
        return Options.Create(
            new ZigbeeOptions
            {
                Config = new Zigbee2MqttConfiguration { Mqtt = new MqttConfiguration { BaseTopic = baseTopic } },
            }
        );
    }

    public static IOptions<HausOptions> CreateHausOptions(
        string eventsTopic = ConfigurationFactory.DefaultHausEventsTopic,
        string unknownEventTopic = ConfigurationFactory.DefaultHausUnknownTopic,
        string commandsTopic = ConfigurationFactory.DefaultHausCommandsTopic
    )
    {
        return Options.Create(
            new HausOptions
            {
                CommandsTopic = commandsTopic,
                EventsTopic = eventsTopic,
                UnknownTopic = unknownEventTopic,
            }
        );
    }

    public static IOptionsMonitor<HausOptions> CreateHausOptionsMonitor(
        string eventsTopic = ConfigurationFactory.DefaultHausEventsTopic,
        string unknownEventTopic = ConfigurationFactory.DefaultHausUnknownTopic,
        string commandsTopic = ConfigurationFactory.DefaultHausCommandsTopic
    )
    {
        return new OptionsMonitorFake<HausOptions>(
            new HausOptions
            {
                CommandsTopic = commandsTopic,
                EventsTopic = eventsTopic,
                UnknownTopic = unknownEventTopic,
            }
        );
    }
}
