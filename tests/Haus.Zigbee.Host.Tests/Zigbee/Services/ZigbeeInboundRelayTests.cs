using System;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.ExternalMessages;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Resolvers;
using Haus.Zigbee.Host.Zigbee.Services;
using Haus.Zigbee.Zcl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeInboundRelayTests : IAsyncLifetime
{
    private const ushort PowerConfigCluster = 0x0001;
    private const ushort BatteryPercentageAttribute = 0x0021;

    private IHausMqttClient? _hausMqttClient;
    private DeviceAddressRegistry? _addressRegistry;
    private ZigbeeInboundRelay? _relay;

    public async Task InitializeAsync()
    {
        var provider = ServiceProviderFactory.Create(mqttFactory: new FakeMqttClientFactory());
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _hausMqttClient = await mqttClientFactory.CreateClient();
        _addressRegistry = new DeviceAddressRegistry();

        var hausOptions = provider.GetRequiredService<IOptions<HausOptions>>();
        var deviceJoinedMapper = new DeviceJoinedMapper(new DeviceTypeResolver(hausOptions));
        var deviceEventMapper = new DeviceEventMapper(_addressRegistry, NullLogger<DeviceEventMapper>.Instance);

        _relay = new ZigbeeInboundRelay(
            _addressRegistry,
            deviceJoinedMapper,
            deviceEventMapper,
            _hausMqttClient,
            hausOptions
        );
    }

    public async Task DisposeAsync()
    {
        await _hausMqttClient!.DisposeAsync();
    }

    [Fact]
    public async Task HandleDeviceJoinedAsync_RegistersNetworkAddressForFutureAttributeReports()
    {
        var address = new IeeeAddress(0x00124b0012345678);
        var joined = new ZigbeeDeviceJoined(address, 0x1234, [], "acme", "widget");

        await _relay!.HandleDeviceJoinedAsync(joined);

        var found = _addressRegistry!.TryGetExternalId(0x1234, out var externalId);
        Assert.True(found);
        Assert.Equal(ExternalIdMap.ToExternalId(address), externalId);
    }

    [Fact]
    public async Task HandleDeviceJoinedAsync_PublishesDeviceDiscoveredEvent()
    {
        DeviceDiscoveredEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            e => published = e.Payload
        );
        var address = new IeeeAddress(1);
        var joined = new ZigbeeDeviceJoined(address, 0x1234, [], "acme", "widget");

        await _relay!.HandleDeviceJoinedAsync(joined);

        Eventually.Assert(() => Assert.Equal(ExternalIdMap.ToExternalId(address), published?.Id));
    }

    [Fact]
    public async Task HandleAttributeReportedAsync_KnownDevice_PublishesSensorEvent()
    {
        _addressRegistry!.Register(0x1234, "the-device-id");
        BatteryChangedModel? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<BatteryChangedModel>(
            BatteryChangedModel.Type,
            e => published = e.Payload
        );
        var report = new ZigbeeAttributeReport(
            0x1234,
            1,
            PowerConfigCluster,
            BatteryPercentageAttribute,
            new ZclAttributeValue(ZclDataType.Uint8, 86)
        );

        await _relay!.HandleAttributeReportedAsync(report);

        Eventually.Assert(() => Assert.Equal("the-device-id", published?.DeviceId));
    }

    [Fact]
    public async Task HandleAttributeReportedAsync_UnrecognizedReport_DoesNotThrowOrPublish()
    {
        _addressRegistry!.Register(0x1234, "the-device-id");
        var report = new ZigbeeAttributeReport(
            0x1234,
            1,
            PowerConfigCluster,
            0x00ff,
            new ZclAttributeValue(ZclDataType.Uint8, 1)
        );

        await _relay!.HandleAttributeReportedAsync(report);
    }
}
