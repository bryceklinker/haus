using System;
using System.Collections.Generic;
using System.Linq;
using Haus.Core.Common;
using Haus.Core.Common.DomainEvents;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Models.Common;
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
        public DeviceType DeviceType { get; set; } = DeviceType.Unknown;
        
        public ICollection<DeviceMetadataEntity> Metadata { get; set; } = new List<DeviceMetadataEntity>();

        public RoomEntity Room { get; set; }
        public Lighting Lighting { get; set; } = Lighting.Default.Copy();

        public bool IsLight => DeviceType == DeviceType.Light;

        public static DeviceEntity FromDiscoveredDevice(DeviceDiscoveredModel model)
        {
            var entity = new DeviceEntity
            {
                ExternalId = model.Id,
                DeviceType = model.DeviceType,
                Name = model.Id
            };
            entity.UpdateFromDiscoveredDevice(model);
            return entity;
        }

        public void UpdateFromDiscoveredDevice(DeviceDiscoveredModel model)
        {
            DeviceType = model.DeviceType;
            AddOrUpdateMetadata(model.Metadata);
        }

        public void UpdateFromModel(DeviceModel model)
        {
            Name = model.Name;
            DeviceType = model.DeviceType;
            AddOrUpdateMetadata(model.Metadata);
        }

        private void AddOrUpdateMetadata(IEnumerable<DeviceMetadataModel> models)
        {
            foreach (var model in models)
                AddOrUpdateMetadata(model.Key, model.Value);
        }

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

        public void AssignToRoom(RoomEntity room)
        {
            Room = room;
        }

        public void UnAssignRoom()
        {
            Room = null;
        }

        public void ChangeLighting(Lighting lighting, IDomainEventBus domainEventBus)
        {
            if (!IsLight)
                throw new InvalidOperationException($"Device with id {Id} is not a light.");
            
            Lighting = lighting;
            domainEventBus.Enqueue(new DeviceLightingChangedDomainEvent(this, lighting));
        }

        public void TurnOff(IDomainEventBus domainEventBus)
        {
            var lightingCopy = Lighting.Copy();
            lightingCopy.State = LightingState.Off;
            ChangeLighting(lightingCopy, domainEventBus);
        }

        public void TurnOn(IDomainEventBus domainEventBus)
        {
            var lightingCopy = Lighting.Copy();
            lightingCopy.State = LightingState.On;
            ChangeLighting(lightingCopy, domainEventBus);
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

            builder.Ignore(d => d.IsLight);
            
            builder.OwnsOne(d => d.Lighting, Lighting.Configure);
            
            builder.HasMany(d => d.Metadata)
                .WithOne(m => m.Device);
        }
    }
}