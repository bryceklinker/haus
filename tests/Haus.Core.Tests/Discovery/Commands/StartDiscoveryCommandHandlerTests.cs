using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Common.Storage;
using Haus.Core.Discovery.Commands;
using Haus.Core.Discovery.Entities;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Discovery.Commands;

public class StartDiscoveryCommandHandlerTests
{
    private readonly HausDbContext _context;
    private readonly CapturingHausBus _hausBus;

    public StartDiscoveryCommandHandlerTests()
    {
        _context = HausDbContextFactory.Create();
        _context.AddDiscovery();

        _hausBus = HausBusFactory.CreateCapturingBus(_context);
    }

    [Fact]
    public async Task WhenDiscoveryIsStartedThenDiscoveryStateIsEnabled()
    {
        var command = new StartDiscoveryCommand();

        await _hausBus.ExecuteCommandAsync(command);

        var entity = Assert.Single(_context.Set<DiscoveryEntity>());
        Assert.Equal(DiscoveryState.Enabled, entity.State);
    }

    [Fact]
    public async Task WhenDiscoveryIsStartedThenStartDiscoveryHausCommandIsPublished()
    {
        var command = new StartDiscoveryCommand();

        await _hausBus.ExecuteCommandAsync(command);

        var routableCommands = _hausBus.GetPublishedEvents<RoutableCommand>();
        Assert.Contains(routableCommands, r => r.HausCommand.Type == StartDiscoveryModel.Type);
    }

    [Fact]
    public async Task WhenDiscoveryStartedThenDiscoveryStartedEventPublished()
    {
        var command = new StartDiscoveryCommand();

        await _hausBus.ExecuteCommandAsync(command);

        Assert.Single(_hausBus.GetPublishedRoutableEvents<DiscoveryStartedEvent>());
    }
}
