using System.Threading.Tasks;
using Haus.Core.Devices.Commands;
using Haus.Core.Models.Devices.Discovery;
using Haus.Core.Tests.Support;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Devices.Commands
{
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

            Assert.Single(_hausBus.GetPublishedHausCommands<SyncDiscoveryModel>());
        }
    }
}