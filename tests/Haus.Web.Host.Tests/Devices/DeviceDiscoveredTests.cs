using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
            await _factory.PublishHausEventAsync(new DeviceDiscoveredModel
            {
                Id = "my-new-id",
                Description = "I don't know",
                Model = "new hotness",
                Vendor = "Klinker"
            });
            
            await Eventually.AssertAsync(async () =>
            {
                var client = _factory.CreateAuthenticatedClient();
                var list = await client.GetDevicesAsync();
                Assert.Contains(list.Items, model => model.ExternalId == "my-new-id");
            });
        }

        [Fact]
        public async Task WhenUnauthorizedThenReturnsUnauthorized()
        {
            var client = _factory.CreateUnauthenticatedClient(); 
            
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetDevicesAsync());
            
            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }
    }
}