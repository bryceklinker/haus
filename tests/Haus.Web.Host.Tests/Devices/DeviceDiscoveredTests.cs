using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
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
            var mqttClient = await _factory.GetMqttClient();
            await mqttClient.PublishAsync("haus/events", new DeviceDiscoveredModel
            {
                Id = "my-new-id",
                Description = "I don't know",
                Model = "new hotness",
                Vendor = "Klinker"
            }.AsHausEvent());
            
            await Eventually.AssertAsync(async () =>
            {
                var client = _factory.CreateAuthenticatedClient();
                var list = await client.GetFromJsonAsync<ListResult<DeviceModel>>("/api/devices");
                Assert.Contains(list.Items, model => model.ExternalId == "my-new-id");
            });
        }

        [Fact]
        public async Task WhenUnauthorizedThenReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/devices");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}