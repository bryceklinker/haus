using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Zigbee.Host.Storage;

public class KnownZigbeeDeviceStore(IDbContextFactory<HausZigbeeDbContext> contextFactory) : IKnownZigbeeDeviceStore
{
    public async Task<IReadOnlyList<KnownZigbeeDeviceRecord>> GetAllAsync(CancellationToken token = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(token);
        return await context
            .KnownDevices.AsNoTracking()
            .Select(e => new KnownZigbeeDeviceRecord(e.ExternalId, e.NetworkAddress, e.EndpointsJson, e.IsSleepy))
            .ToListAsync(token);
    }

    public async Task UpsertNetworkAddressAsync(UpsertNetworkAddressArgs args, CancellationToken token = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(token);
        var entity = await FindOrCreateAsync(context, args.ExternalId, token);
        entity.NetworkAddress = args.NetworkAddress;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(token);
    }

    public async Task UpsertAsync(UpsertKnownZigbeeDeviceArgs args, CancellationToken token = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(token);
        var entity = await FindOrCreateAsync(context, args.ExternalId, token);
        entity.NetworkAddress = args.NetworkAddress;
        entity.EndpointsJson = args.EndpointsJson;
        entity.IsSleepy = args.IsSleepy;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(token);
    }

    private static async Task<KnownZigbeeDeviceEntity> FindOrCreateAsync(
        HausZigbeeDbContext context,
        string externalId,
        CancellationToken token
    )
    {
        var existing = await context.KnownDevices.SingleOrDefaultAsync(e => e.ExternalId == externalId, token);
        if (existing is not null)
            return existing;

        var created = new KnownZigbeeDeviceEntity { ExternalId = externalId };
        context.KnownDevices.Add(created);
        return created;
    }
}
