using System;
using FluentAssertions;
using Haus.Core.Common.Entities;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Entities
{
    public class SimulatedDeviceEntityTests
    {
        [Fact]
        public void WhenCreatedThenSimulatedMetadataIsAdded()
        {
            var model = new SimulatedDeviceModel(DeviceType: DeviceType.Light);

            var entity = SimulatedDeviceEntity.Create(model);

            entity.Id.Should().NotBeNullOrWhiteSpace();
            entity.DeviceType.Should().Be(DeviceType.Light);
            entity.Metadata.Should().Contain(new Metadata("simulated", "true"));
        }

        [Fact]
        public void WhenCreatedWithMetadataThenMetadataIsMappedToSimulatedDevice()
        {
            var model = new SimulatedDeviceModel(Metadata: new []{new MetadataModel("one", "three")});

            var entity = SimulatedDeviceEntity.Create(model);

            entity.Metadata.Should().Contain(new Metadata("one", "three"));
        }

        [Fact]
        public void WhenTurnedIntoDeviceDiscoveredThenDeviceDiscoveredIsPopulatedFromSimulatedDevice()
        {
            var entity = SimulatedDeviceEntity.Create(new SimulatedDeviceModel(DeviceType: DeviceType.Light));

            var model = entity.ToDeviceDiscoveredModel();

            model.Id.Should().Be(entity.Id);
            model.DeviceType.Should().Be(entity.DeviceType);
            model.Metadata.Should().Contain(m => m.Key == "simulated" && m.Value == "true");
        }

        [Fact]
        public void WhenConvertedToModelThenReturnsSimulatedDeviceModel()
        {
            var entity = SimulatedDeviceEntity.Create(new SimulatedDeviceModel($"{Guid.NewGuid()}", DeviceType.Light, new []
            {
                new MetadataModel("one", "three")
            }));

            var model = entity.ToModel();

            model.Id.Should().Be(entity.Id);
            model.DeviceType.Should().Be(DeviceType.Light);
            model.Metadata.Should().HaveCount(2)
                .And.ContainEquivalentOf(new MetadataModel("one", "three"));
        }

        [Fact]
        public void WhenSimulatorIsLightAndConvertedToModelThenLightingIsInModel()
        {
            var entity = new SimulatedDeviceEntity(DeviceType: DeviceType.Light, Lighting: new LightingModel());

            var model = entity.ToModel();

            model.Lighting.Should().BeEquivalentTo(new LightingModel());
        }
    }
}