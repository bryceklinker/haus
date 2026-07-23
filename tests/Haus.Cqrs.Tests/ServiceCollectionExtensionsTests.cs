using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Cqrs.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public async Task WhenAnOpenGenericHandlerIsInAScannedAssemblyThenItIsDiscoveredForEachConcreteEventType()
    {
        var bus = new ServiceCollection()
            .AddLogging()
            .AddHausCqrs(typeof(ServiceCollectionExtensionsTests).Assembly)
            .BuildServiceProvider()
            .GetRequiredService<IHausBus>();
        var handled = new ConcurrentBag<object>();

        await bus.PublishAsync(new FirstGenericEvent(handled));
        await bus.PublishAsync(new SecondGenericEvent(handled));

        Assert.Equal(2, handled.Count);
    }

    private interface ITracksHandling
    {
        ConcurrentBag<object> Handled { get; }
    }

    private record FirstGenericEvent(ConcurrentBag<object> Handled) : IEvent, ITracksHandling;

    private record SecondGenericEvent(ConcurrentBag<object> Handled) : IEvent, ITracksHandling;

    private class GenericEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent, ITracksHandling
    {
        public Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            @event.Handled.Add(@event);
            return Task.CompletedTask;
        }
    }
}
