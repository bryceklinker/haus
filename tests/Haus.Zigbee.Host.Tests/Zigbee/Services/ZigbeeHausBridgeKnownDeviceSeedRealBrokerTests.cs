using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

// Unlike ZigbeeHausBridgeTests (FakeMqttClientFactory, a single in-process connection), this proves
// the actual guarantee #105 depends on against a real broker: a snapshot retained BEFORE this host
// ever existed is still delivered the moment a brand-new connection (re)subscribes -- the exact
// cold-start/restart race DeviceBackfillService's live-only reachability cannot cover.
public class ZigbeeHausBridgeKnownDeviceSeedRealBrokerTests : IAsyncLifetime
{
    private IHausMqttClient? _publisherMqttClient;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        if (_publisherMqttClient != null)
            await _publisherMqttClient.DisposeAsync();
    }

    [Fact]
    public async Task ColdBridgeStart_RetainedSeedPublishedBeforeItExisted_DeviceIsSeededOnSubscribe()
    {
        var externalId = $"0x{Guid.NewGuid():N}";
        var publisherProvider = ServiceProviderFactory.Create(configuration: ConfigurationFactory.CreateConfig());
        _publisherMqttClient = await publisherProvider.GetRequiredService<IHausMqttClientFactory>().CreateClient();
        var seed = new ZigbeeKnownDeviceSeedEvent([
            new ZigbeeKnownDeviceSeedEntry(externalId, 0x4242, [new DeviceEndpointModel(1, [6])], true),
        ]);
        await _publisherMqttClient.PublishHausEventAsync(
            seed,
            DefaultHausMqttTopics.ZigbeeKnownDeviceSeedTopic,
            retain: true
        );

        // A brand-new provider is a brand-new MQTT connection that has never subscribed to this
        // topic before -- the real stand-in for a Zigbee.Host process starting cold.
        var coldProvider = ServiceProviderFactory.Create(configuration: ConfigurationFactory.CreateConfig());
        var registry = coldProvider.GetRequiredService<DeviceAddressRegistry>();
        var store = coldProvider.GetRequiredService<IKnownZigbeeDeviceStore>();
        var bridge = coldProvider.GetServices<IHostedService>().OfType<ZigbeeHausBridge>().Single();

        await bridge.StartAsync(CancellationToken.None);
        try
        {
            await Eventually.AssertAsync(async () =>
            {
                Assert.True(registry.TryGetExternalId(0x4242, out var seededId));
                Assert.Equal(externalId, seededId);
                var devices = await store.GetAllAsync();
                Assert.Contains(devices, d => d.ExternalId == externalId && d.IsSleepy);
            });
        }
        finally
        {
            await bridge.StopAsync(CancellationToken.None);
        }
    }
}
