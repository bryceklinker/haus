using System;
using Haus.Core.Common;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using System.Linq;
using Haus.Core.Models.Common;
using Xunit;

namespace Haus.Core.Tests.Devices.Entities
{
    public class DeviceEntityTest
    {
        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenEntityIsPopulatedFromDiscoveredDevice()
        {
            var model = new DeviceDiscoveredModel
            {
                Id = "this-id",
                DeviceType = DeviceType.Light,
                Metadata = new []
                {
                    new MetadataModel("Model", "some model"),
                    new MetadataModel("Vendor", "Vendy"), 
                }
            };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Equal("this-id", entity.ExternalId);
            Assert.Equal(DeviceType.Light, entity.DeviceType);
            AssertHasMetadata("Vendor", "Vendy", entity);
            AssertHasMetadata("Model", "some model", entity);
        }

        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenNameIsSetToExternalId()
        {
            var model = new DeviceDiscoveredModel { Id = "this-id" };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Equal("this-id", entity.Name);
        }

        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenDeviceTypeIsUpdated()
        {
            var model = new DeviceDiscoveredModel {DeviceType = DeviceType.MotionSensor};
            
            var entity = new DeviceEntity();
            entity.UpdateFromDiscoveredDevice(model);

            Assert.Equal(DeviceType.MotionSensor, entity.DeviceType);
        }
        
        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenModelMetadataIsAdded()
        {
            var model = new DeviceDiscoveredModel
            {
                Metadata = new []
                {
                    new MetadataModel("Model", "boom"),
                }
            };
            var entity = new DeviceEntity();
            
            entity.UpdateFromDiscoveredDevice(model);

            AssertHasMetadata("Model", "boom", entity);
        }

        [Fact]
        public void WhenDeviceHasModelAndUpdatedFromDiscoveredDeviceThenModelMetadataIsUpdated()
        {
            var model = new DeviceDiscoveredModel
            {
                Metadata = new []
                {
                    new MetadataModel("Model", "boom"),
                }
            };
            var entity = new DeviceEntity();
            entity.AddOrUpdateMetadata("Model", "old");

            entity.UpdateFromDiscoveredDevice(model);

            Assert.Single(entity.Metadata);
            AssertHasMetadata("Model", "boom", entity);
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

            Assert.Equal("Somename", entity.Name);
            Assert.Equal(DeviceType.LightSensor, entity.DeviceType);
        }

        [Fact]
        public void WhenDeviceIsUpdatedFromModelThenMetadataForDeviceIsUpdated()
        {
            var model = new DeviceModel
            {
                Metadata = new []
                {
                    new MetadataModel("one", "three"),
                    new MetadataModel("three", "two"), 
                }
            };
            var entity = new DeviceEntity();
            entity.AddOrUpdateMetadata("one", "two");

            entity.UpdateFromModel(model);

            Assert.Equal(2, entity.Metadata.Count);
            Assert.Contains(entity.Metadata, m => m.Key == "one" && m.Value == "three");
            Assert.Contains(entity.Metadata, m => m.Key == "three" && m.Value == "two");
        }

        [Fact]
        public void WhenDeviceLightingChangedThenDeviceLightingChangedEventQueued()
        {
            var domainEventBus = new FakeDomainEventBus();
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            var lighting = new Lighting {Brightness = 12};

            light.ChangeLighting(lighting, domainEventBus);

            Assert.Single(domainEventBus.GetEvents.OfType<DeviceLightingChangedDomainEvent>());
        }

        [Fact]
        public void WhenDeviceIsTurnedOffThenDeviceLightingStateIsSetToOff()
        {
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            light.ChangeLighting(new Lighting{State = LightingState.On, Brightness = 6}, new FakeDomainEventBus());

            light.TurnOff(new FakeDomainEventBus());

            Assert.Equal(LightingState.Off, light.Lighting.State);
            Assert.Equal(6, light.Lighting.Brightness);
        }

        [Fact]
        public void WhenDeviceIsTurnedOnThenDeviceLightingStateIsSetToOn()
        {
            var light = new DeviceEntity{DeviceType = DeviceType.Light};
            light.ChangeLighting(new Lighting{State = LightingState.Off, Brightness = 6}, new FakeDomainEventBus());

            light.TurnOn(new FakeDomainEventBus());

            Assert.Equal(LightingState.On, light.Lighting.State);
            Assert.Equal(6, light.Lighting.Brightness);
        }
        
        [Fact]
        public void WhenDeviceIsNotALightThenChangeLightingThrowsInvalidOperation()
        {
            var device = new DeviceEntity();
            
            Assert.Throws<InvalidOperationException>(() => device.ChangeLighting(new Lighting(), new FakeDomainEventBus()));
        }

        private static void AssertHasMetadata(string key, string value, DeviceEntity entity)
        {
            Assert.Contains(entity.Metadata, meta => meta.Key == key && meta.Value == value);
        }
    }
}