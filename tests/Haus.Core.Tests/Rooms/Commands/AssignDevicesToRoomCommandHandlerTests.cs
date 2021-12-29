using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Rooms.Events;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands;

public class AssignDevicesToRoomCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public AssignDevicesToRoomCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenRoomIdIsMissingFromDbThenThrowsEntityNotFoundException()
    {
        var command = new AssignDevicesToRoomCommand(54);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<EntityNotFoundException<RoomEntity>>();
    }

    [Fact]
    public async Task WhenDeviceIdIsMissingFromDbThenThrowsEntityNotFoundException()
    {
        var room = _context.AddRoom();

        var command = new AssignDevicesToRoomCommand(room.Id, 65);

        var act = () => _hausBus.ExecuteCommandAsync(command);

        await act.Should().ThrowAsync<EntityNotFoundException<DeviceEntity>>();
    }

    [Fact]
    public async Task WhenSingleDeviceIsAddedToRoomThenRoomHasDevice()
    {
        var room = _context.AddRoom();
        var device = _context.AddDevice();

        var command = new AssignDevicesToRoomCommand(room.Id, device.Id);
        await _hausBus.ExecuteCommandAsync(command);

        _context.GetRoomsIncludeDevices().Should()
            .HaveCount(1)
            .And.Contain(r => r.Devices.Contains(device));
    }

    [Fact]
    public async Task WhenMultipleDevicesAreAddedToRoomThenAllDevicesAreInRoom()
    {
        var room = _context.AddRoom();
        var first = _context.AddDevice();
        var second = _context.AddDevice();
        var third = _context.AddDevice();

        var command = new AssignDevicesToRoomCommand(room.Id, first.Id, second.Id, third.Id);
        await _hausBus.ExecuteCommandAsync(command);

        var updatedRoom = _context.GetRoomsIncludeDevices().Single();
        updatedRoom.Devices.Should().HaveCount(3)
            .And.Contain(first)
            .And.Contain(second)
            .And.Contain(third);
    }

    [Fact]
    public async Task WhenDevicesAreAssignedToRoomThenPublishesDevicesAssignedToRoomEvent()
    {
        var room = _context.AddRoom();
        var one = _context.AddDevice();
        var two = _context.AddDevice();

        var command = new AssignDevicesToRoomCommand(room.Id, one.Id, two.Id);
        await _hausBus.ExecuteCommandAsync(command);

        _hausBus.GetPublishedRoutableEvents<DevicesAssignedToRoomEvent>().Should().HaveCount(1);
    }

    [Fact]
    public async Task WhenDevicesAreAssignedToRoomThenPublishesDeviceLightingChangedEvent()
    {
        var room = _context.AddRoom();
        var light = _context.AddDevice(deviceType: DeviceType.Light);

        var command = new AssignDevicesToRoomCommand(room.Id, light.Id);
        await _hausBus.ExecuteCommandAsync(command);

        _hausBus.GetPublishedRoutableEvents<DeviceLightingChangedEvent>().Should().HaveCount(1);
    }
}