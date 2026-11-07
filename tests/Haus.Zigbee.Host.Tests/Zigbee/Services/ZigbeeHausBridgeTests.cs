using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Testing.Support.Fakes;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

public class ZigbeeHausBridgeTests
{
    [Fact]
    public async Task WhenCoordinatorConnectionStatusChangesThenTheDiagnosticsPublisherReceivesIt()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        ZigbeeConnectionStatusChangedEvent? published = null;
        await mqttClient.SubscribeToHausEventsAsync<ZigbeeConnectionStatusChangedEvent>(
            ZigbeeConnectionStatusChangedEvent.Type,
            e => published = e.Payload,
            $"{DefaultHausMqttTopics.ZigbeeTopic}/status"
        );

        coordinator.RaiseConnectionStatusChanged(new ZigbeeConnectionStatus(true, null, null));

        Eventually.Assert(() => Assert.True(published?.IsConnected));

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task WhenCoordinatorCommandSentThenTheDiagnosticsPublisherReceivesIt()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        ZigbeeCommandSentEvent? published = null;
        await mqttClient.SubscribeToHausEventsAsync<ZigbeeCommandSentEvent>(
            ZigbeeCommandSentEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );

        coordinator.RaiseCommandSent(new ZigbeeCommandSent(ApsDestination.Nwk(0x1234, 0x01), 0x0006, 0x01));

        Eventually.Assert(() => Assert.Equal((ushort)0x1234, published?.NetworkAddress));

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task WhenCoordinatorTransportErrorThenTheDiagnosticsPublisherReceivesIt()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        ZigbeeTransportErrorEvent? published = null;
        await mqttClient.SubscribeToHausEventsAsync<ZigbeeTransportErrorEvent>(
            ZigbeeTransportErrorEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );

        coordinator.RaiseTransportError(new ZigbeeTransportError("IOException", "boom", null, null));

        Eventually.Assert(() => Assert.Equal("boom", published?.Message));

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task WhenDeviceJoinsThenTheDiagnosticsPublisherAlsoReceivesIt()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        ZigbeeDeviceJoinedEvent? published = null;
        await mqttClient.SubscribeToHausEventsAsync<ZigbeeDeviceJoinedEvent>(
            ZigbeeDeviceJoinedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );

