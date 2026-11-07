using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Models.Diagnostics;
using Haus.Zigbee.Serial.Frames;
using Microsoft.Extensions.Hosting;

namespace Haus.Zigbee.Host.Diagnostics;

// Records raw Zigbee-network visibility into the in-memory diagnostics store that the read-only
// HTTP API reads from. Deliberately separate from ZigbeeDiagnosticsPublisher (which maps the same
// coordinator events onto MQTT) -- keeping the data-capture here distinct from the MQTT handling.
public class ZigbeeDiagnosticsRecorder(
    IZigbeeCoordinator coordinator,
    IZigbeeDiagnosticsStore store,
    TimeProvider timeProvider
) : IHostedService, IDisposable
{
    private const string ConnectionStatusChangedType = "zigbee_connection_status_changed";
    private const string DeviceJoinedType = "zigbee_device_joined";
    private const string AttributeReportReceivedType = "zigbee_attribute_report_received";
    private const string CommandSentType = "zigbee_command_sent";
    private const string TransportErrorType = "zigbee_transport_error";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        coordinator.ConnectionStatusChanged += OnConnectionStatusChanged;
        coordinator.DeviceJoined += OnDeviceJoined;
        coordinator.AttributeReported += OnAttributeReported;
        coordinator.CommandSent += OnCommandSent;
        coordinator.TransportError += OnTransportError;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Unsubscribe();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }

    private void Unsubscribe()
    {
        coordinator.ConnectionStatusChanged -= OnConnectionStatusChanged;
        coordinator.DeviceJoined -= OnDeviceJoined;
        coordinator.AttributeReported -= OnAttributeReported;
        coordinator.CommandSent -= OnCommandSent;
        coordinator.TransportError -= OnTransportError;
    }

    private void OnConnectionStatusChanged(object? sender, ZigbeeConnectionStatus status)
    {
        var networkConfig = status.NetworkConfig is null
            ? null
            : new ZigbeeNetworkConfigModel(
                status.NetworkConfig.MacAddress.ToString(),
                status.NetworkConfig.PanId,
                status.NetworkConfig.Channel
            );
        var model = new ZigbeeConnectionStatusModel(
            status.IsConnected,
            status.Reason,
            timeProvider.GetUtcNow(),
            networkConfig
        );
        store.Update(s =>
            s.UpdateConnectionStatus(model).RecordActivity(Activity(ConnectionStatusChangedType, status))
        );
    }

    private void OnDeviceJoined(object? sender, ZigbeeDeviceJoined joined)
    {
        RecordActivity(DeviceJoinedType, joined);
    }

    private void OnAttributeReported(object? sender, ZigbeeAttributeReport report)
    {
        RecordActivity(AttributeReportReceivedType, report);
    }

    private void OnCommandSent(object? sender, ZigbeeCommandSent sent)
    {
        RecordActivity(CommandSentType, sent);
    }

    private void OnTransportError(object? sender, ZigbeeTransportError error)
    {
        RecordActivity(TransportErrorType, error);
    }

    private void RecordActivity(string eventType, object payload)
    {
        store.Update(s => s.RecordActivity(Activity(eventType, payload)));
    }

    private ZigbeeActivityEntryModel Activity(string eventType, object payload)
    {
        return new ZigbeeActivityEntryModel(eventType, timeProvider.GetUtcNow(), payload);
    }
}
