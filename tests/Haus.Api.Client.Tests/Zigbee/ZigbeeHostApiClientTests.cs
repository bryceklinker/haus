using System;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Options;
using Haus.Api.Client.Zigbee;
using Haus.Testing.Support.Http;
using Haus.Zigbee.Models.Diagnostics;
using Xunit;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace Haus.Api.Client.Tests.Zigbee;

public class ZigbeeHostApiClientTests
{
    private readonly InMemoryHttpMessageHandler _handler = new();
    private readonly ZigbeeHostApiClient _client;

    public ZigbeeHostApiClientTests()
    {
        _client = new ZigbeeHostApiClient(
            new HttpClient(_handler),
            MsOptions.Create(new ZigbeeHostApiClientSettings { BaseUrl = ConfigureHttpResponseOptions.DefaultBaseUrl })
        );
    }

    [Fact]
    public async Task WhenZigbeeHostUnreachableThenGetStatusAsyncReturnsUnknown()
    {
        _handler.SetupGetThrows("/api/zigbee/status", new HttpRequestException("Connection refused"));

        var status = await _client.GetStatusAsync();

        Assert.Equal(ZigbeeConnectionStatusModel.Unknown, status);
    }

    [Fact]
    public async Task WhenZigbeeHostUnreachableThenGetActivityAsyncReturnsEmpty()
    {
        _handler.SetupGetThrows("/api/zigbee/activity", new HttpRequestException("Connection refused"));

        var activity = await _client.GetActivityAsync();

        Assert.Empty(activity);
    }

    [Fact]
    public async Task WhenZigbeeHostUnreachableThenGetDevicesAsyncReturnsEmpty()
    {
        _handler.SetupGetThrows("/api/zigbee/devices", new HttpRequestException("Connection refused"));

        var devices = await _client.GetDevicesAsync();

        Assert.Empty(devices);
    }

    [Fact]
    public async Task WhenZigbeeHostReturnsStatusThenGetStatusAsyncReturnsIt()
    {
        var canned = new ZigbeeConnectionStatusModel(
            true,
            "connected",
            DateTimeOffset.Parse("2026-01-02T03:04:05+00:00"),
            new ZigbeeNetworkConfigModel("0x00124b0001ce4b11", 6754, 15)
        );
        await _handler.SetupGetAsJson("/api/zigbee/status", canned);

        var status = await _client.GetStatusAsync();

        Assert.Equal(canned, status);
    }

    [Fact]
    public async Task WhenZigbeeHostReturnsActivityThenGetActivityAsyncReturnsIt()
    {
        var canned = new[]
        {
            new ZigbeeActivityEntryModel(
                "zigbee_device_joined",
                DateTimeOffset.Parse("2026-01-02T03:04:05+00:00"),
                new { ieeeAddress = "0x00124b0001ce4b11" }
            ),
        };
        await _handler.SetupGetAsJson("/api/zigbee/activity", canned);

        var activity = await _client.GetActivityAsync();

        var entry = Assert.Single(activity);
        Assert.Equal("zigbee_device_joined", entry.EventType);
        Assert.Equal(DateTimeOffset.Parse("2026-01-02T03:04:05+00:00"), entry.OccurredAt);
    }

    [Fact]
    public async Task WhenZigbeeHostReturnsDevicesThenGetDevicesAsyncReturnsThem()
    {
        var canned = new[]
        {
            new ZigbeeKnownDeviceModel(
                "0x00124b0001ce4b11",
                4455,
                [new ZigbeeEndpointModel(1, 260, 256, [6], [])],
                IsSleepy: true
            ),
        };
        await _handler.SetupGetAsJson("/api/zigbee/devices", canned);

        var devices = await _client.GetDevicesAsync();

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
}
