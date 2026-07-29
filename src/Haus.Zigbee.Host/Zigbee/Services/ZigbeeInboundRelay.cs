using System.Threading.Tasks;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee.Services;

public class ZigbeeInboundRelay(
    DeviceAddressRegistry addressRegistry,
    DeviceJoinedMapper deviceJoinedMapper,
    DeviceEventMapper deviceEventMapper,
    IHausMqttClientFactory mqttClientFactory,
    IOptions<HausOptions> hausOptions
)
{
    public async Task HandleDeviceJoinedAsync(ZigbeeDeviceJoined joined)
    {
        addressRegistry.Register(joined.NetworkAddress, ExternalIdMap.ToExternalId(joined.IeeeAddress));
        var discovered = deviceJoinedMapper.Map(joined);
        var mqttClient = await mqttClientFactory.CreateClient();
        await mqttClient.PublishHausEventAsync(discovered);
    }

    public async Task HandleAttributeReportedAsync(ZigbeeAttributeReport report)
    {
        var hausEvent = deviceEventMapper.Map(report);
        if (hausEvent == null)
            return;

        var mqttClient = await mqttClientFactory.CreateClient();
        await mqttClient.PublishAsync(hausOptions.GetEventsTopic(), hausEvent);
    }
}
