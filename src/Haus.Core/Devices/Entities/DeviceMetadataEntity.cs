using System;
using System.Linq.Expressions;
using Haus.Core.Models.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities
{
    public class DeviceMetadataEntity
    {
        public long Id { get; set; }
        public DeviceEntity Device { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public DeviceMetadataEntity()
        {
            
        }

        public DeviceMetadataEntity(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public void Update(string value)
        {
            Value = value;
        }

        public static Expression<Func<DeviceMetadataEntity, DeviceMetadataModel>> ToModel()
        {
            return e => new DeviceMetadataModel
            {
                Key = e.Key,
                Value = e.Value
            };
        }
    }

    public class DeviceMetadataEntityConfiguration : IEntityTypeConfiguration<DeviceMetadataEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceMetadataEntity> builder)
        {
            builder.HasKey(d => d.Id);
            
            builder.Property(p => p.Key).IsRequired();
            builder.Property(p => p.Value).IsRequired();
            builder.HasOne(d => d.Device)
                .WithMany(d => d.Metadata);
        }
    }
}