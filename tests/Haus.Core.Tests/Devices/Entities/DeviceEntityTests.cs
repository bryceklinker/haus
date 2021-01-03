using System;
using Haus.Core.Common;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using System.Linq;
using FluentAssertions;
using Haus.Core.Common.Entities;
using Haus.Core.Lighting;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;
using Xunit;

namespace Haus.Core.Tests.Devices.Entities
{
    public class DeviceEntityTest
    {
        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenEntityIsPopulatedFromDiscoveredDevice()
        {
            var model = new DeviceDiscoveredModel("this-id", DeviceType.Light, new[]
            {
                new MetadataModel("Model", "some model"),
                new MetadataModel("Vendor", "Vendy"),
            });

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            entity.ExternalId.Should().Be("this-id");
            entity.DeviceType.Should().Be(DeviceType.Light);
            
            entity.Metadata.Should()
                .ContainEquivalentOf(new DeviceMetadataEntity("Vendor", "Vendy"))
                .And.ContainEquivalentOf(new DeviceMetadataEntity("Model", "some model"));
        }

        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenNameIsSetToExternalId()
        {
            var model = new DeviceDiscoveredModel("this-id");

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            entity.Name.Should().Be("this-id");
        }

        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenDeviceTypeIsUpdated()
        {
            var model = new DeviceDiscoveredModel("", DeviceType.MotionSensor);
            
            var entity = new DeviceEntity();
            entity.UpdateFromDiscoveredDevice(model);

            entity.DeviceType.Should().Be(DeviceType.MotionSensor);
        }
        
        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenModelMetadataIsAdded()
        {
            var model = new DeviceDiscoveredModel("", metadata: new []
            {
                new MetadataModel("Model", "boom"),
            });
            var entity = new DeviceEntity();
            
            entity.UpdateFromDiscoveredDevice(model);

            entity.Metadata.Should().ContainEquivalentOf(new DeviceMetadataEntity("Model", "boom"));
        }

        [Fact]
        public void WhenDeviceHasModelAndUpdatedFromDiscoveredDeviceThenModelMetadataIsUpdated()
        {
            var model = new DeviceDiscoveredModel("", metadata: new []
            {
                new MetadataModel("Model", "boom"),
            });
            var entity = new DeviceEntity();
            entity.AddOrUpdateMetadata("Model", "old");

            entity.UpdateFromDiscoveredDevice(model);

            entity.Metadata.Should().HaveCount(1)
                .And.ContainEquivalentOf(new DeviceMetadataEntity("Model", "boom"));
        }

        [Fact]
        public void WhenDeviceIsUpdatedFromModelThenDeviceMatchesModel()
        {
            var model = new DeviceModel
            {
                Name = "Somename", 
                ExternalId = "dont-use-this",
                DeviceType = DeviceType.LightSensor
            };
            var entity = new DeviceEntity();

            entity.UpdateFromModel(model);

            entity.Name.Should().Be("Somename");
            entity.DeviceType.Should().Be(DeviceType.LightSensor);
        }

        [Fact]
        public void WhenDeviceIsUpdatedFromModelThenMetadataForDeviceIsUpdated()
        {
            var model = new DeviceModel(Metadata: new []
            {
                new MetadataModel("one", "three"),
                new MetadataModel("three", "two"), 
            });
            var entity = new DeviceEntity();
            entity.AddOrUpdateMetadata("one", "two");

            entity.UpdateFromModel(model);

            entity.Metadata.Should().HaveCount(2)
                .And.ContainEquivalentOf(new DeviceMetadataEntity("one", "three"))
                .And.ContainEquivalentOf(new DeviceMetadataEntity("three", "two"));
        }

        [Fact]
        public void WhenDeviceLightingChangedThenDeviceLightingChangedEventQueued()
        {
            var domainEventBus = new FakeDomainEventBus();
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            var lighting = new LightingEntity {Level = 12};

            light.ChangeLighting(lighting, domainEventBus);

            domainEventBus.GetEvents.Should()
                .HaveCount(1)
                .And.ContainItemsAssignableTo<DeviceLightingChangedDomainEvent>();
        }

        [Fact]
        public void WhenDeviceIsTurnedOffThenDeviceLightingStateIsSetToOff()
        {
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            light.ChangeLighting(new LightingEntity{State = LightingState.On, Level = 6}, new FakeDomainEventBus());

            light.TurnOff(new FakeDomainEventBus());

            light.Lighting.State.Should().Be(LightingState.Off);
            light.Lighting.Level.Should().Be(6);
        }

        [Fact]
        public void WhenDeviceIsTurnedOnThenDeviceLightingStateIsSetToOn()
        {
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            light.ChangeLighting(new LightingEntity{State = LightingState.Off, Level = 6}, new FakeDomainEventBus());

            light.TurnOn(new FakeDomainEventBus());

            light.Lighting.State.Should().Be(LightingState.On);
            light.Lighting.Level.Should().Be(6);
        }
        
        [Fact]
        public void WhenDeviceIsNotALightThenChangeLightingThrowsInvalidOperation()
        {
            var device = new DeviceEntity();

            Action act = () => device.ChangeLighting(new LightingEntity(), new FakeDomainEventBus());

            act.Should().Throw<InvalidOperationException>();
        }
    }
}