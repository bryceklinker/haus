using System;
using Haus.Core.Devices.Entities;
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
        public void WhenMappedToDeviceModelThenDeviceModelIsPopulatedFromEntity()
        {
            var entity = new DeviceEntity
            {
                Id = 54,
                ExternalId = $"{Guid.NewGuid()}"
            };
            entity.AddOrUpdateMetadata("one", "three");

            var model = DeviceEntity.ToModel().Compile().Invoke(entity);

            Assert.Equal(54, model.Id);
            Assert.Equal(entity.ExternalId, model.ExternalId);
            Assert.Equal("one", model.Metadata[0].Key);
            Assert.Equal("three", model.Metadata[0].Value);
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

        private void AssertHasMetadata(string key, string value, DeviceEntity entity)
        {
            Assert.Contains(entity.Metadata, meta => meta.Key == key && meta.Value == value);
        }
    }
}