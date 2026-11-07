using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Host.Diagnostics;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Diagnostics;

public class ZigbeeDiagnosticsRecorderTests : IAsyncLifetime
{
    private readonly FakeZigbeeCoordinator _coordinator = new();
    private readonly ZigbeeDiagnosticsStore _store = new();
    private readonly ZigbeeDiagnosticsRecorder _recorder;

    public ZigbeeDiagnosticsRecorderTests()
    {
        _recorder = new ZigbeeDiagnosticsRecorder(_coordinator, _store, TimeProvider.System);
    }

    public async Task InitializeAsync()
    {
        await _recorder.StartAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _recorder.StopAsync(CancellationToken.None);
    }

    [Fact]
    public void WhenConnectionStatusChangedThenCurrentStatusReflectsItWithMappedNetworkConfig()
    {
        var networkConfig = new NetworkConfig(new IeeeAddress(0x00124b0001ce4b11), 0x1a62, 15);

        _coordinator.RaiseConnectionStatusChanged(new ZigbeeConnectionStatus(true, networkConfig, "connected"));

        var status = _store.Current.ConnectionStatus;
        Assert.True(status.IsConnected);
        Assert.Equal("connected", status.Reason);
        Assert.NotNull(status.NetworkConfig);
        Assert.Equal(networkConfig.MacAddress.ToString(), status.NetworkConfig.MacAddress);
        Assert.Equal((ushort)0x1a62, status.NetworkConfig.PanId);
        Assert.Equal((byte)15, status.NetworkConfig.Channel);
    }

    [Fact]
    public void WhenConnectionStatusHasNoNetworkConfigThenMappedConfigIsNull()
    {
        _coordinator.RaiseConnectionStatusChanged(new ZigbeeConnectionStatus(false, null, "disconnected"));

        Assert.Null(_store.Current.ConnectionStatus.NetworkConfig);
    }

    [Fact]
    public void WhenCoordinatorEventsRaisedThenEachIsRecordedAsActivity()
    {
        _coordinator.RaiseConnectionStatusChanged(new ZigbeeConnectionStatus(true, null, "connected"));
        _coordinator.RaiseDeviceJoined(new ZigbeeDeviceJoined(new IeeeAddress(0x1), 0x1234, [], "ACME", "Bulb"));
        _coordinator.RaiseCommandSent(new ZigbeeCommandSent(ApsDestination.Nwk(0x1234, 0x01), 0x0006, 0x01));
        _coordinator.RaiseTransportError(new ZigbeeTransportError("aps", "boom", 0x1234, null));

        var eventTypes = _store.Current.RecentActivity.Select(e => e.EventType).ToArray();
        Assert.Equal(
            [
                "zigbee_connection_status_changed",
                "zigbee_device_joined",
                "zigbee_command_sent",
                "zigbee_transport_error",
            ],
            eventTypes
        );
    }

    [Fact]
    public async Task WhenStoppedThenFurtherCoordinatorEventsAreNotRecorded()
    {
        await _recorder.StopAsync(CancellationToken.None);

        _coordinator.RaiseDeviceJoined(new ZigbeeDeviceJoined(new IeeeAddress(0x1), 0x1234, [], "ACME", "Bulb"));

        Assert.Empty(_store.Current.RecentActivity);
    }
}
