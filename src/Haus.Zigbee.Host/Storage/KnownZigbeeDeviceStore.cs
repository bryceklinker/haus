using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Host.Storage.Entities;
using Haus.Zigbee.Models;
using Microsoft.EntityFrameworkCore;

namespace Haus.Zigbee.Host.Storage;

public class KnownZigbeeDeviceStore(IDbContextFactory<HausZigbeeDbContext> contextFactory) : IKnownZigbeeDeviceStore
{
    public async Task<IReadOnlyList<KnownZigbeeDeviceRecord>> GetAllAsync(CancellationToken token = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(token);
        var entities = await context.KnownDevices.AsNoTracking().ToListAsync(token);
        return entities
            .Select(e => new KnownZigbeeDeviceRecord(
                e.ExternalId,
                e.NetworkAddress,
                DeserializeEndpoints(e.EndpointsJson),
                e.IsSleepy
            ))
            .ToList();
    }

    public async Task UpsertNetworkAddressAsync(RegisterNetworkAddress args, CancellationToken token = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(token);
        var entity = await FindOrCreateAsync(context, args.ExternalId, token);
        entity.NetworkAddress = args.NetworkAddress;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(token);
    }

    public async Task UpsertAsync(RegisterKnownDevice args, CancellationToken token = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(token);
        var entity = await FindOrCreateAsync(context, args.ExternalId, token);
        entity.NetworkAddress = args.NetworkAddress;
        entity.EndpointsJson = JsonSerializer.Serialize(args.Endpoints);
        entity.IsSleepy = args.IsSleepy;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync(token);
    }

    private static IReadOnlyList<ZigbeeEndpoint> DeserializeEndpoints(string endpointsJson)
    {
        return JsonSerializer.Deserialize<IReadOnlyList<ZigbeeEndpoint>>(endpointsJson) ?? [];
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
