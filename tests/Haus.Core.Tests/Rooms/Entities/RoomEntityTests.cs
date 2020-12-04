using Haus.Core.Devices.Entities;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Xunit;

namespace Haus.Core.Tests.Rooms.Entities
{
    public class RoomEntityTests
    {
        [Fact]
        public void WhenDeviceIsAddedToRoomThenDeviceIsInRoom()
        {
            var device = new DeviceEntity();

            var room = new RoomEntity();
            room.AddDevice(device);

            Assert.Contains(device, room.Devices);
            Assert.Equal(room, device.Room);
        }

        [Fact]
        public void WhenUpdatedFromModelThenNameIsUpdated()
        {
            var room = new RoomEntity();
            room.UpdateFromModel(new RoomModel {Name = "kitchen"});

            Assert.Equal("kitchen", room.Name);
        }

        [Fact]
        public void WhenDeviceRemovedFromRoomThenDeviceIsMissingFromDevices()
        {
            var device = new DeviceEntity();
            
            var room = new RoomEntity();
            room.AddDevice(device);
            room.RemoveDevice(device);

            Assert.DoesNotContain(device, room.Devices);
            Assert.Null(device.Room);
        }

        [Fact]
        public void WhenCreatedFromModelThenRoomIsPopulatedFromTheModel()
        {
            var model = new RoomModel {Name = "living room"};

            var room = RoomEntity.CreateFromModel(model);

            Assert.Equal("living room", room.Name);
        }
    }
}