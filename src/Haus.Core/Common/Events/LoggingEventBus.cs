using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Common.Events
{
    internal class LoggingEventBus : LoggingBus, IEventBus
    {
        private readonly IEventBus _eventBus;

        protected override string BeginMessage => "Publishing event";
        protected override string FinishedMessage => "Finished publishing event";
        protected override string ErrorMessage => "Error publishing event";

        public LoggingEventBus(IEventBus eventBus, ILogger<LoggingEventBus> logger) 
            : base(logger)
        {
            _eventBus = eventBus;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
        {
            await ExecuteWithLoggingAsync(@event, async () =>
                {
                    await _eventBus.PublishAsync(@event, token).ConfigureAwait(false);
                    return Unit.Value;
                }, token)
                .ConfigureAwait(false);
        }
    }
}