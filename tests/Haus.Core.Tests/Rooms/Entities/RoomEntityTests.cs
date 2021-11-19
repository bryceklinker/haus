using System;
using System.Collections.Generic;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.DomainEvents;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using FluentAssertions;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Lighting;
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

            var light = new DeviceEntity(deviceType: DeviceType.Light);
            room.AddDevice(light, new FakeDomainEventBus());

            light.Lighting.State.Should().Be(room.Lighting.State);
            light.Lighting.Level.Should().BeEquivalentTo(room.Lighting.Level);
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

            room.Lighting.Should().BeEquivalentTo(new LightingEntity(
                    LightingDefaults.State,
                    new LevelLightingEntity(),
                    new TemperatureLightingEntity(),
                    new ColorLightingEntity()
                )
            );
        }

        [Fact]
        public void WhenCreatedFromModelThenRoomIsPopulatedFromTheModel()
        {
            var model = new RoomModel(Name: "living room");

            var room = RoomEntity.CreateFromModel(model);

            room.Name.Should().Be("living room");
        }

        [Fact]
        public void WhenCreatedThenLightingLevelIsTreatedLikeAPercentage()
        {
            var room = RoomEntity.CreateFromModel(new RoomModel());

            room.Lighting.Level.Min.Should().Be(0);
            room.Lighting.Level.Max.Should().Be(100);
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingIsChanged()
        {
            var room = new RoomEntity();

            var lighting = new LightingEntity {State = LightingState.On};
            room.ChangeLighting(lighting, new FakeDomainEventBus());

            room.Lighting.State.Should().Be(LightingState.On);
        }

        [Fact]
        public void WhenLightingIsChangedThenEachLightDeviceLightingIsChanged()
        {
            var light = new DeviceEntity(deviceType: DeviceType.Light);
            var room = new RoomEntity();
            room.AddDevice(light, new FakeDomainEventBus());

            var lighting = new LightingEntity {State = LightingState.On};
            room.ChangeLighting(lighting, new FakeDomainEventBus());

            light.Lighting.State.Should().Be(LightingState.On);
            light.Lighting.Level.Should().BeEquivalentTo(room.Lighting.Level);
        }

        [Fact]
        public void WhenLightingIsChangedThenRoomLightingChangedEventIsQueued()
        {
            var domainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();

            var lighting = new LightingEntity();
            room.ChangeLighting(lighting, domainEventBus);

            domainEventBus.GetEvents.Should().HaveCount(1)
                .And.ContainItemsAssignableTo<RoomLightingChangedDomainEvent>();
        }

        [Fact]
        public void WhenRoomIsTurnedOffThenLightingStateIsSetToOff()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            room.ChangeLighting(new LightingEntity(LightingState.On), fakeDomainEventBus);

            room.TurnOff(fakeDomainEventBus);

            room.Lighting.State.Should().Be(LightingState.Off);
        }

        [Fact]
        public void WhenRoomIsTurnedOnThenLightingStateIsSetToOn()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            room.ChangeLighting(new LightingEntity(LightingState.Off), fakeDomainEventBus);

            room.TurnOn(fakeDomainEventBus);

            room.Lighting.State.Should().Be(LightingState.On);
        }

        [Fact]
        public void
            WhenRoomContainsDevicesWithDifferentMinAndMaxLevelsWhenLightingIsChangedThenDeviceLevelIsSetBasedOnPercentLevelOfRoom()
        {
            var fakeDomainEventBus = new FakeDomainEventBus();
            var room = new RoomEntity();
            var device = new DeviceEntity
            {
                DeviceType = DeviceType.Light,
                Lighting = new LightingEntity(level: new LevelLightingEntity(0, 0, 254))
            };
            room.AddDevice(device, fakeDomainEventBus);

            room.ChangeLighting(new LightingEntity(level: new LevelLightingEntity(50, 0, 100)), fakeDomainEventBus);

            device.Lighting.Level.Should().BeEquivalentTo(new LevelLightingEntity(127, 0, 254));
        }

        [Fact]
        public void WhenRoomConvertedToModelThenModelIsPopulatedFromRoom()
        {
            var lighting = new LightingEntity(
                LightingState.On,
                new LevelLightingEntity(54, 10, 90),
                new TemperatureLightingEntity(1000, 1000, 2000),
                new ColorLightingEntity(123, 124, 125)
            );
            var room = new RoomEntity(34, $"{Guid.NewGuid()}", lighting);

            var model = room.ToModel();

            model.Id.Should().Be(34);
            model.Name.Should().Be(room.Name);
            model.Lighting.Should().BeEquivalentTo(lighting.ToModel());
        }
    }
}