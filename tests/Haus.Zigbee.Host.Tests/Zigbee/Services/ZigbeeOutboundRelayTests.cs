using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Lighting;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeOutboundRelayTests : IAsyncLifetime
{
    private IHausMqttClient? _hausMqttClient;
    private FakeZigbeeCoordinator? _coordinator;
    private DeviceAddressRegistry? _addressRegistry;
    private ZigbeeOutboundRelay? _relay;
    private CapturingLoggerFactory? _loggerFactory;

    public async Task InitializeAsync()
    {
        _coordinator = new FakeZigbeeCoordinator();
        _loggerFactory = new CapturingLoggerFactory();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: _coordinator,
            configureServices: services => services.AddSingleton<ILoggerFactory>(_loggerFactory)
        );
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _hausMqttClient = await mqttClientFactory.CreateClient();
        _addressRegistry = provider.GetRequiredService<DeviceAddressRegistry>();

        _relay = provider.GetRequiredService<ZigbeeOutboundRelay>();
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
        Assert.Equal(ExternalIdConverter.ToExternalId(address), externalId);
        Eventually.Assert(() => Assert.Equal(ExternalIdConverter.ToExternalId(address), published?.Id));
    }

    [Fact]
    public async Task HandleCommandAsync_LightingCommand_ResolvesNetworkAddressAndSendsCommands()
    {
        const ushort networkAddress = 0x1234;
        var device = new DeviceModel
        {
            ExternalId = ExternalIdConverter.ToExternalId(new IeeeAddress(1)),
            NetworkAddress = networkAddress,
        };
        var message = new DeviceLightingChangedEvent(device, new LightingModel(LightingState.On))
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.NotEmpty(_coordinator!.SentCommands);
        Assert.All(
            _coordinator.SentCommands,
            request =>
            {
                Assert.Equal(DeconzAddressMode.Nwk, request.Destination.Mode);
                Assert.Equal(networkAddress, request.Destination.ShortAddress);
            }
        );
    }

    [Fact]
    public async Task HandleCommandAsync_LightingCommandWithoutNetworkAddress_SendsNothing()
    {
        var device = new DeviceModel { ExternalId = ExternalIdConverter.ToExternalId(new IeeeAddress(1)) };
        var message = new DeviceLightingChangedEvent(device, new LightingModel(LightingState.On))
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.Empty(_coordinator!.SentCommands);
    }

    [Fact]
    public async Task HandleCommandAsync_LightingCommandWithFailedConfirmThenSuccessOnRetry_RetriesOnceWithApsAckAndDoesNotLogFailure()
    {
        _coordinator!.ConfirmSequence.Enqueue(FailedConfirm());
        _coordinator.ConfirmSequence.Enqueue(SuccessfulConfirm());
        var device = new DeviceModel
        {
            ExternalId = ExternalIdConverter.ToExternalId(new IeeeAddress(1)),
            NetworkAddress = 0x1234,
        };
        var message = new DeviceLightingChangedEvent(device, new LightingModel(LightingState.Off))
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.Equal(2, _coordinator.SentCommands.Count);
        Assert.False(_coordinator.SentCommands[0].RequestApsAck);
        Assert.True(_coordinator.SentCommands[1].RequestApsAck);
        Assert.Equal(_coordinator.SentCommands[0].Destination, _coordinator.SentCommands[1].Destination);
        Assert.DoesNotContain(
            _loggerFactory!.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains("failed with APS confirm status")
        );
    }

    [Fact]
    public async Task HandleCommandAsync_LightingCommandWithConfirmFailingTwice_RetriesExactlyOnceThenLogsFailure()
    {
        _coordinator!.ConfirmToReturn = FailedConfirm();
        var device = new DeviceModel
        {
            ExternalId = ExternalIdConverter.ToExternalId(new IeeeAddress(1)),
            NetworkAddress = 0x1234,
        };
        var message = new DeviceLightingChangedEvent(device, new LightingModel(LightingState.Off))
            .AsHausCommand()
            .ToMqttMessage("haus/commands");

        await _relay!.HandleCommandAsync(message, CancellationToken.None);

        Assert.Equal(2, _coordinator.SentCommands.Count);
        Assert.False(_coordinator.SentCommands[0].RequestApsAck);
        Assert.True(_coordinator.SentCommands[1].RequestApsAck);
        Assert.Single(
            _loggerFactory!.Entries,
            entry => entry.Level == LogLevel.Warning && entry.Message.Contains("failed with APS confirm status")
        );
    }

    private static ApsDataConfirm FailedConfirm()
    {
        return new ApsDataConfirm(0, 0, 0, DeconzAddressMode.Nwk, 0x1234, null, 1, 1, ConfirmStatus: 0xAD);
    }

    private static ApsDataConfirm SuccessfulConfirm()
    {
        return new ApsDataConfirm(0, 0, 0, DeconzAddressMode.Nwk, 0x1234, null, 1, 1, ConfirmStatus: 0x00);
    }
}
