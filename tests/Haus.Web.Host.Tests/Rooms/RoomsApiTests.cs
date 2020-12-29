using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Rooms
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class RoomsApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;
        private readonly IHausApiClient _apiClient;

        public RoomsApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _apiClient = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenRoomCreatedThenRoomIsAvailableFromGetApi()
        {
            await CreateRoomAsync("Johnny");

            var result = await _apiClient.GetRoomsAsync();
            Assert.Contains(result.Items, r => r.Name == "Johnny");
        }

        [Fact]
        public async Task WhenRoomCreatedThenReturnsLocationOfRoom()
        {
            var response = await _apiClient.CreateRoomAsync(new RoomModel {Name = "something"});

            var result = await response.Content.ReadFromJsonAsync<RoomModel>();
            Assert.Equal($"{_apiClient.BaseUrl}/rooms/{result.Id}", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task WhenRoomUpdatedThenGettingRoomReturnsUpdatedRoom()
        {
            var room = await CreateRoomAsync("old");

            var updateResponse = await _apiClient.UpdateRoomAsync(room.Id, new RoomModel {Name = "new hotness"});
            updateResponse.EnsureSuccessStatusCode();

            var updated = await _apiClient.GetRoomAsync(room.Id);

            Assert.Equal("new hotness", updated.Name);
        }

        [Fact]
        public async Task WhenDeviceAddedToRoomThenRoomHasDevice()
        {
            var room = await CreateRoomAsync("room with devices");
            var device = await _factory.WaitForDeviceToBeDiscovered();
            await _apiClient.AddDevicesToRoomAsync(room.Id, device.Id);

            var result = await _apiClient.GetDevicesInRoomAsync(room.Id);
            Assert.Equal(1, result.Count);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task WhenRoomLightingIsSetThenRoomLightingEventPublishedToMqtt()
        {
            HausCommand<RoomLightingChangedEvent> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(msg => hausCommand = msg);
            
            var room = await CreateRoomAsync("room");
            await _apiClient.ChangeRoomLightingAsync(room.Id, new LightingModel {State = LightingState.On});
            
            Eventually.Assert(() =>
            {
                Assert.Equal(RoomLightingChangedEvent.Type, hausCommand.Type);
            });
        }

        [Fact]
        public async Task WhenRoomIsTurnedOffThenRoomLightingEventPublishedWithStateOff()
        {
            HausCommand<RoomLightingChangedEvent> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(msg => hausCommand = msg);
            
            var room = await CreateRoomAsync("turn-off");
            await _apiClient.TurnRoomOffAsync(room.Id);
            
            Eventually.Assert(() =>
            {
                Assert.Equal(RoomLightingChangedEvent.Type, hausCommand.Type);
                Assert.Equal(LightingState.Off, hausCommand.Payload.Lighting.State);
            });
        }
        
        [Fact]
        public async Task WhenRoomIsTurnedOnThenRoomLightingEventPublishedWithStateOn()
        {
            HausCommand<RoomLightingChangedEvent> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<RoomLightingChangedEvent>(msg => hausCommand = msg);
            
            var room = await CreateRoomAsync("turn-on");
            await _apiClient.TurnRoomOnAsync(room.Id);
            
            Eventually.Assert(() =>
            {
                Assert.Equal(RoomLightingChangedEvent.Type, hausCommand.Type);
                Assert.Equal(LightingState.On, hausCommand.Payload.Lighting.State);
            });
        }

        [Fact]
        public async Task WhenUnauthenticatedThenRoomsRequestsAreRejected()
        {
            var client = _factory.CreateUnauthenticatedClient();

            var response = await client.CreateRoomAsync(new RoomModel {Name = "something"});

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<RoomModel> CreateRoomAsync(string name)
        {
            var createResponse = await _apiClient.CreateRoomAsync(new RoomModel {Name = name});
            return await createResponse.Content.ReadFromJsonAsync<RoomModel>();
        }
    }
}