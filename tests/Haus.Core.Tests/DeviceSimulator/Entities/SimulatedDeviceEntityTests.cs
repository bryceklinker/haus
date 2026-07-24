using System;
using Haus.Core.Common.Entities;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Entities;

public class SimulatedDeviceEntityTests
{
    [Fact]
    public void WhenCreatedThenSimulatedMetadataIsAdded()
    {
        var model = new SimulatedDeviceModel(DeviceType: DeviceType.Light);

        var entity = SimulatedDeviceEntity.Create(model);

        Assert.False(string.IsNullOrWhiteSpace(entity.Id));
        Assert.Equal(DeviceType.Light, entity.DeviceType);
        Assert.Contains(new Metadata("simulated", "true"), entity.Metadata);
    }

    [Fact]
    public void WhenCreatedWithMetadataThenMetadataIsMappedToSimulatedDevice()
    {
        var model = new SimulatedDeviceModel(Metadata: [new MetadataModel("one", "three")]);

        var entity = SimulatedDeviceEntity.Create(model);

        Assert.Contains(new Metadata("one", "three"), entity.Metadata);
    }

    [Fact]
    public void WhenTurnedIntoDeviceDiscoveredThenDeviceDiscoveredIsPopulatedFromSimulatedDevice()
    {
        var entity = SimulatedDeviceEntity.Create(new SimulatedDeviceModel(DeviceType: DeviceType.Light));

        var model = entity.ToDeviceDiscoveredModel();

        Assert.Equal(entity.Id, model.Id);
        Assert.Equal(entity.DeviceType, model.DeviceType);
        Assert.Contains(model.Metadata, m => m.Key == "simulated" && m.Value == "true");
    }

    [Fact]
    public void WhenConvertedToModelThenReturnsSimulatedDeviceModel()
    {
        var entity = SimulatedDeviceEntity.Create(
            new SimulatedDeviceModel($"{Guid.NewGuid()}", DeviceType.Light, true, [new MetadataModel("one", "three")])
        );

        var model = entity.ToModel();

        Assert.Equal(entity.Id, model.Id);
        Assert.Equal(DeviceType.Light, model.DeviceType);
        Assert.False(model.IsOccupied);
        Assert.Equal(2, model.Metadata.Length);
        Assert.Contains(new MetadataModel("one", "three"), model.Metadata);
    }

    [Fact]
    public void WhenSimulatorIsLightAndConvertedToModelThenLightingIsInModel()
    {
        var entity = new SimulatedDeviceEntity(DeviceType: DeviceType.Light, Lighting: new LightingModel());

        var model = entity.ToModel();

        Assert.Equal(new LightingModel(), model.Lighting);
    }

    [Fact]
    public void WhenSimulatorIsMotionSensorAndOccupancyIsChangedThenIsOccupied()
    {
        var entity = new SimulatedDeviceEntity(DeviceType: DeviceType.MotionSensor).ChangeOccupancy();

        Assert.True(entity.IsOccupied);
    }

    [Fact]
    public void WhenSimulatorIsMotionSensorAndOccupiedAndOccupancyIsChangedThenIsVacant()
    {
        var entity = new SimulatedDeviceEntity(DeviceType: DeviceType.MotionSensor, IsOccupied: true).ChangeOccupancy();

        Assert.False(entity.IsOccupied);
    }
}
