using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.DomainEvents;
using Haus.Core.Lighting.Entities;
using Haus.Core.Lighting.Generators;
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
                d.LightType,
                d.Metadata.Select(m => new MetadataModel(m.Key, m.Value)).ToArray(),
                d.Lighting == null
                    ? null
                    : new LightingModel(
                        d.Lighting.State,
                        d.Lighting.Level == null
                            ? null
                            : new LevelLightingModel(d.Lighting.Level.Value, d.Lighting.Level.Min,
                                d.Lighting.Level.Max),
                        d.Lighting.Temperature == null
                            ? null
                            : new TemperatureLightingModel(d.Lighting.Temperature.Value, d.Lighting.Temperature.Min,
                                d.Lighting.Temperature.Max),
                        d.Lighting.Color == null
                            ? null
                            : new ColorLightingModel(d.Lighting.Color.Red, d.Lighting.Color.Green,
                                d.Lighting.Color.Blue)
                    )
            );

        private static readonly Lazy<Func<DeviceEntity, DeviceModel>> ToModelFunc = new(ToModelExpression.Compile);
        
        public string ExternalId { get; set; }

        public string Name { get; set; }

        public DeviceType DeviceType { get; set; }
        public LightType LightType { get; set; }

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
            LightType lightType = LightType.None,
            RoomEntity room = null,
            LightingEntity lighting = null,
            ICollection<DeviceMetadataEntity> metadata = null)
        {
            Id = id;
            ExternalId = externalId ?? string.Empty;
            Name = name ?? string.Empty;
            DeviceType = deviceType;
            LightType = lightType;
            Room = room;
            Lighting = lighting;
            Metadata = metadata ?? new List<DeviceMetadataEntity>();
        }

        public DeviceModel ToModel() => ToModelFunc.Value(this);

        public static DeviceEntity FromDiscoveredDevice(DeviceDiscoveredEvent @event, IDomainEventBus domainEventBus)
        {
            var entity = new DeviceEntity
            {
                ExternalId = @event.Id,
                Name = @event.Id
            };
            entity.UpdateFromDiscoveredDevice(@event, domainEventBus);
            return entity;
        }

        public void UpdateFromDiscoveredDevice(DeviceDiscoveredEvent @event, IDomainEventBus domainEventBus)
        {
            DeviceType = @event.DeviceType;
            LightType = IsLight && LightType == LightType.None ? LightType.Level : LightType;
            Lighting = GenerateDefaultLighting();
            if (IsLight) ChangeLighting(Lighting, domainEventBus);

            AddOrUpdateMetadata(@event.Metadata);
        }

        public void UpdateFromModel(DeviceModel model, IDomainEventBus domainEvenBus)
        {
            Name = model.Name;
            DeviceType = model.DeviceType;
            if (LightType != model.LightType)
            {
                LightType = model.LightType;
                Lighting = GenerateDefaultLighting();
            }
            if (IsLight) ChangeLighting(Lighting, domainEvenBus);
            
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

        public void UnassignFromRoom()
        {
            Room = null;
        }

        public void ChangeLighting(LightingEntity targetLighting, IDomainEventBus domainEventBus)
        {
            if (!IsLight)
                throw new InvalidOperationException($"Device with id {Id} is not a light.");

            Lighting = Lighting == null ? targetLighting : Lighting.CalculateTarget(targetLighting);
            domainEventBus.Enqueue(new DeviceLightingChangedDomainEvent(this, Lighting));
        }

        public void TurnOff(IDomainEventBus domainEventBus)
        {
            var lightingCopy = LightingEntity.FromEntity(Lighting ?? GenerateDefaultLighting());
            lightingCopy.State = LightingState.Off;
            ChangeLighting(lightingCopy, domainEventBus);
        }

        public void TurnOn(IDomainEventBus domainEventBus)
        {
            var turnOnLighting = LightingEntity.FromEntity(Lighting ?? GenerateDefaultLighting());
            turnOnLighting.State = LightingState.On;
            ChangeLighting(turnOnLighting, domainEventBus);
        }

        private LightingEntity GenerateDefaultLighting()
        {
            return DefaultLightingGeneratorFactory.GetGenerator(DeviceType, LightType)
                .Generate(Lighting, Room?.Lighting);
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