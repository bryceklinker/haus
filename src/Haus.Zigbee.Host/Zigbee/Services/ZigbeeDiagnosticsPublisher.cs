using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Zigbee.Services;

// Mirrors ZigbeeInboundRelay's shape but publishes raw Zigbee-network visibility onto the
// haus/zigbee topic, rather than translating Zigbee traffic into Haus domain events -- keeping
// diagnostics data separate from the domain-event handling ZigbeeInboundRelay owns.
public class ZigbeeDiagnosticsPublisher(
    DeviceAddressRegistry addressRegistry,
    IHausMqttClientFactory mqttClientFactory,
    IOptions<HausOptions> hausOptions
)
{
    public async Task HandleConnectionStatusChangedAsync(ZigbeeConnectionStatus status)
    {
        var networkConfig = status.NetworkConfig is null
            ? null
            : new ZigbeeNetworkConfigModel(
                status.NetworkConfig.MacAddress.ToString(),
                status.NetworkConfig.PanId,
                status.NetworkConfig.Channel
            );
        await PublishAsync(new ZigbeeConnectionStatusChangedEvent(status.IsConnected, networkConfig, status.Reason));
    }

    public async Task HandleDeviceJoinedAsync(ZigbeeDeviceJoined joined)
    {
        var ieeeAddress = ExternalIdConverter.ToExternalId(joined.IeeeAddress);
        await PublishAsync(new ZigbeeDeviceJoinedEvent(ieeeAddress, joined.NetworkAddress));
        await PublishAsync(
            new ZigbeeDeviceInfoDiscoveredEvent(
                ieeeAddress,
                joined.ManufacturerName,
                joined.ModelIdentifier,
                joined.Endpoints.Select(ToEndpointModel).ToList()
            )
        );
    }

    public async Task HandleAttributeReportedAsync(ZigbeeAttributeReport report)
    {
        var ieeeAddress = ResolveIeeeAddress(report.SourceNwkAddress);
        await PublishAsync(
            new ZigbeeAttributeReportReceivedEvent(
                report.SourceNwkAddress,
                ieeeAddress,
                report.ClusterId,
                report.AttributeId,
                (byte)report.Value.DataType,
                report.Value.RawValue,
                report.Value.StringValue
            )
        );
    }

    public async Task HandleCommandSentAsync(ZigbeeCommandSent sent)
    {
        var networkAddress =
            sent.Destination.Mode == DeconzAddressMode.Nwk ? (ushort?)sent.Destination.ShortAddress : null;
        var ieeeAddress =
            sent.Destination.Mode == DeconzAddressMode.Ieee
                ? ExternalIdConverter.ToExternalId(sent.Destination.IeeeAddress)
                : ResolveIeeeAddress(sent.Destination.ShortAddress);
        await PublishAsync(new ZigbeeCommandSentEvent(networkAddress, ieeeAddress, sent.ClusterId, sent.CommandId));
    }

    public async Task HandleTransportErrorAsync(ZigbeeTransportError error)
    {
        var ieeeAddress =
            error.IeeeAddress is { } address ? ExternalIdConverter.ToExternalId(address)
            : error.NetworkAddress is { } networkAddress ? ResolveIeeeAddress(networkAddress)
            : null;
        await PublishAsync(
            new ZigbeeTransportErrorEvent(error.ErrorType, error.Message, error.NetworkAddress, ieeeAddress)
        );
    }

    private static ZigbeeEndpointModel ToEndpointModel(ZigbeeEndpoint endpoint)
    {
        return new ZigbeeEndpointModel(endpoint.EndpointId, endpoint.InClusters, endpoint.OutClusters);
    }

    private string? ResolveIeeeAddress(ushort networkAddress)
    {
        return addressRegistry.TryGetExternalId(networkAddress, out var externalId) ? externalId : null;
    }

    private async Task PublishAsync<T>(IHausEventCreator<T> creator)
    {
        var mqttClient = await mqttClientFactory.CreateClient();
        await mqttClient.PublishHausEventAsync(creator, hausOptions.GetZigbeeTopic());
    }
}
