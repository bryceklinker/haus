using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Haus.Core.Models.Rooms;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Rooms
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class RoomsApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;

        public RoomsApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenRoomCreatedThenRoomIsAvailableFromGetApi()
        {
            var client = _factory.CreateAuthenticatedClient();
            await client.CreateRoomAsync(new RoomModel {Name = "Johnny"});

            var result = await client.GetRoomsAsync();
            Assert.Contains(result.Items, r => r.Name == "Johnny");
        }

        [Fact]
        public async Task WhenRoomCreatedThenReturnsLocationOfRoom()
        {
            var client = _factory.CreateAuthenticatedClient();
            var response = await client.CreateRoomAsync(new RoomModel {Name = "something"});

            var result = await response.Content.ReadFromJsonAsync<RoomModel>();
            Assert.Equal($"{client.BaseUrl}/rooms/{result.Id}", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task WhenRoomUpdatedThenGettingRoomReturnsUpdatedRoom()
        {
            var client = _factory.CreateAuthenticatedClient();
            var createResponse = await client.CreateRoomAsync(new RoomModel {Name = "old"});
            var room = await createResponse.Content.ReadFromJsonAsync<RoomModel>();

            var updateResponse = await client.UpdateRoomAsync(room.Id, new RoomModel {Name = "new hotness"});
            updateResponse.EnsureSuccessStatusCode();

            var updated = await client.GetRoomAsync(room.Id);

            Assert.Equal("new hotness", updated.Name);
        }

        [Fact]
        public async Task WhenUnauthenticatedThenRoomsRequestsAreRejected()
        {
            var client = _factory.CreateUnauthenticatedClient();

            var response = await client.CreateRoomAsync(new RoomModel {Name = "something"});

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}