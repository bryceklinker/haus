using System.Threading;
using System.Threading.Tasks;

namespace Haus.Cqrs.Events;

public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}
