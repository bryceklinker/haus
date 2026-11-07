using System;
using System.Net;
using System.Net.Http;
using System.Text;
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

    [Fact]
    public async Task WhenZigbeeHostReturnsStatusThenGetStatusAsyncReturnsIt()
    {
        var handler = new CannedJsonResponseHandler(
            """
            {
              "isConnected": true,
              "reason": "connected",
              "changedAt": "2026-01-02T03:04:05+00:00",
              "networkConfig": { "macAddress": "0x00124b0001ce4b11", "panId": 6754, "channel": 15 }
            }
            """
        );
        var client = CreateClient(handler);

        var status = await client.GetStatusAsync();

        Assert.Equal("/api/zigbee/status", handler.RequestedPath);
        Assert.NotNull(status);
        Assert.True(status.IsConnected);
        Assert.Equal("connected", status.Reason);
        Assert.Equal(DateTimeOffset.Parse("2026-01-02T03:04:05+00:00"), status.ChangedAt);
        Assert.NotNull(status.NetworkConfig);
        Assert.Equal("0x00124b0001ce4b11", status.NetworkConfig.MacAddress);
        Assert.Equal((ushort)6754, status.NetworkConfig.PanId);
        Assert.Equal((byte)15, status.NetworkConfig.Channel);
    }

    [Fact]
    public async Task WhenZigbeeHostReturnsActivityThenGetActivityAsyncReturnsIt()
    {
        var handler = new CannedJsonResponseHandler(
            """
            [
              { "eventType": "zigbee_device_joined", "occurredAt": "2026-01-02T03:04:05+00:00", "payload": { "ieeeAddress": "0x00124b0001ce4b11" } }
            ]
            """
        );
        var client = CreateClient(handler);

        var activity = await client.GetActivityAsync();

        Assert.Equal("/api/zigbee/activity", handler.RequestedPath);
        var entry = Assert.Single(activity);
        Assert.Equal("zigbee_device_joined", entry.EventType);
        Assert.Equal(DateTimeOffset.Parse("2026-01-02T03:04:05+00:00"), entry.OccurredAt);
    }

    [Fact]
    public async Task WhenZigbeeHostReturnsDevicesThenGetDevicesAsyncReturnsThem()
    {
        var handler = new CannedJsonResponseHandler(
            """
            [
              {
                "ieeeAddress": "0x00124b0001ce4b11",
                "networkAddress": 4455,
                "endpoints": [ { "endpointId": 1, "profileId": 260, "deviceId": 256, "inClusters": [6], "outClusters": [] } ],
                "isSleepy": true
              }
            ]
            """
        );
        var client = CreateClient(handler);

        var devices = await client.GetDevicesAsync();

        Assert.Equal("/api/zigbee/devices", handler.RequestedPath);
        var device = Assert.Single(devices);
        Assert.Equal("0x00124b0001ce4b11", device.IeeeAddress);
        Assert.Equal((ushort)4455, device.NetworkAddress);
        Assert.True(device.IsSleepy);
        var endpoint = Assert.Single(device.Endpoints);
        Assert.Equal((byte)1, endpoint.EndpointId);
        Assert.Equal((ushort)260, endpoint.ProfileId);
        Assert.Equal((ushort)256, endpoint.DeviceId);
        Assert.Equal([(ushort)6], endpoint.InClusters);
        Assert.Empty(endpoint.OutClusters);
    }

    private static ZigbeeHostApiClient CreateClient(HttpMessageHandler handler)
    {
        return new ZigbeeHostApiClient(
            new HttpClient(handler) { BaseAddress = new Uri("http://zigbee-host") },
            MsOptions.Create(new ZigbeeHostApiClientSettings { BaseUrl = "http://zigbee-host" })
        );
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

    private class CannedJsonResponseHandler(string json) : HttpMessageHandler
    {
        public string? RequestedPath { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            RequestedPath = request.RequestUri?.AbsolutePath;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
            return Task.FromResult(response);
        }
    }
}
