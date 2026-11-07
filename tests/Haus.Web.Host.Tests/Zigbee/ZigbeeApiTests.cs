using System;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Models;
using Xunit;

namespace Haus.Web.Host.Tests.Zigbee;

[Collection(HausWebHostCollectionFixture.Name)]
public class ZigbeeApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenZigbeeHostReportsConnectionStatusChangedThenStatusEndpointReflectsIt()
    {
        factory.ZigbeeHostFactory.Coordinator.RaiseConnectionStatusChanged(
            new ZigbeeConnectionStatus(true, new NetworkConfig(new IeeeAddress(0x1), 0x1a62, 11), "connected")
        );

        await Eventually.AssertAsync(async () =>
        {
            var status = await _client.GetZigbeeStatusAsync();
            Assert.NotNull(status);
            Assert.True(status.IsConnected);
            Assert.Equal("connected", status.Reason);
        });
    }

    [Fact]
    public async Task WhenZigbeeHostCoordinatorRaisesDeviceJoinedThenActivityEndpointHasIt()
    {
        var ieeeAddress = new IeeeAddress((ulong)Random.Shared.NextInt64());
        factory.ZigbeeHostFactory.Coordinator.RaiseDeviceJoined(
            new ZigbeeDeviceJoined(ieeeAddress, 7, [], "ACME", "Bulb")
        );

        await Eventually.AssertAsync(async () =>
        {
            var activity = await _client.GetZigbeeActivityAsync();
            Assert.Contains(activity.Items, e => e.EventType == "zigbee_device_joined");
        });
    }

    [Fact]
    public async Task WhenZigbeeHostStoresKnownDeviceThenDevicesEndpointHasIt()
    {
        var externalId = $"{Guid.NewGuid()}";
        await factory
            .ZigbeeHostFactory.GetKnownDeviceStore()
            .UpsertAsync(new RegisterKnownDevice(externalId, 9, [], IsSleepy: false));

        await Eventually.AssertAsync(async () =>
        {
            var devices = await _client.GetZigbeeDevicesAsync();
            Assert.Contains(devices.Items, d => d.IeeeAddress == externalId);
        });
    }
}
