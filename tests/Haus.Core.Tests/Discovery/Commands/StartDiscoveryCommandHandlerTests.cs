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

namespace Haus.Core.Tests.Discovery.Commands
{
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

            _context.Set<DiscoveryEntity>().Should().HaveCount(1)
                .And.ContainEquivalentOf(new DiscoveryEntity(0, DiscoveryState.Enabled),
                    opts => opts.Excluding(d => d.Id));
        }
        
        [Fact]
        public async Task WhenDiscoveryIsStartedThenStartDiscoveryHausCommandIsPublished()
        {
            var command = new StartDiscoveryCommand();
            
            await _hausBus.ExecuteCommandAsync(command);

            var routableCommands = _hausBus.GetPublishedEvents<RoutableCommand>();
            routableCommands.Should().Contain(r => r.HausCommand.Type == StartDiscoveryModel.Type);
        }

        [Fact]
        public async Task WhenDiscoveryStartedThenDiscoveryStartedEventPublished()
        {
            var command = new StartDiscoveryCommand();

            await _hausBus.ExecuteCommandAsync(command);

            _hausBus.GetPublishedRoutableEvents<DiscoveryStartedEvent>().Should().HaveCount(1);
        }
    }
}