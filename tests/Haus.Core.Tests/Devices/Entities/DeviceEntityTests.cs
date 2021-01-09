using System;
using System.Linq;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Tests.Support;
using FluentAssertions;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Core.Rooms.Entities;
using Xunit;

namespace Haus.Core.Tests.Devices.Entities
{
    public class DeviceEntityTest
    {
        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenEntityIsPopulatedFromDiscoveredDevice()
        {
            var model = new DeviceDiscoveredEvent("this-id", DeviceType.Light, new[]
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
            var model = new DeviceDiscoveredEvent("this-id");

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            entity.Name.Should().Be("this-id");
        }

        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenDeviceTypeIsUpdated()
        {
            var model = new DeviceDiscoveredEvent("", DeviceType.MotionSensor);
            
            var entity = new DeviceEntity();
            entity.UpdateFromDiscoveredDevice(model);

            entity.DeviceType.Should().Be(DeviceType.MotionSensor);
        }
        
        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenModelMetadataIsAdded()
        {
            var model = new DeviceDiscoveredEvent("", Metadata: new []
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
            var model = new DeviceDiscoveredEvent("", Metadata: new []
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
            var lighting = new LightingEntity(
                LightingState.On,
                6,
                4500,
                new LightingColorEntity(65, 12, 54)
            );
            light.ChangeLighting(lighting, new FakeDomainEventBus());

            light.TurnOff(new FakeDomainEventBus());

            light.Lighting.State.Should().Be(LightingState.Off);
            light.Lighting.Level.Should().Be(6);
            light.Lighting.Color.Red.Should().Be(65);
            light.Lighting.Color.Green.Should().Be(12);
            light.Lighting.Color.Blue.Should().Be(54);
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

        [Fact]
        public void WhenConvertedToModelThenModelIsPopulatedFromDevice()
        {
            var lighting = new LightingEntity(LightingState.On, 50, 12);
            var metadata = new[] {new DeviceMetadataEntity("one", "two")};
            var device = new DeviceEntity(12, $"{Guid.NewGuid()}", $"{Guid.NewGuid()}", DeviceType.LightSensor, new RoomEntity(89, "ignore"), lighting, metadata);

            var model = device.ToModel();

            model.Id.Should().Be(12);
            model.ExternalId.Should().Be(device.ExternalId);
            model.Name.Should().Be(device.Name);
            model.DeviceType.Should().Be(DeviceType.LightSensor);
            model.RoomId.Should().Be(89);
            model.Lighting.Should().BeEquivalentTo(lighting.ToModel());
            model.Metadata.Should().HaveCount(1)
                .And.ContainEquivalentOf(metadata[0].ToModel());
        }

        [Fact]
        public void WhenDeviceIsNotALightThenChangingLightingConstraintsThrowsInvalidOperation()
        {
            var device = new DeviceEntity();

            Action act = () => device.ChangeLightingConstraints(LightingConstraintsEntity.Default, new FakeDomainEventBus());

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void WhenLightingConstraintsChangedThenConstraintsForLightingAreChanged()
        {
            var device = new DeviceEntity(deviceType: DeviceType.Light);
            var originalLighting = device.Lighting;

            var constraints = new LightingConstraintsEntity(45, 92, 200, 300);
            device.ChangeLightingConstraints(constraints, new FakeDomainEventBus());

            device.Lighting.Should().NotBeSameAs(originalLighting);
            device.Lighting.Constraints.Should().BeEquivalentTo(constraints);
        }

        [Fact]
        public void WhenLightingConstraintsChangedThenLightingLevelIsChangedToBeWithinConstraints()
        {
            var device = new DeviceEntity(deviceType: DeviceType.Light, lighting: new LightingEntity(LightingState.Off, 100, 6000));

            var constraints = new LightingConstraintsEntity(10, 50, 4000, 5000);
            device.ChangeLightingConstraints(constraints, new FakeDomainEventBus());

            device.Lighting.Level.Should().Be(50);
            device.Lighting.Temperature.Should().Be(5000);
        }

        [Fact]
        public void WhenLightingConstraintsChangedThenPublishesDeviceLightingConstraintsChangedEvent()
        {
            var device = new DeviceEntity(deviceType: DeviceType.Light);
            var bus = new FakeDomainEventBus();

            device.ChangeLightingConstraints(LightingConstraintsEntity.Default, bus);

            bus.GetEvents.OfType<DeviceLightingConstraintsChangedDomainEvent>()
                .Should().HaveCount(1);
        }

        [Fact]
        public void WhenLightingConstraintsChangedThenPublishesDeviceLightingChangedEvent()
        {
            var device = new DeviceEntity(deviceType: DeviceType.Light);
            var bus = new FakeDomainEventBus();

            device.ChangeLightingConstraints(LightingConstraintsEntity.Default, bus);

            bus.GetEvents.OfType<DeviceLightingChangedDomainEvent>()
                .Should().HaveCount(1);
        }
    }
}