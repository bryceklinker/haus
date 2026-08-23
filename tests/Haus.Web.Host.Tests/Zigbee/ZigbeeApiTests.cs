using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models.Common;
using Haus.Core.Models.Zigbee.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Zigbee;

[Collection(HausWebHostCollectionFixture.Name)]
public class ZigbeeApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenConnectionStatusChangedEventPublishedThenStatusEndpointReflectsIt()
    {
        await factory.PublishZigbeeEventAsync(new ZigbeeConnectionStatusChangedEvent(true, null, "connected"));

        await Eventually.AssertAsync(async () =>
        {
            var status = await _client.GetZigbeeStatusAsync();
            Assert.NotNull(status);
            Assert.True(status.IsConnected);
            Assert.Equal("connected", status.Reason);
        });
    }

    [Fact]
    public async Task WhenZigbeeEventsPublishedThenActivityEndpointHasThem()
    {
        var ieeeAddress = $"{System.Guid.NewGuid()}";
        await factory.PublishZigbeeEventAsync(new ZigbeeDeviceJoinedEvent(ieeeAddress, 7));

        await Eventually.AssertAsync(async () =>
        {
            var activity = await _client.GetZigbeeActivityAsync();
            Assert.Contains(activity.Items, e => e.EventType == ZigbeeDeviceJoinedEvent.Type);
        });
    }

    [Fact]
    public async Task WhenDeviceJoinedEventPublishedThenDevicesEndpointHasKnownDevice()
    {
        var ieeeAddress = $"{System.Guid.NewGuid()}";
        await factory.PublishZigbeeEventAsync(new ZigbeeDeviceJoinedEvent(ieeeAddress, 9));

        await Eventually.AssertAsync(async () =>
        {
            var devices = await _client.GetZigbeeDevicesAsync();
            Assert.Contains(devices.Items, d => d.IeeeAddress == ieeeAddress);
        });
    }
}
