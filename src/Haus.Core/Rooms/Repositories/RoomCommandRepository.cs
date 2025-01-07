using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Entities;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Rooms.Repositories;

public interface IRoomCommandRepository
{
    Task<RoomEntity> GetRoomByDeviceExternalId(string externalId, CancellationToken cancellationToken = default);
    Task<RoomEntity> GetByIdAsync(long roomId, CancellationToken cancellationToken = default);
    Task<RoomEntity> AddAsync(RoomEntity room, CancellationToken cancellationToken = default);
    Task SaveAsync(RoomEntity room, CancellationToken cancellationToken = default);
}

public class RoomCommandRepository(HausDbContext context) : IRoomCommandRepository
{
    public Task<RoomEntity> GetRoomByDeviceExternalId(string externalId, CancellationToken cancellationToken = default)
    {
        return context.GetAll<DeviceEntity>()
            .Where(d => d.ExternalId == externalId)
            .Include(d => d.Room)
            .ThenInclude(r => r.Devices)
            .Select(d => d.Room)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<RoomEntity> GetByIdAsync(long roomId, CancellationToken cancellationToken = default)
    {
        return context.FindByIdOrThrowAsync<RoomEntity>(roomId,
            query => query.Include(r => r.Devices)
                .ThenInclude(d => d.Metadata),
            cancellationToken);
    }

    public async Task<RoomEntity> AddAsync(RoomEntity room, CancellationToken cancellationToken = default)
    {
        context.Add(room);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return room;
    }

    public Task SaveAsync(RoomEntity room, CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}