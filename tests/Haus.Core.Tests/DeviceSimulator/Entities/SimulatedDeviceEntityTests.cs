using FluentAssertions;
using Haus.Core.Common.Entities;
using Haus.Core.DeviceSimulator.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.DeviceSimulator;
using Xunit;

namespace Haus.Core.Tests.DeviceSimulator.Entities
{
    public class SimulatedDeviceEntityTests
    {
        [Fact]
        public void WhenCreatedThenSimulatedMetadataIsAdded()
        {
            var model = new CreateSimulatedDeviceModel {DeviceType = DeviceType.Light};

            var entity = SimulatedDeviceEntity.Create(model);

            entity.Id.Should().NotBeNullOrWhiteSpace();
            entity.DeviceType.Should().Be(DeviceType.Light);
            entity.Metadata.Should().Contain(new Metadata("simulated", "true"));
        }

        [Fact]
        public void WhenCreatedWithMetadataThenMetadataIsMappedToSimulatedDevice()
        {
            var model = new CreateSimulatedDeviceModel
            {
                Metadata = new []{new MetadataModel("one", "three")}
            };

            var entity = SimulatedDeviceEntity.Create(model);

            entity.Metadata.Should().Contain(new Metadata("one", "three"));
        }

        [Fact]
        public void WhenTurnedIntoDeviceDiscoveredThenDeviceDiscoverdIsPopulatedFromSimulatedDevice()
        {
            var entity = SimulatedDeviceEntity.Create(new CreateSimulatedDeviceModel {DeviceType = DeviceType.Light});

            var model = entity.ToDeviceDiscoveredModel();

            model.Id.Should().Be(entity.Id);
            model.DeviceType.Should().Be(entity.DeviceType);
            model.Metadata.Should().Contain(m => m.Key == "simulated" && m.Value == "true");
        }
    }
}