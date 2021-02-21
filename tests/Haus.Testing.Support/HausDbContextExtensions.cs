using System;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Discovery.Entities;
using Haus.Core.Health.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Discovery;
using Haus.Core.Rooms.Entities;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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

        public static DiscoveryEntity AddDiscovery(this HausDbContext context,
            DiscoveryState state = DiscoveryState.Disabled)
        {
            var entity = new DiscoveryEntity(state: state);
            context.AddAndSave(entity);
            return entity;
        }

        public static HealthCheckEntity AddHealthCheck(this HausDbContext context, 
            string name = null,
            Action<HealthCheckEntity> configure = null)
        {
            var check = new HealthCheckEntity
            {
                Name = name ?? $"{Guid.NewGuid()}",
                Status = HealthStatus.Healthy,
                DurationOfCheckInMilliseconds = 0
            };
            configure?.Invoke(check);
            context.AddAndSave(check);
            return check;
        }
        
        private static void AddAndSave<T>(this HausDbContext context, T entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }
    }
}