using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Rooms.Queries;
using Haus.Core.Tests.Support;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Queries;

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

        result.Count.Should().Be(3);
        result
            .Items.Should()
            .HaveCount(3)
            .And.Contain(d => d.ExternalId == "one")
            .And.Contain(d => d.ExternalId == "two")
            .And.Contain(d => d.ExternalId == "three");
    }

    [Fact]
    public async Task WhenRoomIsMissingThenReturnsEmptyList()
    {
        var result = await _hausBus.ExecuteQueryAsync(new GetDevicesInRoomQuery(65));

        result.Should().BeEquivalentTo(new ListResult<DeviceModel>());
    }

    [Fact]
    public async Task WhenDevicesAreNotAssignedToARoomThenExcludesUnassignedDevices()
    {
        var room = _context.AddRoom("hello");
        room.AddDevice(_context.AddDevice("one"), new FakeDomainEventBus());
        _context.AddDevice("unassigned");
        await _context.SaveChangesAsync();

        var result = await _hausBus.ExecuteQueryAsync(new GetDevicesInRoomQuery(room.Id));

        result.Count.Should().Be(1);
    }
}
