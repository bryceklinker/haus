using System;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Core.Rooms.Entities;

namespace Haus.Core.Tests.Support
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

        private static void AddAndSave<T>(this HausDbContext context, T entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }
    }
}