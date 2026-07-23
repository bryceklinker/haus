using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Queries;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Cqrs.Tests;

public class QueryBusTests
{
    [Fact]
    public async Task WhenNoHandlerIsRegisteredThenExecuteQueryAsyncThrows()
    {
        var bus = BuildBus(typeof(QueryBusTests).Assembly);

        var act = () => bus.ExecuteQueryAsync(new UnhandledQuery());

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task WhenMultipleHandlersAreRegisteredThenExecuteQueryAsyncThrows()
    {
        var bus = BuildBus(typeof(QueryBusTests).Assembly);

        var act = () => bus.ExecuteQueryAsync(new AmbiguousQuery());

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task WhenASingleHandlerIsRegisteredThenExecuteQueryAsyncReturnsItsResult()
    {
        var bus = BuildBus(typeof(QueryBusTests).Assembly);

        var result = await bus.ExecuteQueryAsync(new AnsweredQuery());

        Assert.Equal(42, result);
    }

    private static IHausBus BuildBus(System.Reflection.Assembly assembly)
    {
        return new ServiceCollection()
            .AddLogging()
            .AddHausCqrs(assembly)
            .BuildServiceProvider()
            .GetRequiredService<IHausBus>();
    }

    private record UnhandledQuery : IQuery<int>;

    private record AmbiguousQuery : IQuery<int>;

    private class FirstAmbiguousQueryHandler : IQueryHandler<AmbiguousQuery, int>
    {
        public Task<int> Handle(AmbiguousQuery query, CancellationToken cancellationToken) => Task.FromResult(1);
    }

    private class SecondAmbiguousQueryHandler : IQueryHandler<AmbiguousQuery, int>
    {
        public Task<int> Handle(AmbiguousQuery query, CancellationToken cancellationToken) => Task.FromResult(2);
    }

    private record AnsweredQuery : IQuery<int>;

    private class AnsweredQueryHandler : IQueryHandler<AnsweredQuery, int>
    {
        public Task<int> Handle(AnsweredQuery query, CancellationToken cancellationToken) => Task.FromResult(42);
    }
}
