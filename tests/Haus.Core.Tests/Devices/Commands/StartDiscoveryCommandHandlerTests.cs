using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class StartDiscoveryCommandHandlerTests
    {
        private readonly CapturingHausBus _hausBus;

        public StartDiscoveryCommandHandlerTests()
        {
            _hausBus = HausBusFactory.CreateCapturingBus();
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