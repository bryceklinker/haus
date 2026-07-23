using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Common.Storage.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Common.Storage.Commands;

public class ClearDatabaseCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly IHausBus _hausBus;

    public ClearDatabaseCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.Create(_context);
    }

    [Fact]
    public async Task WhenClearDatabaseExecutedThenAllDeviceMetadataIsDeleted()
    {
        _context.AddDevice(configure: config => config.AddOrUpdateMetadata("stuff", "here"));
        _context.AddDevice(configure: config => config.AddOrUpdateMetadata("stuff", "here"));
        _context.AddDevice(configure: config => config.AddOrUpdateMetadata("stuff", "here"));

        await _hausBus.ExecuteCommandAsync(new ClearDatabaseCommand());

        Assert.Empty(_context.Set<DeviceMetadataEntity>());
    }

    [Fact]
    public async Task WhenClearDatabaseExecutedThenAllDevicesAreRemoved()
    {
        _context.AddDevice();
        _context.AddDevice();
        _context.AddDevice();

        await _hausBus.ExecuteCommandAsync(new ClearDatabaseCommand());

        Assert.Empty(_context.Set<DeviceEntity>());
    }

    [Fact]
    public async Task WhenClearDatabaseExecutedThenAllRoomsAreRemoved()
    {
        _context.AddRoom();
        _context.AddRoom();
        _context.AddRoom();

        await _hausBus.ExecuteCommandAsync(new ClearDatabaseCommand());

        Assert.Empty(_context.Set<RoomEntity>());
    }
}
