using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Cqrs.Commands;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Cqrs.Tests;

public class CommandBusExceptionTests
{
    [Fact]
    public async Task WhenHandlerThrowsSynchronouslyThenOriginalExceptionTypePropagates()
    {
        var bus = new ServiceCollection()
            .AddLogging()
            .AddHausCqrs(typeof(CommandBusExceptionTests).Assembly)
            .BuildServiceProvider()
            .GetRequiredService<IHausBus>();

        var act = () => bus.ExecuteCommandAsync(new ThrowingCommand());

        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    private record ThrowingCommand : ICommand;

    private class ThrowingCommandHandler : ICommandHandler<ThrowingCommand>
    {
        public Task Handle(ThrowingCommand command, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("boom");
        }
    }
}
