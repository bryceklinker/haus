using System;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;

namespace Haus.Core.Tests.Support
{
    public static class HausDbContextExtensions
    {
        public static DeviceEntity AddDevice(this HausDbContext context, string externalId = null, string name = null, Action<DeviceEntity> configure = null)
        {
            var device = new DeviceEntity
            {
                Name = name ?? $"{Guid.NewGuid()}",
                ExternalId = externalId ?? $"{Guid.NewGuid()}",
            };
            configure?.Invoke(device);
            context.Add(device);
            context.SaveChanges();
            return device;
        }
    }
}