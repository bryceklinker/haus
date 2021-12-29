using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Queries;

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

        result.Count.Should().Be(3);
        result.Items.Should().HaveCount(3)
            .And.Contain(r => r.Name == "three")
            .And.Contain(r => r.Name == "hello")
            .And.Contain(r => r.Name == "bob");
    }
}