using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Devices.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Haus.Web.Host.Devices
{
    public class SyncDevicesBackgroundService : BackgroundService
    {
        private readonly IHausBus _hasBus;
        private readonly ILogger<SyncDevicesBackgroundService> _logger;

        public SyncDevicesBackgroundService(IHausBus hasBus, ILogger<SyncDevicesBackgroundService> logger)
        {
            _hasBus = hasBus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _hasBus.ExecuteCommandAsync(new SyncDiscoveryCommand(), stoppingToken);
        }
    }
}