using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Web.Host.Tests.Support;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace Haus.Web.Host.Tests.DeviceSimulator;

[Collection(HausWebHostCollectionFixture.Name)]
public class DeviceSimulatorFeatureGatingTests(HausWebHostApplicationFactory factory)
{
    [Fact]
    public async Task WhenDeviceSimulatorIsDisabledThenControllerEndpointIsUnavailable()
    {
        using var disabledFactory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        var client = disabledFactory.CreateClient();

        var response = await client.PostAsync("/api/device-simulator/reset", new ByteArrayContent([]));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task WhenDeviceSimulatorIsDisabledThenRealtimeHubIsUnavailable()
    {
        using var disabledFactory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Production"));
        disabledFactory.CreateClient();
        var connection = new HubConnectionBuilder()
            .WithUrl(
                "http://localhost/hubs/device-simulator",
                o => o.HttpMessageHandlerFactory = _ => disabledFactory.Server.CreateHandler()
            )
            .Build();

        await Assert.ThrowsAsync<HttpRequestException>(() => connection.StartAsync());
    }
}
