using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class StartDiscoveryCommandHandlerTests
    {
        private readonly CapturingHausBus _capturingHausBus;

        public StartDiscoveryCommandHandlerTests()
        {
            _capturingHausBus = HausBusFactory.CreateCapturingBus();
        }

        [Fact]
        public async Task WhenDiscoveryIsStartedThenStartDiscoveryHausCommandIsPublished()
        {
            var command = new StartDiscoveryCommand();
            
            await _capturingHausBus.ExecuteCommandAsync(command);

            var routableCommands = _capturingHausBus.GetPublishedEvents<RoutableCommand>();
            routableCommands.Should().Contain(r => r.HausCommand.Type == StartDiscoveryModel.Type);
        }
    }
}