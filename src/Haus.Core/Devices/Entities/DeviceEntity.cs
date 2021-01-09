using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Lighting;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Lighting;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Devices.Entities
{
    public class DeviceEntity : Entity
    {
        public static readonly Expression<Func<DeviceEntity, DeviceModel>> ToModelExpression =
            d => new DeviceModel(
                d.Id,
                d.Room == null ? default(long?) : d.Room.Id,
                d.ExternalId,
                d.Name,
                d.DeviceType,
                d.Metadata.Select(m => new MetadataModel(m.Key, m.Value)).ToArray(),
                new LightingModel(d.Lighting.State, d.Lighting.Level, d.Lighting.Temperature,
                    new LightingColorModel(d.Lighting.Color.Red, d.Lighting.Color.Green, d.Lighting.Color.Blue),
                    new LightingConstraintsModel(
                        d.Lighting.Constraints.MinLevel, 
                        d.Lighting.Constraints.MaxLevel,
                        d.Lighting.Constraints.MinTemperature, 
                        d.Lighting.Constraints.MaxTemperature)
                )
            );

        private static readonly Lazy<Func<DeviceEntity, DeviceModel>> ToModelFunc = new(ToModelExpression.Compile);

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public DeviceType DeviceType { get; set; }

        public ICollection<DeviceMetadataEntity> Metadata { get; set; }

        public RoomEntity Room { get; set; }

        public LightingEntity Lighting { get; set; }

        public bool IsLight => DeviceType == DeviceType.Light;

        public DeviceEntity()
            : this(0, null, null)
        {
            
        }

        public DeviceEntity(
            long id = 0, 
            string externalId = "", 
            string name = "", 
            DeviceType deviceType = DeviceType.Unknown, 
            RoomEntity room = null, 
            LightingEntity lighting = null, 
            ICollection<DeviceMetadataEntity> metadata = null)
        {
            Id = id;
            ExternalId = externalId ?? string.Empty;
            Name = name ?? string.Empty;
            DeviceType = deviceType;
            Room = room;
            Lighting = lighting ?? LightingEntity.Default.Copy();
            Metadata = metadata ?? new List<DeviceMetadataEntity>();
        }

        public DeviceModel ToModel() => ToModelFunc.Value(this);

        public static DeviceEntity FromDiscoveredDevice(DeviceDiscoveredEvent @event)
        {
            var entity = new DeviceEntity
            {
                ExternalId = @event.Id,
                DeviceType = @event.DeviceType,
                Name = @event.Id
            };
            entity.UpdateFromDiscoveredDevice(@event);
            return entity;
        }

        public void UpdateFromDiscoveredDevice(DeviceDiscoveredEvent @event)
        {
            DeviceType = @event.DeviceType;
            AddOrUpdateMetadata(@event.Metadata);
        }

        public void UpdateFromModel(DeviceModel model)
        {
            Name = model.Name;
            DeviceType = model.DeviceType;
            AddOrUpdateMetadata(model.Metadata);
        }

        private void AddOrUpdateMetadata(IEnumerable<MetadataModel> models)
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

        public void ChangeLighting(LightingEntity desiredLighting, IDomainEventBus domainEventBus)
        {
            if (!IsLight)
                throw new InvalidOperationException($"Device with id {Id} is not a light.");

            Lighting = Lighting.ToDesiredLighting(desiredLighting);
            domainEventBus.Enqueue(new DeviceLightingChangedDomainEvent(this, Lighting));
        }

        public void ChangeLightingConstraints(
            LightingConstraintsEntity lightingConstraintsEntity,
            IDomainEventBus domainEventBus)
        {
            if (!IsLight)
                throw new InvalidOperationException($"Device {Id} is not a light");

            Lighting = Lighting.ChangeLightingConstraints(lightingConstraintsEntity);
            domainEventBus.Enqueue(new DeviceLightingConstraintsChangedDomainEvent(this, Lighting.Constraints));
            domainEventBus.Enqueue(new DeviceLightingChangedDomainEvent(this, Lighting));
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

            builder.OwnsOne(d => d.Lighting, LightingEntity.Configure);

            builder.HasMany(d => d.Metadata)
                .WithOne(m => m.Device);
        }
    }
}