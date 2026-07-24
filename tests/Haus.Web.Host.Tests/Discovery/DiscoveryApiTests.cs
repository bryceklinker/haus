using System.Threading.Tasks;
using Haus.Api.Client;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.ExternalMessages;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Xunit;

namespace Haus.Web.Host.Tests.Discovery;

[Collection(HausWebHostCollectionFixture.Name)]
public class DiscoveryApiTests(HausWebHostApplicationFactory factory)
{
    private readonly IHausApiClient _client = factory.CreateAuthenticatedClient();

    [Fact]
    public async Task WhenDiscoveryStartedThenDiscoveryIsEnabled()
    {
        await _client.StartDiscoveryAsync();

        var model = await _client.GetDiscoveryStateAsync();

        if (model != null)
        {
            Assert.Equal(DiscoveryState.Enabled, model.State);
        }
    }

    [Fact]
    public async Task WhenDiscoveryStoppedThenDiscoveryIsDisabled()
    {
        await _client.StartDiscoveryAsync();
        await _client.StopDiscoveryAsync();

        var model = await _client.GetDiscoveryStateAsync();

        if (model != null)
        {
            Assert.Equal(DiscoveryState.Disabled, model.State);
        }
    }

    [Fact]
    public async Task WhenDiscoveryIsStartedThenStartDiscoveryCommandIsPublished()
    {
        HausCommand<StartDiscoveryModel>? hausCommand = null;
        await factory.SubscribeToHausCommandsAsync<StartDiscoveryModel>(
            StartDiscoveryModel.Type,
            cmd => hausCommand = cmd
        );

        await _client.StartDiscoveryAsync();

        Eventually.Assert(() =>
        {
            if (hausCommand != null)
            {
                Assert.Equal(StartDiscoveryModel.Type, hausCommand.Type);
            }
        });
    }

    [Fact]
    public async Task WhenDiscoveryStoppedThenStopDiscoveryCommandIsPublished()
    {
        HausCommand<StopDiscoveryModel>? hausCommand = null;
        await factory.SubscribeToHausCommandsAsync<StopDiscoveryModel>(
            StopDiscoveryModel.Type,
            cmd => hausCommand = cmd
        );

        await _client.StopDiscoveryAsync();

        Eventually.Assert(() =>
        {
            if (hausCommand != null)
            {
                Assert.Equal(StopDiscoveryModel.Type, hausCommand.Type);
            }
        });
    }

    [Fact]
    public async Task WhenExternalDevicesAreSyncedThenSyncExternalDevicesIsPublished()
    {
        HausCommand<SyncDiscoveryModel>? command = null;
        await factory.SubscribeToHausCommandsAsync<SyncDiscoveryModel>(SyncDiscoveryModel.Type, cmd => command = cmd);

        var client = factory.CreateAuthenticatedClient();
        await client.SyncDevicesAsync();

        Eventually.Assert(() =>
        {
            if (command != null)
            {
                Assert.Equal(SyncDiscoveryModel.Type, command.Type);
            }
        });
    }
}
