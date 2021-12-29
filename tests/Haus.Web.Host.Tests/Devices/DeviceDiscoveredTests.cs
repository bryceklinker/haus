using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices;

[Collection(HausWebHostCollectionFixture.Name)]
public class DeviceDiscoveredTests
{
    private readonly HausWebHostApplicationFactory _factory;

    public DeviceDiscoveredTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task WhenDeviceDiscoveredEventReceivedThenDeviceIsAvailableFromTheApi()
    {
        var deviceType = DeviceType.LightSensor | DeviceType.MotionSensor | DeviceType.TemperatureSensor;
        await _factory.PublishHausEventAsync(new DeviceDiscoveredEvent("my-new-id", deviceType));

        await Eventually.AssertAsync(async () =>
        {
            var client = _factory.CreateAuthenticatedClient();
            var list = await client.GetDevicesAsync();
            list.Items.Should()
                .Contain(m => m.ExternalId == "my-new-id" && m.DeviceType == deviceType);
        });
    }

    [Fact]
    public async Task WhenUnauthorizedThenReturnsUnauthorized()
    {
        var client = _factory.CreateUnauthenticatedClient();

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetDevicesAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}