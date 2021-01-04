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
    public class StopDiscoveryCommandHandlerTests
    {
        private readonly CapturingHausBus _hausBus;

        public StopDiscoveryCommandHandlerTests()
        {
            _hausBus = HausBusFactory.CreateCapturingBus();
        }

        [Fact]
        public async Task WhenDiscoveryIsStoppedThenStopDiscoveryHausCommandIsPublished()
        {
            var command = new StopDiscoveryCommand();

            await _hausBus.ExecuteCommandAsync(command);

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
}