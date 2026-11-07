using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee.Events;
using Haus.Cqrs;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Web.Host.Tests.Zigbee;

// Unlike ZigbeeKnownDeviceSeedPublisherTests, this drives the command through the full app's real
// DI container and a real broker, proving the whole dispatch chain wires up, not just the handler.
[Collection(HausWebHostCollectionFixture.Name)]
public class ZigbeeKnownDeviceSeedPublishingFlowTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenPublishCommandRunsThroughTheRealAppThenTheRegisteredHandlerPutsItOnTheWire()
    {
        var externalId = $"{System.Guid.NewGuid()}";
        await factory.PublishHausEventAsync(
            new DeviceDiscoveredEvent(
                externalId,
                DeviceType.Light,
                NetworkAddress: 4660,
                Endpoints: [new DeviceEndpointModel(1, [6, 8])],
                IsSleepy: true
            )
        );
        var apiClient = factory.CreateAuthenticatedClient();
        await WaitFor.ResultAsync(async () => (await apiClient.GetDevicesAsync(externalId)).Items.Single());

        HausEvent<ZigbeeKnownDeviceSeedEvent>? received = null;
        var mqttClient = await factory.GetMqttClient();
        await mqttClient.SubscribeToHausEventsAsync<ZigbeeKnownDeviceSeedEvent>(
            ZigbeeKnownDeviceSeedEvent.Type,
            evt => received = evt,
            DefaultHausMqttTopics.ZigbeeKnownDeviceSeedTopic
        );

        using var scope = factory.Services.CreateScope();
        var hausBus = scope.ServiceProvider.GetRequiredService<IHausBus>();
        await Eventually.AssertAsync(async () =>
        {
            await hausBus.ExecuteCommandAsync(new PublishZigbeeKnownDeviceSeedCommand());
            Assert.NotNull(received);
            var entry = Assert.Single(received!.Payload.Devices, d => d.ExternalId == externalId);
            Assert.Equal((ushort)4660, entry.NetworkAddress);
            Assert.True(entry.IsSleepy);
        });
    }
}
