using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Health;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Health
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class HealthRealtimeApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;

        public HealthRealtimeApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenListeningForStatusUpdatesThenServiceHealthIsReceived()
        {
            var hub = await _factory.CreateHubConnection("health");
            HausHealthReportModel health = null;
            hub.On<HausHealthReportModel>("OnHealth", h => health = h);
            
            Eventually.Assert(() =>
            {
                health.Should().NotBeNull();
            });
        }
    }
}