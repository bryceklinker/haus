using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Queries;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Queries
{
    public class GetRoomByIdQueryHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public GetRoomByIdQueryHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenRoomWithIdExistsThenReturnsRoom()
        {
            var existing = _context.AddRoom("hotel");

            var actual = await _hausBus.ExecuteQueryAsync(new GetRoomByIdQuery(existing.Id));

            Assert.Equal("hotel", actual.Name);
        }
    }
}