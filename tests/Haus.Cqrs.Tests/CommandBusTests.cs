using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Cqrs.Tests;

public class CommandBusTests
{
    [Fact]
    public async Task WhenNoHandlerIsRegisteredThenExecuteCommandAsyncThrows()
    {
        var bus = BuildBus(typeof(CommandBusTests).Assembly);

        var act = () => bus.ExecuteCommandAsync(new UnhandledCommand());

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task WhenMultipleHandlersAreRegisteredThenExecuteCommandAsyncThrows()
    {
        var bus = BuildBus(typeof(CommandBusTests).Assembly);

        var act = () => bus.ExecuteCommandAsync(new AmbiguousCommand());

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task WhenASingleHandlerIsRegisteredThenExecuteCommandAsyncWithResultReturnsItsResult()
    {
        var bus = BuildBus(typeof(CommandBusTests).Assembly);

        var result = await bus.ExecuteCommandAsync(new AnsweredCommand());

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

    private record UnhandledCommand : ICommand;

    private record AmbiguousCommand : ICommand;

    private class FirstAmbiguousCommandHandler : ICommandHandler<AmbiguousCommand>
    {
        public Task Handle(AmbiguousCommand command, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private class SecondAmbiguousCommandHandler : ICommandHandler<AmbiguousCommand>
    {
        public Task Handle(AmbiguousCommand command, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private record AnsweredCommand : ICommand<int>;

    private class AnsweredCommandHandler : ICommandHandler<AnsweredCommand, int>
    {
        public Task<int> Handle(AnsweredCommand command, CancellationToken cancellationToken) => Task.FromResult(42);
    }
}
