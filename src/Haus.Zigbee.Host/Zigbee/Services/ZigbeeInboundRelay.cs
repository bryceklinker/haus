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
    IHausMqttClient hausMqttClient,
    IOptions<HausOptions> hausOptions
)
{
    public Task HandleDeviceJoinedAsync(Haus.Zigbee.ZigbeeDeviceJoined joined)
    {
        addressRegistry.Register(joined.NetworkAddress, ExternalIdMap.ToExternalId(joined.IeeeAddress));
        var discovered = deviceJoinedMapper.Map(joined);
        return hausMqttClient.PublishHausEventAsync(discovered);
    }

    public Task HandleAttributeReportedAsync(Haus.Zigbee.ZigbeeAttributeReport report)
    {
        var hausEvent = deviceEventMapper.Map(report);
        return hausEvent == null
            ? Task.CompletedTask
            : hausMqttClient.PublishAsync(hausOptions.GetEventsTopic(), hausEvent);
    }
}
