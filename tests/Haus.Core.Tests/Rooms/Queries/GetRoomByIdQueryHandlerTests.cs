using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Queries;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Queries;

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

        actual.Name.Should().Be("hotel");
    }

    [Fact]
    public async Task WhenRoomIsMissingThenReturnsNull()
    {
        var actual = await _hausBus.ExecuteQueryAsync(new GetRoomByIdQuery(3234));

        actual.Should().BeNull();
    }
}
