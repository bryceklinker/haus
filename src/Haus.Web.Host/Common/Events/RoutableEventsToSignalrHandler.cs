using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Cqrs.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Haus.Web.Host.Common.Events
{
    internal class RoutableEventsToSignalrHandler<T> : IEventHandler<T>
        where T : RoutableEvent
    {
        private readonly IHubContext<EventsHub> _hub;

        public RoutableEventsToSignalrHandler(IHubContext<EventsHub> hub)
        {
            _hub = hub;
        }

        public Task Handle(T notification, CancellationToken cancellationToken)
        {
            return _hub.BroadcastAsync("OnEvent", notification);
        }
    }
}