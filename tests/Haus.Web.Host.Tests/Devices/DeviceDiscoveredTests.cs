using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.ExternalMessages;
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
            var deviceType = DeviceType.LightSensor | DeviceType.MotionSensor | DeviceType.TemperatureSensor;
            await _factory.PublishHausEventAsync(new DeviceDiscoveredModel
            {
                Id = "my-new-id",
                Description = "I don't know",
                Model = "new hotness",
                Vendor = "Klinker",
                DeviceType = deviceType
            });
            
            await Eventually.AssertAsync(async () =>
            {
                var client = _factory.CreateAuthenticatedClient();
                var list = await client.GetDevicesAsync();
                Assert.Contains(list.Items, model => model.ExternalId == "my-new-id");
                Assert.Contains(list.Items, model => model.DeviceType == deviceType);
            });
        }

        [Fact]
        public async Task WhenExternalDevicesAreSyncedThenSyncExternalDevicesIsPublished()
        {
            HausCommand<SyncDiscoveryModel> command = null;
            await _factory.SubscribeToHausCommandsAsync<SyncDiscoveryModel>(cmd => command = cmd);

            var client = _factory.CreateAuthenticatedClient();
            await client.SyncDevicesAsync();

            Eventually.Assert(() =>
            {
                Assert.Equal(SyncDiscoveryModel.Type, command.Type);
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