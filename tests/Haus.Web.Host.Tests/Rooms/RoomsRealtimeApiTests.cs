using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.ExternalMessages;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.Rooms
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class RoomsRealtimeApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;
        private readonly IHausApiClient _client;

        public RoomsRealtimeApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenRoomCreatedThenRoomIsBroadcastToRealtimeConsumers()
        {
            var hub = await _factory.CreateHubConnection("events");
            var items = new List<HausEvent>();
            hub.On<HausEvent>("OnEvent", i => items.Add(i));

            await _client.CreateRoomAsync(new RoomModel(Name: $"{Guid.NewGuid()}"));
            
            Eventually.Assert(() =>
            {
                items.Should().Contain(i => i.Type == RoomCreatedEvent.Type);
            });
        }
    }
}