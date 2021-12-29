using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Cqrs.DomainEvents;

internal class LoggingDomainEventBus : IDomainEventBus
{
    private readonly IDomainEventBus _domainEventBus;
    private readonly ILogger<LoggingDomainEventBus> _logger;

    public LoggingDomainEventBus(IDomainEventBus domainEventBus, ILogger<LoggingDomainEventBus> logger)
    {
        _domainEventBus = domainEventBus;
        _logger = logger;
    }

    public void Enqueue(IDomainEvent domainEvent)
    {
        _logger.LogInformation("Enqueuing domain event {EventName}...", domainEvent.GetType().Name);
        _domainEventBus.Enqueue(domainEvent);
    }

    public async Task FlushAsync(CancellationToken token = default)
    {
        _logger.LogInformation("Flushing domain events...");
        await _domainEventBus.FlushAsync(token).ConfigureAwait(false);
        _logger.LogInformation("Finished flushing domain events");
    }
}