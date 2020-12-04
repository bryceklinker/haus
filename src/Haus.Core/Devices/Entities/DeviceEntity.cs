using System.Collections.Generic;
using System.Linq;
using Haus.Core.Common.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Rooms.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities
{
    public class DeviceEntity : Entity
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public DeviceType DeviceType { get; set; }
        
        public ICollection<DeviceMetadataEntity> Metadata { get; set; } = new List<DeviceMetadataEntity>();

        public RoomEntity Room { get; set; }
        
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
            var entity = new DeviceEntity
            {
                ExternalId = model.Id,
                DeviceType = model.DeviceType,
                Name = string.IsNullOrWhiteSpace(model.Model) ? model.Id : model.Model
            };
            entity.UpdateFromDiscoveredDevice(model);
            return entity;
        }

        public void UpdateFromDiscoveredDevice(DeviceDiscoveredModel model)
        {
            DeviceType = model.DeviceType;
            AddOrUpdateMetadata(nameof(model.Model), model.Model);    
            AddOrUpdateMetadata(nameof(model.Vendor), model.Vendor);    
            AddOrUpdateMetadata(nameof(model.Description), model.Description);
        }

        public void UpdateFromModel(DeviceModel model)
        {
            Name = model.Name;
            DeviceType = model.DeviceType;
            foreach (var metadataModel in model.Metadata)
                AddOrUpdateMetadata(metadataModel.Key, metadataModel.Value);
        }

        public void AssignToRoom(RoomEntity room)
        {
            Room = room;
        }

        public void UnAssignRoom()
        {
            Room = null;
        }
    }

    public class DeviceEntityConfiguration : IEntityTypeConfiguration<DeviceEntity>
    {
        public void Configure(EntityTypeBuilder<DeviceEntity> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired();
            builder.Property(d => d.ExternalId).IsRequired();
            builder.Property(d => d.DeviceType).IsRequired().HasConversion<string>();
            
            builder.HasMany(d => d.Metadata)
                .WithOne(m => m.Device);
        }
    }
}