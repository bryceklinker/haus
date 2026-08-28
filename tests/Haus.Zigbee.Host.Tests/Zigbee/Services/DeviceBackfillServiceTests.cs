using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices.Events;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Haus.Zigbee.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class DeviceBackfillServiceTests : IAsyncLifetime
{
    private IHausMqttClient? _mqttClient;
    private FakeZigbeeCoordinator? _coordinator;
    private DeviceBackfillService? _service;
    private DeviceAddressRegistry? _addressRegistry;

    public async Task InitializeAsync()
    {
        _coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: _coordinator
        );
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _mqttClient = await mqttClientFactory.CreateClient();

        _service = provider.GetRequiredService<DeviceBackfillService>();
        _addressRegistry = provider.GetRequiredService<DeviceAddressRegistry>();
    }

    public async Task DisposeAsync()
    {
        await _mqttClient!.DisposeAsync();
    }

    [Fact]
    public async Task BackfillAsync_DeviceResolvesToAKnownType_PublishesUpdatedDeviceDiscoveredEvent()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        _coordinator.DeviceInfoToReturn = new ZigbeeDeviceInfo("Philips", "929002335001");
        DeviceDiscoveredEvent? published = null;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            e => published = e.Payload
        );

        await _service!.BackfillAsync(CancellationToken.None);

        Eventually.Assert(() =>
        {
            Assert.Equal(ExternalIdConverter.ToExternalId(address), published?.Id);
            Assert.NotEqual(Haus.Core.Models.Devices.DeviceType.Unknown, published?.DeviceType);
        });
    }

    [Fact]
    public async Task BackfillAsync_DeviceResolvesToAKnownType_RegistersNetworkAddressForFutureAttributeReports()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        _coordinator.DeviceInfoToReturn = new ZigbeeDeviceInfo("Philips", "929002335001");

        await _service!.BackfillAsync(CancellationToken.None);

        var found = _addressRegistry!.TryGetExternalId(0x1234, out var externalId);
        Assert.True(found);
        Assert.Equal(ExternalIdConverter.ToExternalId(address), externalId);
    }

    [Fact]
    public async Task BackfillAsync_ResolvesEachKnownDevicesNetworkAddressAndUsesTheResolvedAddress()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        _coordinator.DeviceInfoToReturn = new ZigbeeDeviceInfo("Philips", "929002335001");
        _coordinator.NetworkAddressToReturn = 0x5678;
        DeviceDiscoveredEvent? published = null;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            e => published = e.Payload
        );

        await _service!.BackfillAsync(CancellationToken.None);

        Assert.Equal([address], _coordinator.ResolveNetworkAddressCalls);
        Eventually.Assert(() => Assert.Equal((ushort?)0x5678, published?.NetworkAddress));
    }

    [Fact]
    public async Task BackfillAsync_ResolutionFindsNoAnswer_FallsBackToTheAlreadyKnownNetworkAddress()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        _coordinator.DeviceInfoToReturn = new ZigbeeDeviceInfo("Philips", "929002335001");
        _coordinator.NetworkAddressToReturn = null;
        DeviceDiscoveredEvent? published = null;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            e => published = e.Payload
        );

        await _service!.BackfillAsync(CancellationToken.None);

        Eventually.Assert(() => Assert.Equal((ushort?)0x1234, published?.NetworkAddress));
    }

    [Fact]
    public async Task BackfillAsync_DeviceInfoResolvesToUnknown_DoesNotPublish()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        _coordinator.DeviceInfoToReturn = new ZigbeeDeviceInfo("nope", "nope");
        var published = false;
        var canaryReceived = false;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            _ => published = true
        );
        await _mqttClient!.SubscribeToHausEventsAsync<DiscoveryStoppedEvent>(
            DiscoveryStoppedEvent.Type,
            _ => canaryReceived = true
        );

        await _service!.BackfillAsync(CancellationToken.None);
        await _mqttClient!.PublishHausEventAsync(new DiscoveryStoppedEvent());

        Eventually.Assert(() => Assert.True(canaryReceived));
        Assert.False(published);
    }

    [Fact]
    public async Task BackfillAsync_ReadDeviceInfoReturnsNull_DoesNotPublish()
    {
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(new IeeeAddress(1), 0x1234, [])];
        _coordinator.DeviceInfoToReturn = null;
        var published = false;
        var canaryReceived = false;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            _ => published = true
        );
        await _mqttClient!.SubscribeToHausEventsAsync<DiscoveryStoppedEvent>(
            DiscoveryStoppedEvent.Type,
            _ => canaryReceived = true
        );

        await _service!.BackfillAsync(CancellationToken.None);
        await _mqttClient!.PublishHausEventAsync(new DiscoveryStoppedEvent());

        Eventually.Assert(() => Assert.True(canaryReceived));
        Assert.False(published);
    }
}
