using System.Threading.Tasks;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Zigbee.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Zigbee;

[Collection(HausWebHostCollectionFixture.Name)]
public class ZigbeeRealtimeApiTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenTransportErrorEventPublishedThenBroadcastOverEventsHub()
    {
        var hub = await factory.CreateHubConnection("events");
        HausEvent<ZigbeeTransportErrorEvent>? received = null;
        hub.On<HausEvent<ZigbeeTransportErrorEvent>>(
            "OnEvent",
            e =>
            {
                if (e.Payload?.ErrorType == "timeout")
                    received = e;
            }
        );

        await factory.PublishZigbeeEventAsync(new ZigbeeTransportErrorEvent("timeout", "boom", 1, "ieee-1"));

        Eventually.Assert(() =>
        {
            Assert.NotNull(received);
            Assert.Equal("boom", received!.Payload!.Message);
        });
    }
}
