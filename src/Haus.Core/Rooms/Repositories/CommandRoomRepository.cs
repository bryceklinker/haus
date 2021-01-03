using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Storage;
using Haus.Core.Rooms.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Haus.Core.Rooms.Repositories
{
    public interface ICommandRoomRepository
    {
        Task<RoomEntity> GetByIdAsync(long roomId, CancellationToken cancellationToken = default);
        Task<RoomEntity> AddAsync(RoomEntity room, CancellationToken cancellationToken = default);
        Task SaveAsync(RoomEntity room, CancellationToken cancellationToken = default);
    }
    
    public class CommandRoomRepository : ICommandRoomRepository
    {
        private readonly HausDbContext _context;

        public CommandRoomRepository(HausDbContext context)
        {
            _context = context;
        }

        public Task<RoomEntity> GetByIdAsync(long roomId, CancellationToken cancellationToken = default)
        {
            return _context.FindByIdOrThrowAsync<RoomEntity>(roomId,
                query => query.Include(r => r.Devices)
                    .ThenInclude(d => d.Metadata),
                cancellationToken);
        }

        public async Task<RoomEntity> AddAsync(RoomEntity room, CancellationToken cancellationToken = default)
        {
            _context.Add(room);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return room;
        }

        public Task SaveAsync(RoomEntity room, CancellationToken cancellationToken = default)
        {
            _context.Update(room);
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}