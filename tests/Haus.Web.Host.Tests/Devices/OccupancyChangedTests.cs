using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class OccupancyChangedTests
    {
        private readonly HausWebHostApplicationFactory _factory;
        private readonly IHausApiClient _apiClient;

        public OccupancyChangedTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _apiClient = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenOccupancyChangedForDeviceInRoomThenRoomLightingChangedPublished()
        {
            var sensor = await _factory.WaitForDeviceToBeDiscovered(DeviceType.MotionSensor);
            var response = await _apiClient.CreateRoomAsync(new RoomModel(Name: "home"));
            var room = await response.Content.ReadFromJsonAsync<RoomModel>();
            await _apiClient.AddDevicesToRoomAsync(room.Id, sensor.Id);

            HausCommand<RoomLightingChangedEvent> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(cmd => hausCommand = cmd);
            await _factory.PublishHausEventAsync(new OccupancyChangedModel(sensor.ExternalId, true));
            
            Eventually.Assert(() =>
            {
                Assert.Equal(room.Id, hausCommand.Payload.Room.Id);
                Assert.Equal(LightingState.On, hausCommand.Payload.Lighting.State);
            });
        }
    }
}