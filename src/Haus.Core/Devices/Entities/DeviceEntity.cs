using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities
{
    public class DeviceEntity
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        
        public ICollection<DeviceMetadataEntity> Metadata { get; set; } = new List<DeviceMetadataEntity>();

        public void AddOrUpdateMetadata(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var existing = Metadata.SingleOrDefault(m => m.Key == key);
            if (existing != null)
                existing.Update(value);
            else
                Metadata.Add(new DeviceMetadataEntity(key, value));
        }
        
        public static DeviceEntity FromDiscoveredDevice(DeviceDiscoveredModel model)
        {
            var entity = new DeviceEntity { ExternalId = model.Id };
            entity.UpdateFromDiscoveredDevice(model);
            return entity;
        }

        public void UpdateFromDiscoveredDevice(DeviceDiscoveredModel model)
        {
            AddOrUpdateMetadata(nameof(model.Model), model.Model);    
            AddOrUpdateMetadata(nameof(model.Vendor), model.Vendor);    
            AddOrUpdateMetadata(nameof(model.Description), model.Description);
        }

        public static Expression<Func<DeviceEntity, DeviceModel>> ToModel()
        {
            return d => new DeviceModel
            {
                Id = d.Id,
                ExternalId = d.ExternalId,
                Metadata = d.Metadata.Select(e => new DeviceMetadataModel
                {
                    Key = e.Key,
                    Value = e.Value
                }).ToArray()
            };
        }
    }

    public class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceEntity> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.ExternalId).IsRequired();
            
            builder.HasMany(d => d.Metadata)
                .WithOne(m => m.Device);
        }
    }
}