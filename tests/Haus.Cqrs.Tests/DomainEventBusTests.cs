using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Cqrs.Tests;

public class DomainEventBusTests
{
    [Fact]
    public async Task WhenMultipleEventsAreEnqueuedThenFlushAsyncDispatchesEachToAllOfItsHandlersInEnqueueOrder()
    {
        var bus = BuildBus(typeof(DomainEventBusTests).Assembly);
        var dispatched = new List<string>();

        bus.Enqueue(new FirstDomainEvent(dispatched));
        bus.Enqueue(new SecondDomainEvent(dispatched));
        await bus.FlushAsync();

        Assert.Equal(3, dispatched.Count);
        Assert.Contains("first:one", dispatched.Take(2));
        Assert.Contains("first:two", dispatched.Take(2));
        Assert.Equal("second", dispatched[2]);
    }

    private static IHausBus BuildBus(System.Reflection.Assembly assembly)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddHausCqrs(assembly)
            .BuildServiceProvider()
            .GetRequiredService<IHausBus>();
    }

    private record FirstDomainEvent(List<string> Dispatched) : IDomainEvent;

    private class FirstDomainEventFirstHandler : IDomainEventHandler<FirstDomainEvent>
    {
        public Task Handle(FirstDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            domainEvent.Dispatched.Add("first:one");
            return Task.CompletedTask;
        }
    }

    private class FirstDomainEventSecondHandler : IDomainEventHandler<FirstDomainEvent>
    {
        public Task Handle(FirstDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            domainEvent.Dispatched.Add("first:two");
            return Task.CompletedTask;
        }
    }

    private record SecondDomainEvent(List<string> Dispatched) : IDomainEvent;

    private class SecondDomainEventHandler : IDomainEventHandler<SecondDomainEvent>
    {
        public Task Handle(SecondDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            domainEvent.Dispatched.Add("second");
            return Task.CompletedTask;
        }
    }
}
