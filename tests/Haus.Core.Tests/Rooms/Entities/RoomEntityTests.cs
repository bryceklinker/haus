using System;
using System.Threading.Tasks;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.DomainEvents;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Entities;

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

        var light = new DeviceEntity(deviceType: DeviceType.Light);
        room.AddDevice(light, new FakeDomainEventBus());

        Assert.Equal(room.Lighting?.State, light.Lighting?.State);
        Assert.Equal(room.Lighting?.Level, light.Lighting?.Level);
    }

    [Fact]
    public void WhenUpdatedFromModelThenNameIsUpdated()
    {
        var room = new RoomEntity();
        room.UpdateFromModel(new RoomModel(Name: "kitchen", OccupancyTimeoutInSeconds: 50));

        Assert.Equal("kitchen", room.Name);
        Assert.Equal(50, room.OccupancyTimeoutInSeconds);
    }

    [Fact]
    public void WhenDeviceRemovedFromRoomThenDeviceIsMissingFromDevices()
    {
        var device = new DeviceEntity();

        var room = new RoomEntity();
        room.AddDevice(device, new FakeDomainEventBus());
        room.RemoveDevice(device);

        Assert.Empty(room.Devices);
        Assert.Null(device.Room);
    }

    [Fact]
    public void WhenDeviceIsAlreadyInRoomThenAddingDeviceAgainDoesNothing()
    {
        var device = new DeviceEntity { Id = 65 };
        var room = new RoomEntity();
        room.AddDevice(device, new FakeDomainEventBus());

        room.AddDevice(device, new FakeDomainEventBus());

        Assert.Single(room.Devices);
    }

    [Fact]
    public void WhenCreatedThenLightingIsDefaulted()
    {
        var room = new RoomEntity();

        Assert.Equal(
            new LightingEntity(
                LightingDefaults.State,
                new LevelLightingEntity(),
                new TemperatureLightingEntity(),
                new ColorLightingEntity()
            ),
            room.Lighting
        );
    }

    [Fact]
    public void WhenCreatedFromModelThenRoomIsPopulatedFromTheModel()
    {
        var model = new RoomModel(Name: "living room", OccupancyTimeoutInSeconds: 90);

        var room = RoomEntity.CreateFromModel(model);

        Assert.Equal("living room", room.Name);
        Assert.Equal(90, room.OccupancyTimeoutInSeconds);
    }

    [Fact]
    public void WhenCreatedThenLightingLevelIsTreatedLikeAPercentage()
    {
        var room = RoomEntity.CreateFromModel(new RoomModel());

        Assert.Equal(0, room.Lighting?.Level?.Min);
        Assert.Equal(100, room.Lighting?.Level?.Max);
    }

    [Fact]
    public void WhenLightingIsChangedThenRoomLightingIsChanged()
    {
        var room = new RoomEntity();

        var lighting = new LightingEntity { State = LightingState.On };
        room.ChangeLighting(lighting, new FakeDomainEventBus());

        Assert.Equal(LightingState.On, room.Lighting?.State);
    }

    [Fact]
    public void WhenLightingIsChangedThenEachLightDeviceLightingIsChanged()
    {
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        var room = new RoomEntity();
        room.AddDevice(light, new FakeDomainEventBus());

        var lighting = new LightingEntity { State = LightingState.On };
        room.ChangeLighting(lighting, new FakeDomainEventBus());

        Assert.Equal(LightingState.On, light.Lighting?.State);
        Assert.Equal(room.Lighting?.Level, light.Lighting?.Level);
    }

    [Fact]
    public void WhenLightingIsChangedThenRoomLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var room = new RoomEntity();

        var lighting = new LightingEntity();
        room.ChangeLighting(lighting, domainEventBus);

        Assert.IsAssignableFrom<RoomLightingChangedDomainEvent>(Assert.Single(domainEventBus.GetEvents));
    }

    [Fact]
    public void WhenRoomIsTurnedOffThenLightingStateIsSetToOff()
    {
        var fakeDomainEventBus = new FakeDomainEventBus();
        var room = new RoomEntity();
        room.ChangeLighting(new LightingEntity(LightingState.On), fakeDomainEventBus);

        room.TurnOff(fakeDomainEventBus);

        Assert.Equal(LightingState.Off, room.Lighting?.State);
    }

    [Fact]
    public void WhenRoomIsTurnedOnThenLightingStateIsSetToOn()
    {
        var fakeDomainEventBus = new FakeDomainEventBus();
        var room = new RoomEntity();
        room.ChangeLighting(new LightingEntity(LightingState.Off), fakeDomainEventBus);

        room.TurnOn(fakeDomainEventBus);

        Assert.Equal(LightingState.On, room.Lighting?.State);
    }

    [Fact]
    public void WhenRoomContainsDevicesWithDifferentMinAndMaxLevelsWhenLightingIsChangedThenDeviceLevelIsSetBasedOnPercentLevelOfRoom()
    {
        var fakeDomainEventBus = new FakeDomainEventBus();
        var room = new RoomEntity();
        var device = new DeviceEntity
        {
            DeviceType = DeviceType.Light,
            Lighting = new LightingEntity(Level: new LevelLightingEntity(0, 0, 254)),
        };
        room.AddDevice(device, fakeDomainEventBus);

        room.ChangeLighting(new LightingEntity(Level: new LevelLightingEntity(50)), fakeDomainEventBus);

        Assert.Equal(new LevelLightingEntity(127, 0, 254), device.Lighting.Level);
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
        var room = new RoomEntity(34, $"{Guid.NewGuid()}", 80, lighting: lighting);

        var model = room.ToModel();

        Assert.Equal(34, model.Id);
        Assert.Equal(room.Name, model.Name);
        Assert.Equal(80, model.OccupancyTimeoutInSeconds);
        Assert.Equal(lighting.ToModel(), model.Lighting);
    }

    [Fact]
    public void WhenRoomBecomesOccupiedThenLightsAreTurnedOn()
    {
        var lighting = new LightingEntity(LightingState.Off);
        var room = new RoomEntity(12, $"{Guid.NewGuid()}", lighting: lighting);

        room.ChangeOccupancy(new OccupancyChangedModel("", true), new FakeDomainEventBus());

        Assert.Equal(LightingState.On, room.Lighting?.State);
    }

    [Fact]
    public void WhenRoomBecomesOccupiedThenRoomHasLastOccupiedTime()
    {
        var room = new RoomEntity(12, "");

        room.ChangeOccupancy(new OccupancyChangedModel("", true), new FakeDomainEventBus());

        Assert.True(
            Math.Abs((DateTime.UtcNow - room.LastOccupiedTime!.Value).Ticks) <= TimeSpan.FromMilliseconds(500).Ticks
        );
    }

    [Fact]
    public void WhenRoomIsNoLongerOccupiedBeforeOccupancyTimeoutThenRoomLightingIsUnchanged()
    {
        var lighting = new LightingEntity(LightingState.On);
        var room = new RoomEntity(12, "", 100, lighting: lighting);

        room.ChangeOccupancy(new OccupancyChangedModel("", true), new FakeDomainEventBus());
        room.ChangeOccupancy(new OccupancyChangedModel(""), new FakeDomainEventBus());

        Assert.Equal(LightingState.On, room.Lighting?.State);
    }

    [Fact]
    public async Task WhenRoomIsNoLongerOccupiedAndOccupancyTimeoutIsExceededThenRoomIsTurnedOff()
    {
        var lighting = new LightingEntity(LightingState.On);
        var room = new RoomEntity(12, "", 1, lighting: lighting);

        room.ChangeOccupancy(new OccupancyChangedModel("", true), new FakeDomainEventBus());
        await Task.Delay(TimeSpan.FromSeconds(2));
        room.ChangeOccupancy(new OccupancyChangedModel(""), new FakeDomainEventBus());

        Assert.Equal(LightingState.Off, room.Lighting?.State);
    }
}
