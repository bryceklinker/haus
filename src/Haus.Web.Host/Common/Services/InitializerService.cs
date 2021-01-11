using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Discovery.Commands;
using Haus.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haus.Web.Host.Common.Services
{
    public class InitializerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public InitializerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var bus = scope.GetService<IHausBus>();
            await Task.WhenAll(
                bus.ExecuteCommandAsync(new InitializeDiscoveryCommand(), stoppingToken),
                bus.ExecuteCommandAsync(new SyncDiscoveryCommand(), stoppingToken)
            ).ConfigureAwait(false);
        }
    }
}