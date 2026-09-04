using System;
using System.Linq;
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

        Assert.Equal("this-id", entity.ExternalId);
        Assert.Equal(DeviceType.MotionSensor, entity.DeviceType);
        Assert.Contains(new DeviceMetadataEntity("Vendor", "Vendy") { Device = entity }, entity.Metadata);
        Assert.Contains(new DeviceMetadataEntity("Model", "some model") { Device = entity }, entity.Metadata);
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

        Assert.Equal(DeviceType.Light, entity.DeviceType);
        Assert.Equal(LightType.Level, entity.LightType);
        Assert.Equal(new LightingEntity(LightingDefaults.State, new LevelLightingEntity()), entity.Lighting);
    }

    [Fact]
    public void WhenCreatedFromDeviceDiscoveredThenNameIsSetToExternalId()
    {
        var model = new DeviceDiscoveredEvent("this-id");

        var entity = DeviceEntity.FromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal("this-id", entity.Name);
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceThenDeviceTypeIsUpdated()
    {
        var model = new DeviceDiscoveredEvent("", DeviceType.MotionSensor);

        var entity = new DeviceEntity();
        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal(DeviceType.MotionSensor, entity.DeviceType);
    }

    [Fact]
    public void WhenCreatedFromDeviceDiscoveredThenNetworkAddressIsSet()
    {
        var model = new DeviceDiscoveredEvent("this-id", NetworkAddress: 0x9abc);

        var entity = DeviceEntity.FromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal((ushort)0x9abc, entity.NetworkAddress);
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceThenNetworkAddressIsUpdated()
    {
        var model = new DeviceDiscoveredEvent("", NetworkAddress: 0x1234);
        var entity = new DeviceEntity();

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal((ushort)0x1234, entity.NetworkAddress);
    }

    [Fact]
    public void WhenCreatedFromDeviceDiscoveredThenEndpointIdIsSet()
    {
        var model = new DeviceDiscoveredEvent("this-id", EndpointId: 0x0b);

        var entity = DeviceEntity.FromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal((byte)0x0b, entity.EndpointId);
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceThenEndpointIdIsUpdated()
    {
        var model = new DeviceDiscoveredEvent("", EndpointId: 0x0b);
        var entity = new DeviceEntity();

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal((byte)0x0b, entity.EndpointId);
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceToLightThenLightTypeIsLevel()
    {
        var model = new DeviceDiscoveredEvent("", DeviceType.Light);
        var entity = new DeviceEntity();

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Equal(DeviceType.Light, entity.DeviceType);
        Assert.Equal(LightType.Level, entity.LightType);
        Assert.Equal(new LightingEntity(LightingDefaults.State, new LevelLightingEntity()), entity.Lighting);
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

        Assert.Equal(LightType.Temperature, entity.LightType);
        Assert.Equal(
            new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), new TemperatureLightingEntity()),
            entity.Lighting
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

        Assert.Equal(LightType.Temperature, entity.LightType);
        Assert.Equal(LightingState.On, entity.Lighting?.State);
        Assert.Equal(new LevelLightingEntity(45), entity.Lighting?.Level);
        Assert.Equal(new TemperatureLightingEntity(4500), entity.Lighting?.Temperature);
    }

    [Fact]
    public void WhenUpdatedFromDiscoveredDeviceThenModelMetadataIsAdded()
    {
        var model = new DeviceDiscoveredEvent("", Metadata: [new MetadataModel("Model", "boom")]);
        var entity = new DeviceEntity();

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Contains(new DeviceMetadataEntity("Model", "boom") { Device = entity }, entity.Metadata);
    }

    [Fact]
    public void WhenDeviceHasModelAndUpdatedFromDiscoveredDeviceThenModelMetadataIsUpdated()
    {
        var model = new DeviceDiscoveredEvent("", Metadata: [new MetadataModel("Model", "boom")]);
        var entity = new DeviceEntity();
        entity.AddOrUpdateMetadata("Model", "old");

        entity.UpdateFromDiscoveredDevice(model, new FakeDomainEventBus());

        Assert.Single(entity.Metadata);
        Assert.Contains(new DeviceMetadataEntity("Model", "boom") { Device = entity }, entity.Metadata);
    }

    [Fact]
    public void WhenDeviceIsUpdatedFromModelThenDeviceMatchesModel()
    {
        var model = new DeviceModel { Name = "Somename", ExternalId = "dont-use-this" };
        var entity = new DeviceEntity();

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        Assert.Equal("Somename", entity.Name);
    }

    [Fact]
    public void WhenDeviceIsUpdatedFromModelThenMetadataForDeviceIsUpdated()
    {
        var model = new DeviceModel(Metadata: [new MetadataModel("one", "three"), new MetadataModel("three", "two")]);
        var entity = new DeviceEntity();
        entity.AddOrUpdateMetadata("one", "two");

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        Assert.Equal(2, entity.Metadata.Count);
        Assert.Contains(new DeviceMetadataEntity("one", "three") { Device = entity }, entity.Metadata);
        Assert.Contains(new DeviceMetadataEntity("three", "two") { Device = entity }, entity.Metadata);
    }

    [Fact]
    public void WhenLightIsUpdatedToTemperatureLightingThenLightTypeIsUpdatedAndTemperatureLightingIsInitialized()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Temperature);

        var entity = new DeviceEntity(deviceType: DeviceType.Light);

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        Assert.Equal(LightType.Temperature, entity.LightType);
        Assert.Equal(
            new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), new TemperatureLightingEntity()),
            entity.Lighting
        );
    }

    [Fact]
    public void WhenLightIsUpdatedToColorLightingThenLightTypeIsUpdatedAndColorLightingIsInitialized()
    {
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Color);

        var entity = new DeviceEntity(deviceType: DeviceType.Light);

        entity.UpdateFromModel(model, new FakeDomainEventBus());

        Assert.Equal(LightType.Color, entity.LightType);
        Assert.Equal(
            new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), Color: new ColorLightingEntity()),
            entity.Lighting
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

        Assert.Equal(LightingState.On, entity.Lighting?.State);
        Assert.Equal(new LevelLightingEntity(45, 12, 95), entity.Lighting?.Level);
        Assert.Equal(new TemperatureLightingEntity(), entity.Lighting?.Temperature);
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

        Assert.Equal(LightingState.On, entity.Lighting?.State);
        Assert.Equal(new LevelLightingEntity(45, 12, 95), entity.Lighting?.Level);
        Assert.Equal(new ColorLightingEntity(), entity.Lighting?.Color);
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

        Assert.Equal(LightingState.On, light.Lighting?.State);
        Assert.Equal(room.Lighting.Level, light.Lighting?.Level);
        Assert.Equal(room.Lighting.Temperature, light.Lighting?.Temperature);
    }

    [Fact]
    public void WhenLightIsDiscoveredThenLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var model = new DeviceDiscoveredEvent($"{Guid.NewGuid()}", DeviceType.Light);

        DeviceEntity.FromDiscoveredDevice(model, domainEventBus);

        Assert.Single(domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>());
    }

    [Fact]
    public void WhenLightTypeIsChangedThenLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var model = new DeviceModel(DeviceType: DeviceType.Light, LightType: LightType.Color);
        var entity = new DeviceEntity(deviceType: DeviceType.Light, lightType: LightType.Level);

        entity.UpdateFromModel(model, domainEventBus);

        Assert.Single(domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>());
    }

    [Fact]
    public void WhenDeviceLightingChangedThenDeviceLightingChangedEventQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        var lighting = new LightingEntity(LightingState.On);

        light.ChangeLighting(lighting, domainEventBus);

        Assert.IsAssignableFrom<DeviceLightingChangedDomainEvent>(Assert.Single(domainEventBus.GetEvents));
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
        Assert.Single(events);
        Assert.Equal(light, events[0].Device);
        Assert.Equal(LightingState.On, events[0].Lighting.State);
        Assert.Equal(new LevelLightingEntity(50), events[0].Lighting.Level);
        Assert.Null(events[0].Lighting.Color);
        Assert.Null(events[0].Lighting.Temperature);
    }

    [Fact]
    public void WhenDeviceIsTurnedOffThenDeviceLightingStateIsSetToOff()
    {
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        var lighting = new LightingEntity(LightingState.On);
        light.ChangeLighting(lighting, new FakeDomainEventBus());

        light.TurnOff(new FakeDomainEventBus());

        Assert.Equal(LightingState.Off, light.Lighting?.State);
    }

    [Fact]
    public void WhenDeviceIsTurnedOnThenDeviceLightingStateIsSetToOn()
    {
        var light = new DeviceEntity(deviceType: DeviceType.Light);
        light.ChangeLighting(new LightingEntity(LightingState.Off), new FakeDomainEventBus());

        light.TurnOn(new FakeDomainEventBus());

        Assert.Equal(LightingState.On, light.Lighting?.State);
    }

    [Fact]
    public void WhenDeviceIsNotALightThenChangeLightingThrowsInvalidOperation()
    {
        var device = new DeviceEntity();

        var act = () => device.ChangeLighting(new LightingEntity(), new FakeDomainEventBus());

        Assert.Throws<InvalidOperationException>(act);
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
        var metadata = new[] { new DeviceMetadataEntity("one", "two") { Device = new DeviceEntity() } };
        var device = new DeviceEntity(
            12,
            $"{Guid.NewGuid()}",
            $"{Guid.NewGuid()}",
            DeviceType.Light,
            LightType.Level,
            new RoomEntity(89, "ignore"),
            lighting,
            metadata,
            networkAddress: 0x9abc,
            endpointId: 0x0b
        );

        var model = device.ToModel();

        Assert.Equal(12, model.Id);
        Assert.Equal(device.ExternalId, model.ExternalId);
        Assert.Equal(device.Name, model.Name);
        Assert.Equal(DeviceType.Light, model.DeviceType);
        Assert.Equal(LightType.Level, model.LightType);
        Assert.Equal(89, model.RoomId);
        Assert.Equal(lighting.ToModel(), model.Lighting);
        Assert.Equal(metadata[0].ToModel(), Assert.Single(model.Metadata));
        Assert.Equal((ushort)0x9abc, model.NetworkAddress);
        Assert.Equal((byte)0x0b, model.EndpointId);
    }

    [Fact]
    public void WhenDeviceUpdatedFromLightingConstraintsThenLightingMinAndMaxLevelsAreUpdated()
    {
        var device = new DeviceEntity(deviceType: DeviceType.Light);
        var model = new LightingConstraintsModel(1, 254);

        device.UpdateFromLightingConstraints(model, new FakeDomainEventBus());

        Assert.Equal(1, device.Lighting?.Level.Min);
        Assert.Equal(254, device.Lighting?.Level.Max);
    }

    [Fact]
    public void WhenDeviceUpdatedFromLightingConstraintsThenDeviceLightingChangedEventIsQueued()
    {
        var domainEventBus = new FakeDomainEventBus();
        var device = new DeviceEntity(deviceType: DeviceType.Light);
        var model = new LightingConstraintsModel(1, 254);

        device.UpdateFromLightingConstraints(model, domainEventBus);

        Assert.Single(domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>());
    }
}
