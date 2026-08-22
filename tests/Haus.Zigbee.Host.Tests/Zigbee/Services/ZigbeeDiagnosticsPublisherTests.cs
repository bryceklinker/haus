using System.Collections.Generic;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeDiagnosticsPublisherTests : IAsyncLifetime
{
    private IHausMqttClient? _hausMqttClient;
    private DeviceAddressRegistry? _addressRegistry;
    private ZigbeeDiagnosticsPublisher? _publisher;

    public async Task InitializeAsync()
    {
        var provider = ServiceProviderFactory.Create(mqttFactory: new FakeMqttClientFactory());
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _hausMqttClient = await mqttClientFactory.CreateClient();
        _addressRegistry = provider.GetRequiredService<DeviceAddressRegistry>();

        _publisher = provider.GetRequiredService<ZigbeeDiagnosticsPublisher>();
    }

    public async Task DisposeAsync()
    {
        await _hausMqttClient!.DisposeAsync();
    }

    [Fact]
    public async Task HandleConnectionStatusChangedAsync_Connected_PublishesConnectedStatus()
    {
        ZigbeeConnectionStatusChangedEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeConnectionStatusChangedEvent>(
            ZigbeeConnectionStatusChangedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var networkConfig = new NetworkConfig(new IeeeAddress(0x00124b0001aabbcc), 0x1a62, 0x0f);
        var status = new ZigbeeConnectionStatus(true, networkConfig, null);

        await _publisher!.HandleConnectionStatusChangedAsync(status);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.True(published!.IsConnected);
            Assert.Equal("0x00124b0001aabbcc", published.NetworkConfig!.MacAddress);
            Assert.Equal((ushort)0x1a62, published.NetworkConfig.PanId);
            Assert.Equal((byte)0x0f, published.NetworkConfig.Channel);
            Assert.Null(published.Reason);
        });
    }

    [Fact]
    public async Task HandleConnectionStatusChangedAsync_Disconnected_PublishesDisconnectedStatusWithReason()
    {
        ZigbeeConnectionStatusChangedEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeConnectionStatusChangedEvent>(
            ZigbeeConnectionStatusChangedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var status = new ZigbeeConnectionStatus(false, null, "dongle unplugged");

        await _publisher!.HandleConnectionStatusChangedAsync(status);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.False(published!.IsConnected);
            Assert.Null(published.NetworkConfig);
            Assert.Equal("dongle unplugged", published.Reason);
        });
    }

    [Fact]
    public async Task HandleDeviceJoinedAsync_PublishesDeviceJoinedEvent()
    {
        ZigbeeDeviceJoinedEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeDeviceJoinedEvent>(
            ZigbeeDeviceJoinedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var address = new IeeeAddress(0x00124b0001aabbcc);
        var joined = new ZigbeeDeviceJoined(address, 0x1a2b, [], "acme", "widget");

        await _publisher!.HandleDeviceJoinedAsync(joined);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Equal("0x00124b0001aabbcc", published!.IeeeAddress);
            Assert.Equal((ushort)0x1a2b, published.NetworkAddress);
        });
    }

    [Fact]
    public async Task HandleDeviceJoinedAsync_PublishesDeviceInfoDiscoveredEvent()
    {
        ZigbeeDeviceInfoDiscoveredEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeDeviceInfoDiscoveredEvent>(
            ZigbeeDeviceInfoDiscoveredEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var address = new IeeeAddress(0x00124b0001aabbcc);
        var endpoints = new List<ZigbeeEndpoint> { new(0x01, 0x0104, 0x0000, [0x0000, 0x0006], [0x0019]) };
        var joined = new ZigbeeDeviceJoined(address, 0x1a2b, endpoints, "acme", "widget");

        await _publisher!.HandleDeviceJoinedAsync(joined);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Equal("0x00124b0001aabbcc", published!.IeeeAddress);
            Assert.Equal("acme", published.ManufacturerName);
            Assert.Equal("widget", published.ModelIdentifier);
            var endpoint = Assert.Single(published.Endpoints);
            Assert.Equal((byte)0x01, endpoint.EndpointId);
            Assert.Equal(new ushort[] { 0x0000, 0x0006 }, endpoint.InClusters);
            Assert.Equal(new ushort[] { 0x0019 }, endpoint.OutClusters);
        });
    }

    [Fact]
    public async Task HandleAttributeReportedAsync_KnownDevice_PublishesReportWithResolvedIeeeAddress()
    {
        _addressRegistry!.Register(0x1a2b, "0x00124b0001aabbcc");
        ZigbeeAttributeReportReceivedEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeAttributeReportReceivedEvent>(
            ZigbeeAttributeReportReceivedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var report = new ZigbeeAttributeReport(
            0x1a2b,
            1,
            0x0402,
            0x0000,
            new ZclAttributeValue(ZclDataType.Int16, 2100)
        );

        await _publisher!.HandleAttributeReportedAsync(report);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Equal((ushort)0x1a2b, published!.NetworkAddress);
            Assert.Equal("0x00124b0001aabbcc", published.IeeeAddress);
            Assert.Equal((ushort)0x0402, published.ClusterId);
            Assert.Equal((ushort)0x0000, published.AttributeId);
            Assert.Equal((byte)ZclDataType.Int16, published.DataType);
            Assert.Equal(2100ul, published.RawValue);
        });
    }

    [Fact]
    public async Task HandleAttributeReportedAsync_UnknownDevice_PublishesReportWithNoIeeeAddress()
    {
        ZigbeeAttributeReportReceivedEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeAttributeReportReceivedEvent>(
            ZigbeeAttributeReportReceivedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var report = new ZigbeeAttributeReport(
            0x9999,
            1,
            0x0402,
            0x0000,
            new ZclAttributeValue(ZclDataType.Int16, 2100)
        );

        await _publisher!.HandleAttributeReportedAsync(report);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Null(published!.IeeeAddress);
        });
    }

    [Fact]
    public async Task HandleCommandSentAsync_IeeeDestination_PublishesResolvedTarget()
    {
        ZigbeeCommandSentEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeCommandSentEvent>(
            ZigbeeCommandSentEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var destination = ApsDestination.Ieee(new IeeeAddress(0x00124b0001aabbcc), 0x01);
        var sent = new ZigbeeCommandSent(destination, 0x0006, 0x01);

        await _publisher!.HandleCommandSentAsync(sent);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Equal("0x00124b0001aabbcc", published!.IeeeAddress);
            Assert.Null(published.NetworkAddress);
            Assert.Equal((ushort)0x0006, published.ClusterId);
            Assert.Equal((byte)0x01, published.CommandId);
        });
    }

    [Fact]
    public async Task HandleCommandSentAsync_NwkDestination_PublishesResolvedNetworkAddress()
    {
        ZigbeeCommandSentEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeCommandSentEvent>(
            ZigbeeCommandSentEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var destination = ApsDestination.Nwk(0x1a2b, 0x01);
        var sent = new ZigbeeCommandSent(destination, 0x0006, 0x01);

        await _publisher!.HandleCommandSentAsync(sent);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Equal((ushort)0x1a2b, published!.NetworkAddress);
            Assert.Null(published.IeeeAddress);
        });
    }

    [Fact]
    public async Task HandleTransportErrorAsync_PublishesErrorTypeAndMessage()
    {
        ZigbeeTransportErrorEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeTransportErrorEvent>(
            ZigbeeTransportErrorEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );
        var error = new ZigbeeTransportError("IOException", "Simulated serial read failure", null, null);

        await _publisher!.HandleTransportErrorAsync(error);

        Eventually.Assert(() =>
        {
            Assert.NotNull(published);
            Assert.Equal("IOException", published!.ErrorType);
            Assert.Equal("Simulated serial read failure", published.Message);
            Assert.Null(published.NetworkAddress);
            Assert.Null(published.IeeeAddress);
        });
    }
}
