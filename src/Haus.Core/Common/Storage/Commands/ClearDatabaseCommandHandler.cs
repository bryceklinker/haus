using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Common.Storage.Commands
{
    public record ClearDatabaseCommand : ICommand;

    internal class ClearDatabaseCommandHandler : AsyncRequestHandler<ClearDatabaseCommand>, ICommandHandler<ClearDatabaseCommand>
    {
        private readonly HausDbContext _context;

        public ClearDatabaseCommandHandler(HausDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(ClearDatabaseCommand request, CancellationToken cancellationToken)
        {
            await ClearAllAsync<DeviceMetadataEntity>(cancellationToken);
            await ClearAllAsync<DeviceEntity>(cancellationToken);
            await ClearAllAsync<RoomEntity>(cancellationToken);
        }

        private async Task ClearAllAsync<T>(CancellationToken token)
            where T : class
        {
            var entities = await _context.Set<T>().ToArrayAsync(token).ConfigureAwait(false);
            foreach (var entity in entities) _context.Remove(entity);
            await _context.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}