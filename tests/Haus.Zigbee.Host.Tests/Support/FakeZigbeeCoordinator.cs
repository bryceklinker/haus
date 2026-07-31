using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Host.Tests.Support;

public class FakeZigbeeCoordinator : IZigbeeCoordinator
{
    public List<bool> PermitJoinCalls { get; } = [];
    public List<ZigbeeCommandRequest> SentCommands { get; } = [];
    public IReadOnlyList<ZigbeeDevice> DevicesToReturn { get; set; } = [];
    public ApsDataConfirm ConfirmToReturn { get; set; } =
        new(0, 0, 0, DeconzAddressMode.Nwk, 0, null, 0, 0, ConfirmStatus: 0);
    public ZigbeeDeviceInfo? DeviceInfoToReturn { get; set; }

    public bool IsConnected { get; set; }
    public NetworkConfig? NetworkConfig { get; set; }
    public Exception? ConnectShouldThrow { get; set; }
    public Exception? SetPermitJoinShouldThrow { get; set; }
    public int ConnectAttempts { get; private set; }

    public event System.EventHandler<ZigbeeAttributeReport>? AttributeReported;
    public event System.EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

    public Task ConnectAsync(CancellationToken token)
    {
        ConnectAttempts++;
        if (ConnectShouldThrow != null)
            throw ConnectShouldThrow;

        IsConnected = true;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ZigbeeDevice>> GetDevicesAsync(CancellationToken token)
    {
        return Task.FromResult(DevicesToReturn);
    }

    public Task<ZigbeeDeviceInfo?> ReadDeviceInfoAsync(IeeeAddress ieeeAddress, CancellationToken token)
    {
        return Task.FromResult(DeviceInfoToReturn);
    }

    public Task SetPermitJoinAsync(bool enabled, CancellationToken token)
    {
        if (SetPermitJoinShouldThrow != null)
            throw SetPermitJoinShouldThrow;

        PermitJoinCalls.Add(enabled);
        return Task.CompletedTask;
    }

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        SentCommands.Add(request);
        return Task.FromResult(ConfirmToReturn);
    }

    public void RaiseDeviceJoined(ZigbeeDeviceJoined joined)
    {
        DeviceJoined?.Invoke(this, joined);
    }

    public void RaiseAttributeReported(ZigbeeAttributeReport report)
    {
        AttributeReported?.Invoke(this, report);
    }

    public void Dispose() { }
}
