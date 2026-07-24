using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Cqrs.Tests;

public class EventBusTests
{
    [Fact]
    public async Task WhenNoHandlerIsRegisteredThenPublishAsyncCompletesWithoutThrowing()
    {
        var bus = BuildBus(typeof(EventBusTests).Assembly);

        await bus.PublishAsync(new UnhandledEvent());
    }

    [Fact]
    public async Task WhenMultipleHandlersAreRegisteredThenPublishAsyncInvokesAllOfThem()
    {
        var bus = BuildBus(typeof(EventBusTests).Assembly);
        var invokedBy = new ConcurrentBag<string>();

        await bus.PublishAsync(new FannedOutEvent(invokedBy));

        Assert.Equal(["first", "second"], invokedBy.OrderBy(name => name));
    }

    private static IHausBus BuildBus(System.Reflection.Assembly assembly)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddHausCqrs(assembly)
            .BuildServiceProvider()
            .GetRequiredService<IHausBus>();
    }

    private record UnhandledEvent : IEvent;

    private record FannedOutEvent(ConcurrentBag<string> InvokedBy) : IEvent;

    private class FirstFannedOutEventHandler : IEventHandler<FannedOutEvent>
    {
        public Task Handle(FannedOutEvent @event, CancellationToken cancellationToken)
        {
            @event.InvokedBy.Add("first");
            return Task.CompletedTask;
        }
    }

    private class SecondFannedOutEventHandler : IEventHandler<FannedOutEvent>
    {
        public Task Handle(FannedOutEvent @event, CancellationToken cancellationToken)
        {
            @event.InvokedBy.Add("second");
            return Task.CompletedTask;
        }
    }
}
