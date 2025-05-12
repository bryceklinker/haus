using System;
using System.Linq;
using FluentAssertions;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Core.Rooms.Entities;
using Haus.Core.Tests.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Entities;

public class DeviceEntityTest
{
    [Fact]
    public void WhenCreatedFromDeviceDiscoveredThenEntityIsPopulatedFromDiscoveredDevice()
    {
        var model = new DeviceDiscoveredEvent(
            "this-id",
            DeviceType.MotionSensor,
            [new MetadataModel("Model", "some model"), new MetadataModel("Vendor", "Vendy")]
        );

        var entity = DeviceEntity.FromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.ExternalId.Should().Be("this-id");
        entity.DeviceType.Should().Be(DeviceType.MotionSensor);
        entity
            .Metadata.Should()
            .ContainEquivalentOf(new DeviceMetadataEntity("Vendor", "Vendy") { Device = entity })
            .And.ContainEquivalentOf(new DeviceMetadataEntity("Model", "some model") { Device = entity });
    }

    [Fact]
    public void WhenLightCreatedFromDeviceDiscoveredThenLightTypeIsLevel()
    {
        var model = new DeviceDiscoveredEvent(
            "this-id",
            DeviceType.Light,
            [new MetadataModel("Model", "some model"), new MetadataModel("Vendor", "Vendy")]
        );

        var entity = DeviceEntity.FromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.DeviceType.Should().Be(DeviceType.Light);
        entity.LightType.Should().Be(LightType.Level);
        entity.Lighting.Should().BeEquivalentTo(new LightingEntity(LightingDefaults.State, new LevelLightingEntity()));
    }

    [Fact]
    public void WhenCreatedFromDeviceDiscoveredThenNameIsSetToExternalId()
    {
        var model = new DeviceDiscoveredEvent("this-id");

        var entity = DeviceEntity.FromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.Name.Should().Be("this-id");
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceThenDeviceTypeIsUpdated()
    {
        var model = new DeviceDiscoveredEvent("", DeviceType.MotionSensor);

        var entity = new DeviceEntity();
        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.DeviceType.Should().Be(DeviceType.MotionSensor);
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceToLightThenLightTypeIsLevel()
    {
        var model = new DeviceDiscoveredEvent("", DeviceType.Light);
        var entity = new DeviceEntity();

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.DeviceType.Should().Be(DeviceType.Light);
        entity.LightType.Should().Be(LightType.Level);
        entity.Lighting.Should().BeEquivalentTo(new LightingEntity(LightingDefaults.State, new LevelLightingEntity()));
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceToLightWhenLightTypeIsAlreadySetThenLightTypeAndLightingAreUnchanged()
    {
        var model = new DeviceDiscoveredEvent("", DeviceType.Light);
        var entity = new DeviceEntity(
            deviceType: DeviceType.Light,
            lightType: LightType.Temperature,
            lighting: new LightingEntity(
                LightingDefaults.State,
                new LevelLightingEntity(),
                new TemperatureLightingEntity()
            )
        );

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.LightType.Should().Be(LightType.Temperature);
        entity
            .Lighting.Should()
            .BeEquivalentTo(
                new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), new TemperatureLightingEntity())
            );
    }

    [Fact]
    public void WhenLightIsUpdatedFromModelWithoutChangingLightTypeThenLightingIsUnmodified()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Temperature);
        var entity = new DeviceEntity(
            deviceType: DeviceType.Light,
            lightType: LightType.Temperature,
            lighting: new LightingEntity(
                LightingState.On,
                new LevelLightingEntity(45),
                new TemperatureLightingEntity(4500)
            )
        );

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity.LightType.Should().Be(LightType.Temperature);
        entity.Lighting?.State.Should().Be(LightingState.On);
        entity.Lighting?.Level.Should().BeEquivalentTo(new LevelLightingEntity(45));
        entity.Lighting?.Temperature.Should().BeEquivalentTo(new TemperatureLightingEntity(4500));
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceThenModelMetadataIsAdded()
    {
        var model = new DeviceDiscoveredEvent("", Metadata: [new MetadataModel("Model", "boom")]);
        var entity = new DeviceEntity();

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        entity.Metadata.Should().ContainEquivalentOf(new DeviceMetadataEntity("Model", "boom") { Device = entity });
    }

    [Fact]
    public void WhenDeviceHasModelAndUpdatedFromDiscoveredDeviceThenModelMetadataIsUpdated()
    {
        var model = new DeviceDiscoveredEvent("", Metadata: [new MetadataModel("Model", "boom")]);
        var entity = new DeviceEntity();
        entity.AddOrUpdateMetadata("Model", "old");

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        entity
            .Metadata.Should()
            .HaveCount(1)
            .And.ContainEquivalentOf(new DeviceMetadataEntity("Model", "boom") { Device = entity });
    }

