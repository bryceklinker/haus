using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Discovery.Commands;
using Haus.Core.Discovery.Entities;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Discovery.Commands;

public class StopDiscoveryCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;
    private readonly StopDiscoveryCommand _command;

    public StopDiscoveryCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _context.AddDiscovery(DiscoveryState.Enabled);

        _command = new StopDiscoveryCommand();
        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDiscoveryIsStoppedThenDiscoveryIsDisabled()
    {
        await _hausBus.ExecuteCommandAsync(_command);

        _context.Set<DiscoveryEntity>().Should().HaveCount(1)
            .And.ContainEquivalentOf(new DiscoveryEntity(), opts => opts.Excluding(d => d.Id));
    }

    [Fact]
    public async Task WhenDiscoveryIsStoppedThenStopDiscoveryHausCommandIsPublished()
    {
        await _hausBus.ExecuteCommandAsync(_command);

        var routableCommands = _hausBus.GetPublishedEvents<RoutableCommand>();
        routableCommands.Should().Contain(r => r.HausCommand.Type == StopDiscoveryModel.Type);
    }

    [Fact]
    public async Task WhenDiscoveryStoppedThenDiscoveryStoppedEventIsPublished()
    {
        var command = new StopDiscoveryCommand();

        await _hausBus.ExecuteCommandAsync(command);

        _hausBus.GetPublishedRoutableEvents<DiscoveryStoppedEvent>().Should().HaveCount(1);
    }
}