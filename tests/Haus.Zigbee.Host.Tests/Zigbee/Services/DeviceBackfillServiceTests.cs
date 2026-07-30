using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices.Events;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class DeviceBackfillServiceTests : IAsyncLifetime
{
    private IHausMqttClient? _mqttClient;
    private FakeZigbeeCoordinator? _coordinator;
    private DeviceBackfillService? _service;

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
            Assert.Equal(ExternalIdMap.ToExternalId(address), published?.Id);
            Assert.NotEqual(Haus.Core.Models.Devices.DeviceType.Unknown, published?.DeviceType);
        });
    }

    [Fact]
    public async Task BackfillAsync_DeviceInfoResolvesToUnknown_DoesNotPublish()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        _coordinator.DeviceInfoToReturn = new ZigbeeDeviceInfo("nope", "nope");
        var published = false;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            _ => published = true
        );

        await _service!.BackfillAsync(CancellationToken.None);
        await Task.Delay(200);

        Assert.False(published);
    }

    [Fact]
    public async Task BackfillAsync_ReadDeviceInfoReturnsNull_DoesNotPublish()
    {
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(new IeeeAddress(1), 0x1234, [])];
        _coordinator.DeviceInfoToReturn = null;
        var published = false;
        await _mqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            _ => published = true
        );

        await _service!.BackfillAsync(CancellationToken.None);
        await Task.Delay(200);

        Assert.False(published);
    }
}
