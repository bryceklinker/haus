using System.Threading.Tasks;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee2Mqtt.Configuration;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee2Mqtt.Mqtt;

public interface IZigbeeMqttClientFactory
{
    Task<IHausMqttClient> CreateZigbeeClient();
    Task<IHausMqttClient> CreateHausClient();
}

public class ZigbeeMqttClientFactory(
    IOptions<ZigbeeOptions> zigbeeOptions,
    IOptions<HausOptions> hausOptions,
    IHausMqttClientFactory mqttFactory
) : IZigbeeMqttClientFactory
{
    private string ZigbeeMqttUrl => zigbeeOptions.Value.Config.Mqtt.Server;
    private string HausMqttUrl => hausOptions.Value.Server;

    public Task<IHausMqttClient> CreateZigbeeClient()
    {
        return mqttFactory.CreateClient(ZigbeeMqttUrl);
    }

    public Task<IHausMqttClient> CreateHausClient()
    {
        return mqttFactory.CreateClient(HausMqttUrl);
    }
}
