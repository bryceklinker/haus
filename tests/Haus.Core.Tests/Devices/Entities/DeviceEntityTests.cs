using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
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
                Description = "idk",
                Vendor = "Vendy",
                Model = "some model",
                DeviceType = DeviceType.Light
            };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Equal("this-id", entity.ExternalId);
            Assert.Equal(DeviceType.Light, entity.DeviceType);
            AssertHasMetadata("Description", "idk", entity);
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
        public void WhenCreatedFromDeviceDiscoveredWithModelThenNameIsSetToModel()
        {
            var model = new DeviceDiscoveredModel { Id = "this-id", Model = "GCL-LED-Strip"};

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Equal("GCL-LED-Strip", entity.Name);
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
            var model = new DeviceDiscoveredModel { Model = "boom" };
            var entity = new DeviceEntity();
            
            entity.UpdateFromDiscoveredDevice(model);

            AssertHasMetadata("Model", "boom", entity);
        }

        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenVendorMetadataIsAdded()
        {
            var model = new DeviceDiscoveredModel { Vendor = "vendy" };
            var entity = new DeviceEntity();
            
            entity.UpdateFromDiscoveredDevice(model);

            AssertHasMetadata("Vendor", "vendy", entity);
        }
        
        [Fact]
        public void WhenUpdatedFromDiscoveredDeviceThenDescriptionMetadataIsAdded()
        {
            var model = new DeviceDiscoveredModel { Description = "yep" };
            var entity = new DeviceEntity();
            
            entity.UpdateFromDiscoveredDevice(model);

            AssertHasMetadata("Description", "yep", entity);
        }

        [Fact]
        public void WhenDeviceHasModelAndUpdatedFromDiscoveredDeviceThenModelMetadataIsUpdated()
        {
            var model = new DeviceDiscoveredModel { Model = "boom" };
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
                    new DeviceMetadataModel("one", "three"),
                    new DeviceMetadataModel("three", "two"), 
                }
            };
            var entity = new DeviceEntity();
            entity.AddOrUpdateMetadata("one", "two");

            entity.UpdateFromModel(model);

            Assert.Equal(2, entity.Metadata.Count);
            Assert.Contains(entity.Metadata, m => m.Key == "one" && m.Value == "three");
            Assert.Contains(entity.Metadata, m => m.Key == "three" && m.Value == "two");
        }

        private static void AssertHasMetadata(string key, string value, DeviceEntity entity)
        {
            Assert.Contains(entity.Metadata, meta => meta.Key == key && meta.Value == value);
        }
    }
}