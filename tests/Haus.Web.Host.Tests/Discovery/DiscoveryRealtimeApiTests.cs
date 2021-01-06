using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Discovery
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DiscoveryRealtimeApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;

        public DiscoveryRealtimeApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task WhenDiscoveryIsStartedThenEventsGetsNotified()
        {
            var hub = await _factory.CreateHubConnection("events");

            var events = new List<HausEvent>();
            hub.On<HausEvent>("OnEvent", msg => events.Add(msg));

            await _factory.CreateAuthenticatedClient().StartDiscoveryAsync();
            Eventually.Assert(() =>
            {
                events.Should().Contain(e => e.Type == DiscoveryStartedEvent.Type);
            });
        }
    }
}