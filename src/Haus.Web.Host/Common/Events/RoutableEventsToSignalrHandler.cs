using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Cqrs.Events;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.Common.Events;

internal class RoutableEventsToSignalrHandler<T>(IHubContext<EventsHub> hub) : IEventHandler<T>
    where T : RoutableEvent
{
    public Task Handle(T notification, CancellationToken cancellationToken)
    {
        return hub.BroadcastAsync("OnEvent", notification);
    }
}
