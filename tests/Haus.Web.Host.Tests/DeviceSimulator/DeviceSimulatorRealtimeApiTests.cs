using System.Threading.Tasks;
using FluentAssertions;
using Haus.Api.Client;
using Haus.Core.Models.DeviceSimulator;
using Haus.Testing.Support;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.DeviceSimulator;

[Collection(HausWebHostCollectionFixture.Name)]
public class DeviceSimulatorRealtimeApiTests
{
    private readonly IHausApiClient _client;
    private readonly HausWebHostApplicationFactory _factory;

    public DeviceSimulatorRealtimeApiTests(HausWebHostApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task WhenSimulatedDeviceIsAddedThenStateContainsSimulatedDevice()
    {
        await _client.ResetDeviceSimulatorAsync();

        var hub = await _factory.CreateHubConnection("device-simulator");

        DeviceSimulatorStateModel state = null;
        hub.On<DeviceSimulatorStateModel>("OnState", s => state = s);

        await _client.AddSimulatedDeviceAsync(new SimulatedDeviceModel());

        Eventually.Assert(() =>
        {
            state.Devices.Should().HaveCount(1);
        });
    }
}
