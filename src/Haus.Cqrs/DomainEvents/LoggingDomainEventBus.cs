using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs.DomainEvents;

internal class LoggingDomainEventBus(IDomainEventBus domainEventBus, ILogger<LoggingDomainEventBus> logger)
    : IDomainEventBus
{
    public void Enqueue(IDomainEvent domainEvent)
    {
        logger.LogInformation("Enqueuing domain event {EventName}...", domainEvent.GetType().Name);
        domainEventBus.Enqueue(domainEvent);
    }

    public async Task FlushAsync(CancellationToken token = default)
    {
        logger.LogInformation("Flushing domain events...");
        await domainEventBus.FlushAsync(token).ConfigureAwait(false);
        logger.LogInformation("Finished flushing domain events");
    }
}
