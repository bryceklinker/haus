using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Rooms;
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
            var device = await WaitForDeviceToBeDiscovered("one");
            await _apiClient.AddDevicesToRoomAsync(room.Id, device.Id);

            var result = await _apiClient.GetDevicesInRoomAsync(room.Id);
            Assert.Equal(1, result.Count);
            Assert.Single(result.Items);
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

        private async Task<DeviceModel> WaitForDeviceToBeDiscovered(string deviceName)
        {
            var externalId = $"{Guid.NewGuid()}";
            await _factory.PublishHausEventAsync(new DeviceDiscoveredModel { Id = externalId });
            return await WaitFor.ResultAsync(async () =>
            {
                var devices = await _apiClient.GetDevicesAsync(externalId);
                return devices.Items.Single();
            });
        }
    }
}