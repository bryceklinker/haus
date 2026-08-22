using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee;

// The single entry point the Haus.Zigbee library exposes to the outside world. Everything else in
// the assembly is an internal collaborator this façade composes; a consumer only programs against
// this contract.
public interface IZigbeeCoordinator : IDisposable
{
    bool IsConnected { get; }

    NetworkConfig? NetworkConfig { get; }

    Task ConnectAsync(CancellationToken token);

    Task<IReadOnlyList<ZigbeeDevice>> GetDevicesAsync(CancellationToken token);

    // Returns null when the address is not (or no longer) in the known-device table; otherwise
    // re-reads its Basic cluster, the same way a fresh device-interview would.
    Task<ZigbeeDeviceInfo?> ReadDeviceInfoAsync(IeeeAddress ieeeAddress, CancellationToken token);

    Task SetPermitJoinAsync(bool enabled, CancellationToken token);

    Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token);

    event EventHandler<ZigbeeAttributeReport>? AttributeReported;

    event EventHandler<ZigbeeDeviceJoined>? DeviceJoined;

    event EventHandler<ZigbeeConnectionStatus>? ConnectionStatusChanged;

    event EventHandler<ZigbeeCommandSent>? CommandSent;

    event EventHandler<ZigbeeTransportError>? TransportError;
}
