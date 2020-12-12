using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
    public class StopDiscoveryCommandHandlerTests
    {
        private readonly CapturingHausBus _capturingHausBus;

        public StopDiscoveryCommandHandlerTests()
        {
            _capturingHausBus = HausBusFactory.CreateCapturingBus();
        }

        [Fact]
        public async Task WhenDiscoveryIsStoppedThenStopDiscoveryHausCommandIsPublished()
        {
            var command = new StopDiscoveryCommand();

            await _capturingHausBus.ExecuteCommandAsync(command);

            var routableCommands = _capturingHausBus.GetPublishedEvents<RoutableCommand>();
            Assert.Contains(routableCommands, r => r.HausCommand.Type == StopDiscoveryModel.Type);
        }
    }
}