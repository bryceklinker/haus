using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class DeviceDiscoveredTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenDeviceDiscoveredEventReceivedThenDeviceIsAvailableFromTheApi()
    {
        var deviceType = DeviceType.LightSensor | DeviceType.MotionSensor | DeviceType.TemperatureSensor;
        await factory.PublishHausEventAsync(new DeviceDiscoveredEvent("my-new-id", deviceType));

        await Eventually.AssertAsync(async () =>
        {
            var client = factory.CreateAuthenticatedClient();
            var list = await client.GetDevicesAsync();
            Assert.Contains(list.Items, m => m.ExternalId == "my-new-id" && m.DeviceType == deviceType);
        });
    }

    [Fact]
    public async Task WhenDeviceDiscoveredEventReceivedThenNetworkAddressIsAvailableFromTheApi()
    {
        await factory.PublishHausEventAsync(new DeviceDiscoveredEvent("network-address-id", NetworkAddress: 0x9abc));

        await Eventually.AssertAsync(async () =>
        {
            var client = factory.CreateAuthenticatedClient();
            var list = await client.GetDevicesAsync();
            Assert.Contains(list.Items, m => m.ExternalId == "network-address-id" && m.NetworkAddress == 0x9abc);
        });
    }

    [Fact]
    public async Task WhenUnauthorizedThenReturnsUnauthorized()
    {
        var client = factory.CreateUnauthenticatedClient();

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetDevicesAsync());

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }
}
