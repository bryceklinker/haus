using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Polly;
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
            var httpClient = _factory.CreateAuthenticatedClient();
            await mqttClient.PublishAsync("haus/events", new DeviceDiscoveredModel
            {
                Id = "hello"
            }.AsHausEvent());

            await Eventually.AssertAsync(async () =>
            {
                var result = await httpClient.GetFromJsonAsync<ListResult<DeviceModel>>("/api/devices?externalId=hello");
                var device = result.Items[0];
                await httpClient.PutAsJsonAsync($"/api/devices/{device.Id}", new DeviceModel
                {
                    Name = "some-name"
                });

                var response = await httpClient.GetAsync($"/api/devices/{device.Id}");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }
    }
}