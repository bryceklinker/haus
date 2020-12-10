using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Queries;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Queries
{
    public class GetDevicesInRoomQueryHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public GetDevicesInRoomQueryHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenGettingDevicesInRoomThenReturnsAllDevicesInRoom()
        {
            var room = _context.AddRoom("hello");
            room.AddDevice(_context.AddDevice("one"), new FakeDomainEventBus());
            room.AddDevice(_context.AddDevice("two"), new FakeDomainEventBus());
            room.AddDevice(_context.AddDevice("three"), new FakeDomainEventBus());
            await _context.SaveChangesAsync();
            
            var result = await _hausBus.ExecuteQueryAsync(new GetDevicesInRoomQuery(room.Id));

            Assert.Equal(3, result.Count);
            Assert.Contains(result.Items, d => d.ExternalId == "one");
            Assert.Contains(result.Items, d => d.ExternalId == "two");
            Assert.Contains(result.Items, d => d.ExternalId == "three");
        }

        [Fact]
        public async Task WhenRoomIsMissingThenReturnsNull()
        {
            var result = await _hausBus.ExecuteQueryAsync(new GetDevicesInRoomQuery(65));

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenDevicesAreNotAssignedToARoomThenExcludesUnassignedDevices()
        {
            var room = _context.AddRoom("hello");
            room.AddDevice(_context.AddDevice("one"), new FakeDomainEventBus());
            _context.AddDevice("unassigned");
            await _context.SaveChangesAsync();

            var result = await _hausBus.ExecuteQueryAsync(new GetDevicesInRoomQuery(room.Id));

            Assert.Equal(1, result.Count);
        }
    }
}