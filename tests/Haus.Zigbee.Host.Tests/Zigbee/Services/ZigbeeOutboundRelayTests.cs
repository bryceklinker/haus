using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Lighting;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Host.Zigbee.Mappers.ToZigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeOutboundRelayTests : IAsyncLifetime
{
    private IHausMqttClient? _hausMqttClient;
    private FakeZigbeeCoordinator? _coordinator;
    private DeviceAddressRegistry? _addressRegistry;
    private ZigbeeOutboundRelay? _relay;

    public async Task InitializeAsync()
    {
        var provider = ServiceProviderFactory.Create(mqttFactory: new FakeMqttClientFactory());
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _hausMqttClient = await mqttClientFactory.CreateClient();
        _coordinator = new FakeZigbeeCoordinator();
        _addressRegistry = new DeviceAddressRegistry();

        _relay = new ZigbeeOutboundRelay(
            _coordinator,
            new HausDiscoveryToZigbeeMapper(),
            new HausLightingToZigbeeMapper(),
            new DevicesMapper(),
            _addressRegistry,
            _hausMqttClient,
            NullLogger<ZigbeeOutboundRelay>.Instance
        );
    }

    public async Task DisposeAsync()
    {
        await _hausMqttClient!.DisposeAsync();
    }

    [Fact]
    public async Task HandleCommandAsync_StartDiscovery_EnablesPermitJoin()
    {
        var message = new StartDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.Equal([true], _coordinator!.PermitJoinCalls);
    }

    [Fact]
    public async Task HandleCommandAsync_StopDiscovery_DisablesPermitJoin()
    {
        var message = new StopDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.Equal([false], _coordinator!.PermitJoinCalls);
    }

    [Fact]
    public async Task HandleCommandAsync_SyncDiscovery_RegistersAddressesAndPublishesDiscoveredDevices()
    {
        var address = new IeeeAddress(1);
        _coordinator!.DevicesToReturn = [new ZigbeeDevice(address, 0x1234, [])];
        DeviceDiscoveredEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<DeviceDiscoveredEvent>(
            DeviceDiscoveredEvent.Type,
            e => published = e.Payload
        );
        var message = new SyncDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.True(_addressRegistry!.TryGetExternalId(0x1234, out var externalId));
        Assert.Equal(ExternalIdMap.ToExternalId(address), externalId);
        Eventually.Assert(() => Assert.Equal(ExternalIdMap.ToExternalId(address), published?.Id));
    }

    [Fact]
    public async Task HandleCommandAsync_LightingCommand_ResolvesAddressAndSendsCommands()
    {
        var address = new IeeeAddress(1);
        var device = new DeviceModel { ExternalId = ExternalIdMap.ToExternalId(address) };
        var message = new DeviceLightingChangedEvent(device, new LightingModel(LightingState.On))
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.NotEmpty(_coordinator!.SentCommands);
    }

    [Fact]
    public async Task HandleCommandAsync_LightingCommandWithUnresolvableExternalId_SendsNothing()
    {
        var device = new DeviceModel { ExternalId = "not-an-address" };
        var message = new DeviceLightingChangedEvent(device, new LightingModel(LightingState.On))
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.Empty(_coordinator!.SentCommands);
    }
}
