using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Devices.Entities;
using Haus.Core.Rooms.Entities;
using Haus.Cqrs.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Common.Storage.Commands;

public record ClearDatabaseCommand : ICommand;

internal class ClearDatabaseCommandHandler(HausDbContext context) : ICommandHandler<ClearDatabaseCommand>
{
    public async Task Handle(ClearDatabaseCommand request, CancellationToken cancellationToken)
    {
        await ClearAllAsync<DeviceMetadataEntity>(cancellationToken).ConfigureAwait(false);
        await ClearAllAsync<DeviceEntity>(cancellationToken).ConfigureAwait(false);
        await ClearAllAsync<RoomEntity>(cancellationToken).ConfigureAwait(false);
    }

    private async Task ClearAllAsync<T>(CancellationToken token)
        where T : class
    {
        var entities = await context.Set<T>().ToArrayAsync(token).ConfigureAwait(false);
        foreach (var entity in entities)
            context.Remove(entity);
        await context.SaveChangesAsync(token).ConfigureAwait(false);
    }
}
