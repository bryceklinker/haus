using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DevicesApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;

        public DevicesApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenADeviceIsUpdatedThenUpdatedDeviceIsAvailableFromTheApi()
        {
            var mqttClient = await _factory.GetMqttClient();
            var hausClient = _factory.CreateAuthenticatedClient();
            await mqttClient.PublishAsync("haus/events", new DeviceDiscoveredModel
            {
                Id = "hello"
            }.AsHausEvent());

            await Eventually.AssertAsync(async () =>
            {
                var result = await hausClient.GetDevicesAsync("hello");
                var device = result.Items[0];
                await hausClient.UpdateDeviceAsync(device.Id, new DeviceModel
                {
                    Name = "some-name"
                });

                var updated = await hausClient.GetDeviceAsync(device.Id);
                Assert.Equal("some-name", updated.Name);
            });
        }
    }
}