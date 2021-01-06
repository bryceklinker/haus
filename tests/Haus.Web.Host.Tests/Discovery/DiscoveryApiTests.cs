using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Discovery
{
    [Collection(HausWebHostCollectionFixture.Name)]
    public class DiscoveryApiTests
    {
        private readonly HausWebHostApplicationFactory _factory;
        private readonly IHausApiClient _client;

        public DiscoveryApiTests(HausWebHostApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task WhenDiscoveryStartedThenDiscoveryIsEnabled()
        {
            await _client.StartDiscoveryAsync();

            var model = await _client.GetDiscoveryStateAsync();

            model.State.Should().Be(DiscoveryState.Enabled);
        }

        [Fact]
        public async Task WhenDiscoveryStoppedThenDiscoveryIsDisabled()
        {
            await _client.StartDiscoveryAsync();
            await _client.StopDiscoveryAsync();

            var model = await _client.GetDiscoveryStateAsync();

            model.State.Should().Be(DiscoveryState.Disabled);
        }
        
        [Fact]
        public async Task WhenDiscoveryIsStartedThenStartDiscoveryCommandIsPublished()
        {
            HausCommand<StartDiscoveryModel> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<StartDiscoveryModel>(cmd => hausCommand = cmd);

            await _client.StartDiscoveryAsync();

            Eventually.Assert(() =>
            {
                hausCommand.Type.Should().Be(StartDiscoveryModel.Type);
            });
        }

        [Fact]
        public async Task WhenDiscoveryStoppedThenStopDiscoveryCommandIsPublished()
        {
            HausCommand<StopDiscoveryModel> hausCommand = null;
            await _factory.SubscribeToHausCommandsAsync<StopDiscoveryModel>(cmd => hausCommand = cmd);

            await _client.StopDiscoveryAsync();

            Eventually.Assert(() =>
            {
                hausCommand.Type.Should().Be(StopDiscoveryModel.Type);
            });
        }
        
        [Fact]
        public async Task WhenExternalDevicesAreSyncedThenSyncExternalDevicesIsPublished()
        {
            HausCommand<SyncDiscoveryModel> command = null;
            await _factory.SubscribeToHausCommandsAsync<SyncDiscoveryModel>(cmd => command = cmd);

            var client = _factory.CreateAuthenticatedClient();
            await client.SyncDevicesAsync();

            Eventually.Assert(() =>
            {
                command.Type.Should().Be(SyncDiscoveryModel.Type);
            });
        }
    }
}