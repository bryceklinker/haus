using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Haus.Core.Common.Events
{
    public class LoggingEventBus : IEventBus
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<LoggingEventBus> _logger;

        public LoggingEventBus(IEventBus eventBus, ILogger<LoggingEventBus> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken token = default) where TEvent : IEvent
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.LogInformation("Publishing event", @event);
            await _eventBus.PublishAsync(@event, token).ConfigureAwait(false);
            stopwatch.Stop();
            _logger.LogInformation("Finished publishing event", new {@event, stopwatch.ElapsedMilliseconds});
        }
    }
}