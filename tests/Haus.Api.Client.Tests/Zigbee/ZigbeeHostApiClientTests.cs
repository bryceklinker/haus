using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Haus.Api.Client.Options;
using Haus.Api.Client.Zigbee;
using Haus.Zigbee.Models.Diagnostics;
using Xunit;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace Haus.Api.Client.Tests.Zigbee;

public class ZigbeeHostApiClientTests
{
    private readonly ZigbeeHostApiClient _client = new(
        new HttpClient(new UnreachableHostHandler()) { BaseAddress = new Uri("http://unreachable-zigbee-host") },
        MsOptions.Create(new ZigbeeHostApiClientSettings { BaseUrl = "http://unreachable-zigbee-host" })
    );

    [Fact]
    public async Task WhenZigbeeHostUnreachableThenGetStatusAsyncReturnsUnknown()
    {
        var status = await _client.GetStatusAsync();

        Assert.Equal(ZigbeeConnectionStatusModel.Unknown, status);
    }

    [Fact]
    public async Task WhenZigbeeHostUnreachableThenGetActivityAsyncReturnsEmpty()
    {
        var activity = await _client.GetActivityAsync();

        Assert.Empty(activity);
    }

    [Fact]
    public async Task WhenZigbeeHostUnreachableThenGetDevicesAsyncReturnsEmpty()
    {
        var devices = await _client.GetDevicesAsync();

        Assert.Empty(devices);
    }

    private class UnreachableHostHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            throw new HttpRequestException("Connection refused");
        }
    }
}
