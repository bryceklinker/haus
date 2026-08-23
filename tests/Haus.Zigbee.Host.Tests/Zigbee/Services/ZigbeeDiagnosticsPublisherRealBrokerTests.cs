using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee.Events;
using Haus.Mqtt.Client;
using Haus.Testing.Support;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Services;

// Unlike ZigbeeDiagnosticsPublisherTests, this exercises the publisher against a real MQTT broker
// (no FakeMqttClientFactory) -- the same real-broker pattern Haus.Web.Host.Tests relies on, backed
// by the disposable broker make test-unit stands up on Haus__Server (see ConfigurationFactory).
public class ZigbeeDiagnosticsPublisherRealBrokerTests : IAsyncLifetime
{
    private IHausMqttClient? _hausMqttClient;
    private ZigbeeDiagnosticsPublisher? _publisher;

    public async Task InitializeAsync()
    {
        var provider = ServiceProviderFactory.Create(configuration: ConfigurationFactory.CreateConfig());
        var mqttClientFactory = provider.GetRequiredService<IHausMqttClientFactory>();
        _hausMqttClient = await mqttClientFactory.CreateClient();

        _publisher = provider.GetRequiredService<ZigbeeDiagnosticsPublisher>();
    }

    public async Task DisposeAsync()
    {
        await _hausMqttClient!.DisposeAsync();
    }

    [Fact]
    public async Task HandleConnectionStatusChangedAsync_RealBroker_RoundTripsOverHausZigbeeStatusTopic()
    {
        ZigbeeConnectionStatusChangedEvent? published = null;
        await _hausMqttClient!.SubscribeToHausEventsAsync<ZigbeeConnectionStatusChangedEvent>(
            ZigbeeConnectionStatusChangedEvent.Type,
            e => published = e.Payload,
            $"{DefaultHausMqttTopics.ZigbeeTopic}/status"
        );

        // MQTTnet's managed client applies a SubscribeAsync call asynchronously in the background
        // rather than confirming it against the broker before returning, so a single publish
        // immediately after subscribing can race ahead of the subscription actually landing.
        // Re-publishing on every retry (rather than once before the loop) closes that window.
        await Eventually.AssertAsync(async () =>
        {
            await _publisher!.HandleConnectionStatusChangedAsync(new ZigbeeConnectionStatus(true, null, null));
            Assert.NotNull(published);
            Assert.True(published!.IsConnected);
        });
    }
}
