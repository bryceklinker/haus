using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Storage;
using Haus.Core.Health.Entities;
using Haus.Core.Models.Health;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Health.Commands;

public record StoreHealthReportCommand(HausHealthReportModel Report) : ICommand
{
    public HausHealthCheckModel[] Checks => Report.Checks;
}

internal class StoreHealthReportCommandHandler : ICommandHandler<StoreHealthReportCommand>
{
    private readonly HausDbContext _context;
    private readonly IClock _clock;

    public StoreHealthReportCommandHandler(HausDbContext context, IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public async Task Handle(StoreHealthReportCommand request, CancellationToken cancellationToken)
    {
        foreach (var check in request.Checks)
            await AddOrUpdateHealthCheck(check).ConfigureAwait(false);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task AddOrUpdateHealthCheck(HausHealthCheckModel healthCheck)
    {
        var timestamp = _clock.UtcNowOffset;
        var entity = await _context.FindByAsync<HealthCheckEntity>(e => e.Name == healthCheck.Name)
            .ConfigureAwait(false);

        if (entity == null)
            _context.Add(HealthCheckEntity.FromModel(healthCheck, timestamp));
        else
            entity.UpdateFromModel(healthCheck, timestamp);
    }
}