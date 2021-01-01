using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Devices.Commands;
using Haus.Testing.Support;
using Haus.Web.Host.Devices;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Web.Host.Tests.Devices
{
    public class SyncDevicesBackgroundServiceTests
    {
        private readonly SyncDevicesBackgroundService _service;
        private readonly CapturingHausBus _hausBus;

        public SyncDevicesBackgroundServiceTests()
        {
            _hausBus = HausBusFactory.CreateCapturingBus();

            _service = new SyncDevicesBackgroundService(_hausBus, new NullLogger<SyncDevicesBackgroundService>());
        }
        
        [Fact]
        public async Task WhenStartedThenExecutesSyncDevicesCommand()
        {
            await _service.StartAsync(CancellationToken.None);

            _hausBus.GetExecutedCommands<SyncDiscoveryCommand>().Should().HaveCount(1);
        }
    }
}