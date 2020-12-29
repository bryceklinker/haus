using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.DeviceSimulator;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.DeviceSimulator
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DeviceSimulatorApiTests
    {
        private readonly IHausApiClient _client;
        private readonly HausWebHostApplicationFactory _factory;

        public DeviceSimulatorApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenSimulatedDeviceIsAddedThenPublishesDeviceDiscoveredEvent()
        {
            DeviceDiscoveredModel discoveredModel = null;
            await _factory.SubscribeToHausEventsAsync<DeviceDiscoveredModel>(evt => discoveredModel = evt.Payload);

            await _client.AddSimulatedDevice(new CreateSimulatedDeviceModel
            {
                DeviceType = DeviceType.Light
            });

            Eventually.Assert(() =>
            {
                discoveredModel.DeviceType.Should().Be(DeviceType.Light);
            });
        }
    }
}