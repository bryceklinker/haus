using System.Threading;
using System.Threading.Tasks;

namespace Haus.Cqrs.DomainEvents;

public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
}
