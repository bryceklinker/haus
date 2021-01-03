using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Commands
{
    public class AddDevicesToRoomCommandHandlerTests
    {
        private readonly HausDbContext _context;
        private readonly IHausBus _hausBus;

        public AddDevicesToRoomCommandHandlerTests()
        {
            _context = HausDbContextFactory.Create();
            _hausBus = HausBusFactory.Create(_context);
        }

        [Fact]
        public async Task WhenRoomIdIsMissingFromDbThenThrowsEntityNotFoundException()
        {
            var command = new AddDevicesToRoomCommand(54);

            Func<Task> act = () => _hausBus.ExecuteCommandAsync(command);

            await act.Should().ThrowAsync<EntityNotFoundException<RoomEntity>>();
        }

        [Fact]
        public async Task WhenDeviceIdIsMissingFromDbThenThrowsEntityNotFoundException()
        {
            var room = _context.AddRoom();

            var command = new AddDevicesToRoomCommand(room.Id, 65);

            Func<Task> act = () => _hausBus.ExecuteCommandAsync(command);

            await act.Should().ThrowAsync<EntityNotFoundException<DeviceEntity>>();
        }

        [Fact]
        public async Task WhenSingleDeviceIsAddedToRoomThenRoomHasDevice()
        {
            var room = _context.AddRoom();
            var device = _context.AddDevice();

            var command = new AddDevicesToRoomCommand(room.Id, device.Id);
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

            var command = new AddDevicesToRoomCommand(room.Id, first.Id, second.Id, third.Id);
            await _hausBus.ExecuteCommandAsync(command);

            var updatedRoom = _context.GetRoomsIncludeDevices().Single();
            updatedRoom.Devices.Should().HaveCount(3)
                .And.Contain(first)
                .And.Contain(second)
                .And.Contain(third);
        }
    }
}