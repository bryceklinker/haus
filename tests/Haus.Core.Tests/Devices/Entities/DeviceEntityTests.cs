using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Xunit;

namespace Haus.Core.Tests.Devices.Entities
{
    public class DeviceEntityTest
    {
        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenExternalIdIsSetToDiscoveredId()
        {
            var model = new DeviceDiscoveredModel { Id = "this-id" };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Equal("this-id", entity.ExternalId);
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
        public void WhenCreatedFromDeviceDiscoveredThenModelIsInMetadata()
        {
            var model = new DeviceDiscoveredModel { Model = "this model" };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Single(entity.Metadata);
            AssertHasMetadata("Model", "this model", entity);
        }

        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenVendorIsInMetadata()
        {
            var model = new DeviceDiscoveredModel { Vendor = "whoops" };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Single(entity.Metadata);
            AssertHasMetadata("Vendor", "whoops", entity);
        }

        [Fact]
        public void WhenCreatedFromDeviceDiscoveredThenDescriptionIsInMetadata()
        {
            var model = new DeviceDiscoveredModel { Description = "new hotness" };

            var entity = DeviceEntity.FromDiscoveredDevice(model);

            Assert.Single(entity.Metadata);
            AssertHasMetadata("Description", "new hotness", entity);
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
            var model = new DeviceModel { Name = "Somename", ExternalId = "dont-use-this"};
            var entity = new DeviceEntity();

            entity.UpdateFromModel(model);

            Assert.Equal("Somename", entity.Name);
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