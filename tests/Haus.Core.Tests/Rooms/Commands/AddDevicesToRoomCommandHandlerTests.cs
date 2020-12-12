using System.Linq;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Commands;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Microsoft.EntityFrameworkCore;
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

            await Assert.ThrowsAsync<EntityNotFoundException<RoomEntity>>(() => _hausBus.ExecuteCommandAsync(command));
        }

        [Fact]
        public async Task WhenDeviceIdIsMissingFromDbThenThrowsEntityNotFoundException()
        {
            var room = _context.AddRoom();

            var command = new AddDevicesToRoomCommand(room.Id, 65);

            await Assert.ThrowsAsync<EntityNotFoundException<DeviceEntity>>(() =>
                _hausBus.ExecuteCommandAsync(command));
        }

        [Fact]
        public async Task WhenSingleDeviceIsAddedToRoomThenRoomHasDevice()
        {
            var room = _context.AddRoom();
            var device = _context.AddDevice();

            var command = new AddDevicesToRoomCommand(room.Id, device.Id);
            await _hausBus.ExecuteCommandAsync(command);

            var updatedRoom = _context.GetRoomsIncludeDevices().Single();

            Assert.Single(updatedRoom.Devices);
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

            Assert.Equal(3, updatedRoom.Devices.Count);
        }
    }
}