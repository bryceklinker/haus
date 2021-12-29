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

public class ZigbeeMqttClientFactory : IZigbeeMqttClientFactory
{
    private readonly IOptions<ZigbeeOptions> _zigbeeOptions;
    private readonly IOptions<HausOptions> _hausOptions;
    private readonly IHausMqttClientFactory _mqttFactory;

    private string ZigbeeMqttUrl => _zigbeeOptions.Value.Config.Mqtt.Server;
    private string HausMqttUrl => _hausOptions.Value.Server;

    public ZigbeeMqttClientFactory(IOptions<ZigbeeOptions> zigbeeOptions, IOptions<HausOptions> hausOptions,
        IHausMqttClientFactory mqttFactory)
    {
        _zigbeeOptions = zigbeeOptions;
        _hausOptions = hausOptions;
        _mqttFactory = mqttFactory;
    }

    public Task<IHausMqttClient> CreateZigbeeClient()
    {
        return _mqttFactory.CreateClient(ZigbeeMqttUrl);
    }

    public Task<IHausMqttClient> CreateHausClient()
    {
        return _mqttFactory.CreateClient(HausMqttUrl);
    }
}