using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Storage;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Models;
using Haus.Zigbee.Models.Diagnostics;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee;

public class ZigbeeDiagnosticsApiTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HausZigbeeHostApplicationFactory _factory = new();
    private readonly System.Net.Http.HttpClient _client;

    public ZigbeeDiagnosticsApiTests()
    {
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task WhenNoConnectionReportedThenStatusIsUnknown()
    {
        var response = await _client.GetAsync("/api/zigbee/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var status = await response.Content.ReadFromJsonAsync<ZigbeeConnectionStatusModel>(JsonOptions);
        Assert.NotNull(status);
        Assert.False(status.IsConnected);
    }

    [Fact]
    public async Task WhenConnectionStatusRaisedThenStatusEndpointReflectsIt()
    {
        _factory.Coordinator.RaiseConnectionStatusChanged(
            new ZigbeeConnectionStatus(true, new NetworkConfig(new IeeeAddress(0x1), 0x1a62, 11), "connected")
        );

        var status = await _client.GetFromJsonAsync<ZigbeeConnectionStatusModel>("/api/zigbee/status", JsonOptions);

        Assert.NotNull(status);
        Assert.True(status.IsConnected);
        Assert.Equal("connected", status.Reason);
        Assert.NotNull(status.NetworkConfig);
        Assert.Equal((ushort)0x1a62, status.NetworkConfig.PanId);
    }

    [Fact]
    public async Task WhenCoordinatorEventRaisedThenActivityEndpointHasIt()
    {
        _factory.Coordinator.RaiseDeviceJoined(
            new ZigbeeDeviceJoined(new IeeeAddress(0x2), 0x1234, [], "ACME", "Bulb")
        );

        var activity = await _client.GetFromJsonAsync<ZigbeeActivityEntryModel[]>("/api/zigbee/activity", JsonOptions);

        Assert.NotNull(activity);
        Assert.Contains(activity, e => e.EventType == "zigbee_device_joined");
    }

    [Fact]
    public async Task WhenDeviceStoredThenDevicesEndpointReturnsIt()
    {
        await _factory
            .GetKnownDeviceStore()
            .UpsertAsync(
                new RegisterKnownDevice(
                    "0x00124b0001ce4b11",
                    0x4455,
                    [new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0006], [])],
                    IsSleepy: true
                )
            );

        var devices = await _client.GetFromJsonAsync<ZigbeeKnownDeviceModel[]>("/api/zigbee/devices", JsonOptions);

        Assert.NotNull(devices);
        var device = Assert.Single(devices);
        Assert.Equal("0x00124b0001ce4b11", device.IeeeAddress);
        Assert.Equal((ushort)0x4455, device.NetworkAddress);
        Assert.True(device.IsSleepy);
        var endpoint = Assert.Single(device.Endpoints);
        Assert.Equal((byte)1, endpoint.EndpointId);
        Assert.Equal([(ushort)0x0006], endpoint.InClusters);
    }
}
