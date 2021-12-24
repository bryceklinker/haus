using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.Entities;
using Haus.Core.Lighting.Entities;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Lighting;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.DomainEvents;
using Haus.Cqrs.DomainEvents;

namespace Haus.Core.Rooms.Entities
{
    public record RoomEntity : Entity
    {
        public static readonly Expression<Func<RoomEntity, RoomModel>> ToModelExpression =
            r => new RoomModel(
                r.Id,
                r.Name,
                r.OccupancyTimeoutInSeconds,
                new LightingModel(
                    r.Lighting.State,
                    new LevelLightingModel(r.Lighting.Level.Value, r.Lighting.Level.Min, r.Lighting.Level.Max),
                    new TemperatureLightingModel(r.Lighting.Temperature.Value, r.Lighting.Temperature.Min, r.Lighting.Temperature.Max),
                    new ColorLightingModel(r.Lighting.Color.Red, r.Lighting.Color.Green, r.Lighting.Color.Blue)
                )
            );


        private static readonly Lazy<Func<RoomEntity, RoomModel>> ToModelFunc = new(ToModelExpression.Compile);

        public string Name { get; set; }
        public int OccupancyTimeoutInSeconds { get; set; }

        public DateTime? LastOccupiedTime { get; set; }
        
        public LightingEntity Lighting { get; set; }

        public ICollection<DeviceEntity> Devices { get; set; }

        public IEnumerable<DeviceEntity> Lights => Devices.Where(d => d.IsLight);

        public RoomEntity()
            : this(0, null)
        {
        }

        public RoomEntity(
            long id,
            string name,
            int occupancyTimeoutInSeconds = RoomDefaults.OccupancyTimeoutInSeconds,
            DateTime? lastOccupiedTime = null,
            LightingEntity lighting = null,
            ICollection<DeviceEntity> devices = null)
        {
            Id = id;
            OccupancyTimeoutInSeconds = occupancyTimeoutInSeconds;
            LastOccupiedTime = lastOccupiedTime;
            Name = name ?? string.Empty;
            Lighting = lighting ?? new LightingEntity(LightingDefaults.State, new LevelLightingEntity(), new TemperatureLightingEntity(), new ColorLightingEntity());
            Devices = devices ?? new List<DeviceEntity>();
        }

        public RoomModel ToModel() => ToModelFunc.Value(this);

        public static RoomEntity CreateFromModel(RoomModel model)
        {
            return new RoomEntity
            {
                Name = model.Name, 
                OccupancyTimeoutInSeconds = model.OccupancyTimeoutInSeconds
            };
        }

        public void UpdateFromModel(RoomModel roomModel)
        {
            Name = roomModel.Name;
            OccupancyTimeoutInSeconds = roomModel.OccupancyTimeoutInSeconds;
        }

        public void AddDevice(DeviceEntity device, IDomainEventBus domainEventBus)
        {
            if (Devices.Any(d => d.Id == device.Id))
                return;

            Devices.Add(device);
            device.AssignToRoom(this, domainEventBus);
        }

        public void AddDevices(IEnumerable<DeviceEntity> devices, IDomainEventBus domainEventBus)
        {
            foreach (var device in devices)
                AddDevice(device, domainEventBus);
        }

        public void RemoveDevice(DeviceEntity device)
        {
            Devices.Remove(device);
            device.UnassignFromRoom();
        }

        public void ChangeLighting(LightingEntity lighting, IDomainEventBus domainEventBus)
        {
            Lighting = LightingEntity.CalculateTarget(Lighting, lighting);
            foreach (var light in Lights)
                light.ChangeLighting(Lighting, domainEventBus);

            domainEventBus.Enqueue(new RoomLightingChangedDomainEvent(this, Lighting));
        }

        public void TurnOff(IDomainEventBus domainEventBus)
        {
            var lightingCopy = LightingEntity.FromEntity(Lighting);
            lightingCopy.State = LightingState.Off;
            ChangeLighting(lightingCopy, domainEventBus);
        }

        public void TurnOn(IDomainEventBus domainEventBus)
        {
            var lightingCopy = LightingEntity.FromEntity(Lighting);
            lightingCopy.State = LightingState.On;
            ChangeLighting(lightingCopy, domainEventBus);
        }

        public void ChangeOccupancy(OccupancyChangedModel model, IDomainEventBus domainEventBus)
        {
            if (model.Occupancy) 
                HandleRoomOccupied(domainEventBus);
            else
                HandleRoomVacant(domainEventBus);
        }

        private void HandleRoomVacant(IDomainEventBus domainEventBus)
        {
            var lastOccupied = LastOccupiedTime.GetValueOrDefault();
            if (lastOccupied.AddSeconds(OccupancyTimeoutInSeconds) > DateTime.UtcNow)
                return;

            TurnOff(domainEventBus);
        }

        private void HandleRoomOccupied(IDomainEventBus domainEventBus)
        {
            LastOccupiedTime = DateTime.UtcNow;
            TurnOn(domainEventBus);
        }
    }
}