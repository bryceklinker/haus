using Haus.Core.Common;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
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
        public void WhenDeviceIsAlreadyInRoomThenAddingDeviceAgainDoesNothing()
        {
            var device = new DeviceEntity{Id = 65};
            var room = new RoomEntity();
            room.AddDevice(device);

            room.AddDevice(device);

            Assert.Single(room.Devices);
        }

        [Fact]
        public void WhenCreatedFromModelThenRoomIsPopulatedFromTheModel()
        {
            var model = new RoomModel {Name = "living room"};

            var room = RoomEntity.CreateFromModel(model);

            Assert.Equal("living room", room.Name);
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingIsChanged()
        {
            var room = new RoomEntity();

            var lighting = new Lighting { State = LightingState.On };
            room.ChangeLighting(lighting);

            Assert.Equal(lighting, room.Lighting);
        }

        [Fact]
        public void WhenLightingIsChangedThenEachLightDeviceLightingIsChanged()
        {
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            var room = new RoomEntity();
            room.AddDevice(light);
            
            var lighting = new Lighting{State = LightingState.On};
            room.ChangeLighting(lighting);
            
            Assert.Equal(lighting, light.Lighting);
        }
    }
}