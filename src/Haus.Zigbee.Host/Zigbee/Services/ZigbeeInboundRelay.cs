using System.Threading.Tasks;
using Haus.Mqtt.Client;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee.Services;

public class ZigbeeInboundRelay(
    DeviceAddressRegistry addressRegistry,
    DeviceJoinedMapper deviceJoinedMapper,
    DeviceEventMapper deviceEventMapper,
    IHausMqttClientFactory mqttClientFactory,
    IOptions<HausOptions> hausOptions,
    ILogger<ZigbeeInboundRelay> logger
)
{
    public async Task HandleDeviceJoinedAsync(ZigbeeDeviceJoined joined)
    {
        logger.LogInformation(
            "Device joined: {@IeeeAddress} ({@Manufacturer} {@Model})",
            joined.IeeeAddress,
            joined.ManufacturerName,
            joined.ModelIdentifier
        );
        addressRegistry.Register(joined.NetworkAddress, ExternalIdMap.ToExternalId(joined.IeeeAddress));
        var discovered = deviceJoinedMapper.Map(joined);
        var mqttClient = await mqttClientFactory.CreateClient();
        await mqttClient.PublishHausEventAsync(discovered);
    }

    public async Task HandleAttributeReportedAsync(ZigbeeAttributeReport report)
    {
        var hausEvent = deviceEventMapper.Map(report);
        if (hausEvent == null)
        {
            logger.LogInformation(
                "Ignoring attribute report from {@NetworkAddress}: cluster {@ClusterId}, attribute {@AttributeId}",
                report.SourceNwkAddress,
                report.ClusterId,
                report.AttributeId
            );
            return;
        }

        logger.LogInformation(
            "Attribute report from {@NetworkAddress}: publishing {@Type}",
            report.SourceNwkAddress,
            hausEvent.Type
        );
        var mqttClient = await mqttClientFactory.CreateClient();
        await mqttClient.PublishAsync(hausOptions.GetEventsTopic(), hausEvent);
    }
}
