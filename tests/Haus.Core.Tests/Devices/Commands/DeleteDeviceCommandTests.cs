using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Commands;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands;

public class DeleteDeviceCommandTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public DeleteDeviceCommandTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDeleteDeviceCommandExecutedThenDeviceIsRemovedFromDatabase()
    {
        var device = _context.AddDevice();

        await _hausBus.ExecuteCommandAsync(new DeleteDeviceCommand(device.Id));

        var deleted = await _context.FindAsync<DeviceEntity>(device.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task WhenDeleteDeviceCommandExecutedThenDeviceMetadataIsRemovedFromDatabase()
    {
        var device = _context.AddDevice(configure: d => d.AddOrUpdateMetadata("Watts", "100"));

        await _hausBus.ExecuteCommandAsync(new DeleteDeviceCommand(device.Id));

        var remainingMetadata = _context.Set<DeviceMetadataEntity>().Where(m => m.Device.Id == device.Id);
        Assert.Empty(remainingMetadata);
    }

    [Fact]
    public async Task WhenDeletedDeviceBelongsToARoomThenDeviceIsRemovedFromRoomsDeviceCollection()
    {
        var room = _context.AddRoom();
        var device = _context.AddDevice(configure: d => d.Room = room);

        await _hausBus.ExecuteCommandAsync(new DeleteDeviceCommand(device.Id));

        var updatedRoom = await _context.FindByIdOrThrowAsync<RoomEntity>(room.Id, q => q.Include(r => r.Devices));
        Assert.DoesNotContain(updatedRoom.Devices, d => d.Id == device.Id);
    }

    [Fact]
    public async Task WhenDeletedDeviceDoesNotBelongToARoomThenDeviceIsDeletedSuccessfully()
    {
        var device = _context.AddDevice();

        await _hausBus.ExecuteCommandAsync(new DeleteDeviceCommand(device.Id));

        var deleted = await _context.FindAsync<DeviceEntity>(device.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task WhenDeleteDeviceCommandExecutedThenPublishesDeviceDeletedEvent()
    {
        var device = _context.AddDevice();

        await _hausBus.ExecuteCommandAsync(new DeleteDeviceCommand(device.Id));

        var published = Assert.Single(_hausBus.GetPublishedRoutableEvents<DeviceDeletedEvent>());
        Assert.Equal(device.Id, published.Payload.Device.Id);
    }

    [Fact]
    public async Task WhenDeleteDeviceCommandExecutedForMissingDeviceThenThrowsException()
    {
        var command = new DeleteDeviceCommand(-1);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await Assert.ThrowsAsync<EntityNotFoundException<DeviceEntity>>(act);
    }
}
