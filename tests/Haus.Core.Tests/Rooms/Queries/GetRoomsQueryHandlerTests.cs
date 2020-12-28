using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Queries;
using Haus.Core.Tests.Support;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Queries
{
    public class GetRoomsQueryHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public GetRoomsQueryHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenRetrievingAllRoomsThenReturnsAllRoomsFromTheDatabase()
        {
            _context.AddRoom("three");
            _context.AddRoom("hello");
            _context.AddRoom("bob");

            var result = await _hausBus.ExecuteQueryAsync(new GetRoomsQuery());

            Assert.Equal(3, result.Count);
            Assert.Contains(result.Items, r => r.Name == "three");
            Assert.Contains(result.Items, r => r.Name == "hello");
            Assert.Contains(result.Items, r => r.Name == "bob");
        }
    }
}