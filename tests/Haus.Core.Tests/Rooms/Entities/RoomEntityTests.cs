using Haus.Core.Common;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.DomainEvents;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using System.Linq;
using FluentAssertions;
using Haus.Core.Models.Rooms.Events;
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

            room.Devices.Should().Contain(device);
            device.Room.Should().Be(room);
        }

        [Fact]
        public void WhenLightDeviceIsAddedToRoomThenLightingForDeviceIsSetToRoomLighting()
        {
            var room = new RoomEntity();
            var roomLighting = new Lighting {BrightnessPercent = 65};
            room.ChangeLighting(roomLighting, new FakeDomainEventBus());

            var light = new DeviceEntity {DeviceType = DeviceType.Light};
            room.AddDevice(light, new FakeDomainEventBus());

            light.Lighting.Should().BeEquivalentTo(roomLighting);
        }

        [Fact]
        public void WhenUpdatedFromModelThenNameIsUpdated()
        {
            var room = new RoomEntity();
            room.UpdateFromModel(new RoomModel(Name: "kitchen"));

            room.Name.Should().Be("kitchen");
        }

        [Fact]
        public void WhenDeviceRemovedFromRoomThenDeviceIsMissingFromDevices()
        {
            var device = new DeviceEntity();

            var room = new RoomEntity();
            room.AddDevice(device, new FakeDomainEventBus());
            room.RemoveDevice(device);

            room.Devices.Should().BeEmpty();
            device.Room.Should().BeNull();
        }

        [Fact]
        public void WhenDeviceIsAlreadyInRoomThenAddingDeviceAgainDoesNothing()
        {
            var device = new DeviceEntity {Id = 65};
            var room = new RoomEntity();
            room.AddDevice(device, new FakeDomainEventBus());

            room.AddDevice(device, new FakeDomainEventBus());

            room.Devices.Should().HaveCount(1);
        }

        [Fact]
        public void WhenCreatedThenLightingIsDefaulted()
        {
            var room = new RoomEntity();

            room.Lighting.Should().BeEquivalentTo(Lighting.Default);
        }

        [Fact]
        public void WhenCreatedFromModelThenRoomIsPopulatedFromTheModel()
        {
            var model = new RoomModel(Name: "living room");

            var room = RoomEntity.CreateFromModel(model);

            room.Name.Should().Be("living room");
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingIsChanged()
        {
            var room = new RoomEntity();

            var lighting = new Lighting {State = LightingState.On};
            room.ChangeLighting(lighting, new FakeDomainEventBus());

            room.Lighting.Should().BeEquivalentTo(lighting);
        }

        [Fact]
        public void WhenLightingIsChangedThenEachLightDeviceLightingIsChanged()
        {
            var light = new DeviceEntity {DeviceType = DeviceType.Light};
            var room = new RoomEntity();
            room.AddDevice(light, new FakeDomainEventBus());

            var lighting = new Lighting {State = LightingState.On};
            room.ChangeLighting(lighting, new FakeDomainEventBus());

            light.Lighting.Should().BeEquivalentTo(lighting);
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingChangedEventIsQueued()
        {
            var domainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();

            var lighting = new Lighting();
            room.ChangeLighting(lighting, domainEventBus);

            domainEventBus.GetEvents.Should().HaveCount(1)
                .And.ContainItemsAssignableTo<RoomLightingChangedDomainEvent>();
        }

        [Fact]
        public void WhenRoomIsTurnedOffThenLightingStateIsSetToOff()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            room.ChangeLighting(new Lighting {State = LightingState.On, BrightnessPercent = 54}, fakeDomainEventBus);

            room.TurnOff(fakeDomainEventBus);

            room.Lighting.State.Should().Be(LightingState.Off);
            room.Lighting.BrightnessPercent.Should().Be(54);
        }

        [Fact]
        public void WhenRoomIsTurnedOnThenLightingStateIsSetToOn()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            room.ChangeLighting(new Lighting {State = LightingState.Off, BrightnessPercent = 54}, fakeDomainEventBus);

            room.TurnOn(fakeDomainEventBus);

            room.Lighting.State.Should().Be(LightingState.On);
            room.Lighting.BrightnessPercent.Should().Be(54);
        }
    }
}