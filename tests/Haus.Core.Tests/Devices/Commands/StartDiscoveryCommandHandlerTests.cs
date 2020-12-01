using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
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
            Assert.Contains(routableCommands, r => r.HausCommand.Type == StartDiscoveryModel.Type);
        }
    }
}