using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Haus.Core.Common;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.DomainEvents;
using Haus.Cqrs.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Rooms.Entities
{
    public class RoomEntity : Entity
    {
        public string Name { get; set; } = string.Empty;
        public Lighting Lighting { get; set; } = Lighting.Default.Copy();
        public ICollection<DeviceEntity> Devices { get; set; } = new List<DeviceEntity>();
        public IEnumerable<DeviceEntity> Lights => Devices.Where(d => d.IsLight);

        public static readonly Expression<Func<RoomEntity, RoomModel>> ToModelExpression =
            r => new RoomModel(
                r.Id,
                r.Name,
                new LightingModel(
                    r.Lighting.State,
                    r.Lighting.BrightnessPercent,
                    r.Lighting.Temperature,
                    new LightingColorModel(r.Lighting.Color.Red, r.Lighting.Color.Green, r.Lighting.Color.Blue)
                )
            );

        private static readonly Lazy<Func<RoomEntity, RoomModel>> ToModelFunc = new(ToModelExpression.Compile);
        
        public RoomModel ToModel() => ToModelFunc.Value(this);

        public static RoomEntity CreateFromModel(RoomModel model)
        {
            return new RoomEntity {Name = model.Name};
        }

        public void UpdateFromModel(RoomModel roomModel)
        {
            Name = roomModel.Name;
        }

        public void AddDevice(DeviceEntity device, IDomainEventBus domainEventBus)
        {
            if (Devices.Any(d => d.Id == device.Id))
                return;

            Devices.Add(device);
            device.AssignToRoom(this);
            if (device.IsLight) device.ChangeLighting(Lighting, domainEventBus);
        }

        public void AddDevices(IEnumerable<DeviceEntity> devices, IDomainEventBus domainEventBus)
        {
            foreach (var device in devices)
                AddDevice(device, domainEventBus);
        }

        public void RemoveDevice(DeviceEntity device)
        {
            Devices.Remove(device);
            device.UnAssignRoom();
        }

        public void ChangeLighting(Lighting lighting, IDomainEventBus domainEventBus)
        {
            Lighting = lighting;
            foreach (var light in Lights)
                light.ChangeLighting(Lighting, domainEventBus);

            domainEventBus.Enqueue(new RoomLightingChangedDomainEvent(this, lighting));
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

    public class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
    {
        public void Configure(EntityTypeBuilder<RoomEntity> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired();

            builder.OwnsOne(r => r.Lighting, Lighting.Configure);

            builder.Ignore(r => r.Lights);
            builder.HasMany(r => r.Devices)
                .WithOne(d => d.Room);
        }
    }
}