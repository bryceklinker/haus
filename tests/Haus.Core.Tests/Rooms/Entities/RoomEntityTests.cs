using Haus.Core.Common;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.DomainEvents;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using System.Linq;
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
            room.AddDevice(device, new FakeDomainEventBus());

            Assert.Contains(device, room.Devices);
            Assert.Equal(room, device.Room);
        }

        [Fact]
        public void WhenLightDeviceIsAddedToRoomThenLightingForDeviceIsSetToRoomLighting()
        {
            var room = new RoomEntity();
            var roomLighting = new Lighting {BrightnessPercent = 65};
            room.ChangeLighting(roomLighting, new FakeDomainEventBus());

            var light = new DeviceEntity {DeviceType = DeviceType.Light};
            room.AddDevice(light, new FakeDomainEventBus());

            Assert.Equal(roomLighting, light.Lighting);
        }

        [Fact]
        public void WhenUpdatedFromModelThenNameIsUpdated()
        {
            var room = new RoomEntity();
            room.UpdateFromModel(new RoomModel(name: "kitchen"));

            Assert.Equal("kitchen", room.Name);
        }

        [Fact]
        public void WhenDeviceRemovedFromRoomThenDeviceIsMissingFromDevices()
        {
            var device = new DeviceEntity();

            var room = new RoomEntity();
            room.AddDevice(device, new FakeDomainEventBus());
            room.RemoveDevice(device);

            Assert.DoesNotContain(device, room.Devices);
            Assert.Null(device.Room);
        }

        [Fact]
        public void WhenDeviceIsAlreadyInRoomThenAddingDeviceAgainDoesNothing()
        {
            var device = new DeviceEntity {Id = 65};
            var room = new RoomEntity();
            room.AddDevice(device, new FakeDomainEventBus());

            room.AddDevice(device, new FakeDomainEventBus());

            Assert.Single(room.Devices);
        }

        [Fact]
        public void WhenCreatedThenLightingIsDefaulted()
        {
            var room = new RoomEntity();

            Assert.Equal(Lighting.Default, room.Lighting);
        }

        [Fact]
        public void WhenCreatedFromModelThenRoomIsPopulatedFromTheModel()
        {
            var model = new RoomModel(name: "living room");

            var room = RoomEntity.CreateFromModel(model);

            Assert.Equal("living room", room.Name);
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingIsChanged()
        {
            var room = new RoomEntity();

            var lighting = new Lighting {State = LightingState.On};
            room.ChangeLighting(lighting, new FakeDomainEventBus());

            Assert.Equal(lighting, room.Lighting);
        }

        [Fact]
        public void WhenLightingIsChangedThenEachLightDeviceLightingIsChanged()
        {
            var light = new DeviceEntity {DeviceType = DeviceType.Light};
            var room = new RoomEntity();
            room.AddDevice(light, new FakeDomainEventBus());

            var lighting = new Lighting {State = LightingState.On};
            room.ChangeLighting(lighting, new FakeDomainEventBus());

            Assert.Equal(lighting, light.Lighting);
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingChangedEventIsQueued()
        {
            var domainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();

            var lighting = new Lighting();
            room.ChangeLighting(lighting, domainEventBus);

            Assert.Single(domainEventBus.GetEvents.OfType<RoomLightingChangedDomainEvent>());
        }

        [Fact]
        public void WhenRoomIsTurnedOffThenLightingStateIsSetToOff()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            room.ChangeLighting(new Lighting {State = LightingState.On, BrightnessPercent = 54}, fakeDomainEventBus);

            room.TurnOff(fakeDomainEventBus);

            Assert.Equal(LightingState.Off, room.Lighting.State);
            Assert.Equal(54, room.Lighting.BrightnessPercent);
        }

        [Fact]
        public void WhenRoomIsTurnedOnThenLightingStateIsSetToOn()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            room.ChangeLighting(new Lighting {State = LightingState.Off, BrightnessPercent = 54}, fakeDomainEventBus);

            room.TurnOn(fakeDomainEventBus);

            Assert.Equal(LightingState.On, room.Lighting.State);
            Assert.Equal(54, room.Lighting.BrightnessPercent);
        }
    }
}