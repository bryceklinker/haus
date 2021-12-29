using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Discovery.Commands;
using Haus.Core.Models.Discovery;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Discovery.Commands;

public class SyncDiscoveryCommandHandlerTests
{
    private readonly CapturingHausBus _hausBus;

    public SyncDiscoveryCommandHandlerTests()
    {
        _hausBus = HausBusFactory.CreateCapturingBus();
    }

    [Fact]
    public async Task WhenSyncDevicesIsExecutedThenSyncDevicesIsPublished()
    {
        await _hausBus.ExecuteCommandAsync(new SyncDiscoveryCommand());

        _hausBus.GetPublishedHausCommands<SyncDiscoveryModel>().Should().HaveCount(1);
    }
}