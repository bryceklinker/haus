using System;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Discovery.Entities;
using Haus.Core.Lighting;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Lighting;
using Haus.Core.Rooms.Entities;

namespace Haus.Testing.Support
{
    public static class HausDbContextExtensions
    {
        public static DeviceEntity AddDevice(this HausDbContext context, string externalId = null, string name = null, DeviceType deviceType = DeviceType.Unknown, Action<DeviceEntity> configure = null)
        {
            var device = new DeviceEntity
            {
                DeviceType = deviceType,
                Name = name ?? $"{Guid.NewGuid()}",
                ExternalId = externalId ?? $"{Guid.NewGuid()}",
            };
            configure?.Invoke(device);
            context.AddAndSave(device);
            return device;
        }

        public static RoomEntity AddRoom(this HausDbContext context, string name = null, Action<RoomEntity> configure = null)
        {
            var room = new RoomEntity
            {
                Name = name ?? $"{Guid.NewGuid()}"
            };
            configure?.Invoke(room);
            context.AddAndSave(room);
            return room;
        }

        public static DefaultLightingConstraintsEntity AddDefaultLightingConstraints(this HausDbContext context,
            double minLevel = LightingDefaults.MinLevel,
            double maxLevel = LightingDefaults.MaxLevel,
            double minTemperature = LightingDefaults.MinTemperature,
            double maxTemperature = LightingDefaults.MaxTemperature)
        {
            var constraints = new DefaultLightingConstraintsEntity
            {
                Constraints = new LightingConstraintsEntity
                {
                    MaxTemperature = maxTemperature,
                    MinTemperature = minTemperature,
                    MaxLevel = maxLevel,
                    MinLevel = minLevel
                }
            };

            context.AddAndSave(constraints);
            return constraints;
        }

        public static DiscoveryEntity AddDiscovery(this HausDbContext context,
            DiscoveryState state = DiscoveryState.Disabled)
        {
            var entity = new DiscoveryEntity(state: state);
            context.AddAndSave(entity);
            return entity;
        }
        
        private static void AddAndSave<T>(this HausDbContext context, T entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }
    }
}