        coordinator.RaiseDeviceJoined(new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [], "acme", "widget"));

        Eventually.Assert(() => Assert.Equal((ushort)0x1234, published?.NetworkAddress));

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task WhenAttributeReportedThenTheDiagnosticsPublisherAlsoReceivesIt()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        ZigbeeAttributeReportReceivedEvent? published = null;
        await mqttClient.SubscribeToHausEventsAsync<ZigbeeAttributeReportReceivedEvent>(
            ZigbeeAttributeReportReceivedEvent.Type,
            e => published = e.Payload,
            DefaultHausMqttTopics.ZigbeeTopic
        );

        coordinator.RaiseAttributeReported(
            new ZigbeeAttributeReport(
                0x1234,
                1,
                0x0402,
                0x0000,
                new Haus.Zigbee.Zcl.ZclAttributeValue(Haus.Zigbee.Zcl.ZclDataType.Int16, 100)
            )
        );

        Eventually.Assert(() => Assert.Equal((ushort)0x1234, published?.NetworkAddress));

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task HandleHausCommand_OutboundRelayThrows_SubsequentCommandsStillGetHandled()
    {
        var coordinator = new FakeZigbeeCoordinator
        {
            SetPermitJoinShouldThrow = new InvalidOperationException("boom"),
        };
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);

        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        var hausMqttClient = await mqttClientFactory.CreateClient();

        var badMessage = new StartDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");
        await hausMqttClient.PublishAsync(badMessage);

        coordinator.SetPermitJoinShouldThrow = null;
        var goodMessage = new StopDiscoveryModel().AsHausCommand().ToMqttMessage("haus/commands");
        await hausMqttClient.PublishAsync(goodMessage);

        Eventually.Assert(() => Assert.Contains(false, coordinator.PermitJoinCalls));
        Assert.DoesNotContain(true, coordinator.PermitJoinCalls);

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_CoordinatorConnectFails_DoesNotThrow()
    {
        var coordinator = new FakeZigbeeCoordinator { ConnectShouldThrow = new InvalidOperationException("no port") };
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);

        var exception = await Record.ExceptionAsync(() => bridge.StartAsync(CancellationToken.None));

        Assert.Null(exception);
        Assert.False(coordinator.IsConnected);
        Assert.True(coordinator.ConnectAttempts >= 1);

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_CoordinatorConnectSucceeds_LeavesCoordinatorConnected()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);

        await bridge.StartAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task StopAsync_CalledWithoutStartAsyncHavingSetTheMqttClient_DoesNotThrow()
    {
        var coordinator = new FakeZigbeeCoordinator();
        var provider = ServiceProviderFactory.Create(
            mqttFactory: new FakeMqttClientFactory(),
            zigbeeCoordinator: coordinator
        );
        var bridge = ResolveBridge(provider);

        var exception = await Record.ExceptionAsync(() => bridge.StopAsync(CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task WhenKnownDeviceSeedSnapshotArrivesThenEachDeviceIsSeeded()
    {
        var provider = ServiceProviderFactory.Create(mqttFactory: new FakeMqttClientFactory());
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        var registry = provider.GetRequiredService<DeviceAddressRegistry>();
        var store = provider.GetRequiredService<IKnownZigbeeDeviceStore>();
        var seed = new ZigbeeKnownDeviceSeedEvent([
            new ZigbeeKnownDeviceSeedEntry("0x1", 0x1234, [new DeviceEndpointModel(1, [6])], true),
            new ZigbeeKnownDeviceSeedEntry("0x2", 0x5678, [], false),
        ]);

        await mqttClient.PublishHausEventAsync(seed, DefaultHausMqttTopics.ZigbeeKnownDeviceSeedTopic, retain: true);

        await Eventually.AssertAsync(async () =>
        {
            Assert.True(registry.TryGetExternalId(0x1234, out var firstId));
            Assert.Equal("0x1", firstId);
            Assert.True(registry.TryGetExternalId(0x5678, out var secondId));
            Assert.Equal("0x2", secondId);
            Assert.Equal(2, (await store.GetAllAsync()).Count);
        });

        await bridge.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task WhenUnparseableSeedMessageArrivesThenSubsequentValidSnapshotsStillSeed()
    {
        var provider = ServiceProviderFactory.Create(mqttFactory: new FakeMqttClientFactory());
        var bridge = ResolveBridge(provider);
        await bridge.StartAsync(CancellationToken.None);
        var mqttClient = await provider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        var registry = provider.GetRequiredService<DeviceAddressRegistry>();

        await mqttClient.PublishAsync(
            new MqttApplicationMessage
            {
                Topic = DefaultHausMqttTopics.ZigbeeKnownDeviceSeedTopic,
                PayloadSegment = Encoding.UTF8.GetBytes("not valid json"),
            }
        );
        var seed = new ZigbeeKnownDeviceSeedEvent([new ZigbeeKnownDeviceSeedEntry("0x1", 0x1234, [], false)]);
        await mqttClient.PublishHausEventAsync(seed, DefaultHausMqttTopics.ZigbeeKnownDeviceSeedTopic, retain: true);

        await Eventually.AssertAsync(async () =>
        {
            Assert.True(registry.TryGetExternalId(0x1234, out var externalId));
            Assert.Equal("0x1", externalId);
            await Task.CompletedTask;
        });

        await bridge.StopAsync(CancellationToken.None);
    }

    private static IHostedService ResolveBridge(IServiceProvider provider)
    {
        return provider.GetServices<IHostedService>().OfType<ZigbeeHausBridge>().Single();
    }
}
