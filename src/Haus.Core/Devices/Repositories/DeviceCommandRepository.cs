using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Devices.Repositories;

public interface IDeviceCommandRepository
{
    Task<DeviceEntity> GetById(long id, CancellationToken token = default);
    Task<DeviceEntity> GetByExternalId(string externalId, CancellationToken token = default);
    Task<DeviceEntity> AddAsync(DeviceEntity device, CancellationToken token = default);
    Task SaveAsync(DeviceEntity device, CancellationToken token = default);
}

public class DeviceCommandRepository(HausDbContext context) : IDeviceCommandRepository
{
    public Task<DeviceEntity> GetById(long id, CancellationToken token = default)
    {
        return context.FindByIdOrThrowAsync<DeviceEntity>(id, AddIncludes, token);
    }

    public Task<DeviceEntity> GetByExternalId(string externalId, CancellationToken token = default)
    {
        return context.FindByAsync<DeviceEntity>(d => d.ExternalId == externalId, AddIncludes, token);
    }

    public async Task<DeviceEntity> AddAsync(DeviceEntity device, CancellationToken token = default)
    {
        context.Add(device);
        await context.SaveChangesAsync(token).ConfigureAwait(false);
        return device;
    }

    public Task SaveAsync(DeviceEntity device, CancellationToken token = default)
    {
        context.Update(device);
        return context.SaveChangesAsync(token);
    }

    private static IQueryable<DeviceEntity> AddIncludes(IQueryable<DeviceEntity> query)
    {
        return query.Include(d => d.Metadata).Include(d => d.Room);
    }
}
