using System;
using System.Collections.Generic;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;

namespace Haus.Core.Tests.Support
{
    public static class HausDbContextExtensions
    {
        public static DeviceEntity AddDevice(this HausDbContext context, string externalId, Action<DeviceEntity> configure = null)
        {
            var device = new DeviceEntity {ExternalId = externalId};
            configure?.Invoke(device);
            context.Add(device);
            context.SaveChanges();
            return device;
        }
    }
}