    [Fact]
    public void WhenDeviceIsUpdatedFromModelThenDeviceMatchesModel()
    {
        var model = new DeviceModel { Name = "Somename", ExternalId = "dont-use-this" };
        var entity = new DeviceEntity();

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity.Name.Should().Be("Somename");
    }

    [Fact]
    public void WhenDeviceIsUpdatedFromModelThenMetadataForDeviceIsUpdated()
    {
        var model = new DeviceModel(Metadata: [new MetadataModel("one", "three"), new MetadataModel("three", "two")]);
        var entity = new DeviceEntity();
        entity.AddOrUpdateMetadata("one", "two");

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity
            .Metadata.Should()
            .HaveCount(2)
            .And.ContainEquivalentOf(new DeviceMetadataEntity("one", "three") { Device = entity })
            .And.ContainEquivalentOf(new DeviceMetadataEntity("three", "two") { Device = entity });
    }

    [Fact]
    public void WhenLightIsUpdatedToTemperatureLightingThenLightTypeIsUpdatedAndTemperatureLightingIsInitialized()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Temperature);

        var entity = new DeviceEntity(deviceType: DeviceType.Light);

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity.LightType.Should().Be(LightType.Temperature);
        entity
            .Lighting.Should()
            .BeEquivalentTo(
                new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), new TemperatureLightingEntity())
            );
    }

    [Fact]
    public void WhenLightIsUpdatedToColorLightingThenLightTypeIsUpdatedAndColorLightingIsInitialized()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Color);

        var entity = new DeviceEntity(deviceType: DeviceType.Light);

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity.LightType.Should().Be(LightType.Color);
        entity
            .Lighting.Should()
            .BeEquivalentTo(
                new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), Color: new ColorLightingEntity())
            );
    }

    [Fact]
    public void WhenLightIsUpdatedToTemperatureLightingThenCurrentLightingIsMaintained()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Temperature);

        var entity = new DeviceEntity(
            deviceType: DeviceType.Light,
            lightType: LightType.Level,
            lighting: new LightingEntity(LightingState.On, new LevelLightingEntity(45, 12, 95))
        );

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity.Lighting?.State.Should().Be(LightingState.On);
        entity.Lighting?.Level.Should().BeEquivalentTo(new LevelLightingEntity(45, 12, 95));
        entity.Lighting?.Temperature.Should().BeEquivalentTo(new TemperatureLightingEntity());
    }

    [Fact]
    public void WhenLightIsUpdatedToColorLightingThenCurrentLightingIsMaintained()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Color);

        var entity = new DeviceEntity(
            deviceType: DeviceType.Light,
            lightType: LightType.Level,
            lighting: new LightingEntity(LightingState.On, new LevelLightingEntity(45, 12, 95))
        );

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        entity.Lighting?.State.Should().Be(LightingState.On);
        entity.Lighting?.Level.Should().BeEquivalentTo(new LevelLightingEntity(45, 12, 95));
        entity.Lighting?.Color.Should().BeEquivalentTo(new ColorLightingEntity());
    }

    [Fact]
    public void WhenLightIsAssignedToRoomAndLightTypeIsUpdatedThenLightingIsSynchronizedToRoomLighting()
    {
        var room = new RoomEntity(
            4,
            "",
            0,
            lighting: new LightingEntity(
                LightingState.On,
                new LevelLightingEntity(88),
                new TemperatureLightingEntity(4500),
                new ColorLightingEntity(45, 45, 45)
            )
        );
        var light = new DeviceEntity(deviceType: DeviceType.Light, room: room);

        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Temperature);
        light.UpdateFromModel(model, new FakeDomainEventBus());

        light.Lighting?.State.Should().Be(LightingState.On);
        light.Lighting?.Level.Should().BeEquivalentTo(room.Lighting?.Level);
        light.Lighting?.Temperature.Should().BeEquivalentTo(room.Lighting?.Temperature);
    }

    [Fact]
    public void WhenLightIsDiscoveredThenLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var model = new DeviceDiscoveredEvent($"{Guid.NewGuid()}", DeviceType.Light);

        DeviceEntity.FromDiscoveredDevice(model, domainEventBus);

        domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>().Should().HaveCount(1);
    }

    [Fact]
    public void WhenLightTypeIsChangedThenLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Color);
        var entity = new DeviceEntity(deviceType: DeviceType.Light, lightType: LightType.Level);

        entity.UpdateFromModel(model, domainEventBus);

        domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>().Should().HaveCount(1);
    }

    [Fact]
    public void WhenDeviceLightingChangedThenDeviceLightingChangedEventQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        var lighting = new LightingEntity(LightingState.On);

        light.ChangeLighting(lighting, domainEventBus);

        domainEventBus.GetEvents.Should().HaveCount(1).And.ContainItemsAssignableTo<DeviceLightingChangedDomainEvent>();
    }

    [Fact]
    public void WhenLightingChangedForLevelOnlyLightThenDeviceLightingChangedEventIsMissingTemperatureAndColor()
    {
        var domainEventBus = new FakeDomainEventBus();
        var light = new DeviceEntity(
            deviceType: DeviceType.Light,
            lightType: LightType.Level,
            lighting: new LightingEntity(LightingState.Off, new LevelLightingEntity(90))
        );
        var lighting = new LightingEntity(
            LightingState.On,
            new LevelLightingEntity(50),
            new TemperatureLightingEntity(4500),
            new ColorLightingEntity(20, 20, 20)
        );

        light.ChangeLighting(lighting, domainEventBus);

        var events = domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>().ToList();
        events.Should().HaveCount(1);
        events[0].Device.Should().Be(light);
        events[0].Lighting?.State.Should().Be(LightingState.On);
        events[0].Lighting?.Level.Should().BeEquivalentTo(new LevelLightingEntity(50));
        events[0].Lighting?.Color.Should().BeNull();
        events[0].Lighting?.Temperature.Should().BeNull();
    }

    [Fact]
    public void WhenDeviceIsTurnedOffThenDeviceLightingStateIsSetToOff()
    {
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        var lighting = new LightingEntity(LightingState.On);
        light.ChangeLighting(lighting, new FakeDomainEventBus());

        light.TurnOff(new FakeDomainEventBus());

        light.Lighting?.State.Should().Be(LightingState.Off);
    }

    [Fact]
    public void WhenDeviceIsTurnedOnThenDeviceLightingStateIsSetToOn()
    {
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        light.ChangeLighting(new LightingEntity(LightingState.Off), new FakeDomainEventBus());

        light.TurnOn(new FakeDomainEventBus());

        light.Lighting?.State.Should().Be(LightingState.On);
    }

    [Fact]
    public void WhenDeviceIsNotALightThenChangeLightingThrowsInvalidOperation()
    {
        var device = new DeviceEntity();

        var act = () => device.ChangeLighting(new LightingEntity(), new FakeDomainEventBus());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void WhenConvertedToModelThenModelIsPopulatedFromDevice()
    {
        var lighting = new LightingEntity(
            LightingState.On,
            new LevelLightingEntity(32, 25, 45),
            new TemperatureLightingEntity(12, 0, 2000),
            new ColorLightingEntity(23, 12, 89)
        );
        var metadata = new[] { new DeviceMetadataEntity("one", "two") };
        var device = new DeviceEntity(
            12,
            $"{Guid.NewGuid()}",
            $"{Guid.NewGuid()}",
            DeviceType.Light,
            LightType.Level,
            new RoomEntity(89, "ignore"),
            lighting,
            metadata
        );

        var model = device.ToModel();

        model.Id.Should().Be(12);
        model.ExternalId.Should().Be(device.ExternalId);
        model.Name.Should().Be(device.Name);
        model.DeviceType.Should().Be(DeviceType.Light);
        model.LightType.Should().Be(LightType.Level);
        model.RoomId.Should().Be(89);
        model.Lighting.Should().BeEquivalentTo(lighting.ToModel());
        model.Metadata.Should().HaveCount(1).And.ContainEquivalentOf(metadata[0].ToModel());
    }

    [Fact]
    public void WhenDeviceUpdatedFromLightingConstraintsThenLightingMinAndMaxLevelsAreUpdated()
    {
        var device = new DeviceEntity(deviceType: DeviceType.Light);
        var model = new LightingConstraintsModel(1, 254);

        device.UpdateFromLightingConstraints(model, new FakeDomainEventBus());

        device.Lighting?.Level?.Min.Should().Be(1);
        device.Lighting?.Level?.Max.Should().Be(254);
    }

    [Fact]
    public void WhenDeviceUpdatedFromLightingConstraintsThenDeviceLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var device = new DeviceEntity(deviceType: DeviceType.Light);
        var model = new LightingConstraintsModel(1, 254);

        device.UpdateFromLightingConstraints(model, domainEventBus);

        domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>().Should().HaveCount(1);
    }
}
