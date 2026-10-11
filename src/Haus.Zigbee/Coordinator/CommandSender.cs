using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Coordinator;

// The ZCL transaction sequence number and the APS request id are distinct concerns, so each
// layer owns its own counter here.
public class CommandSender
{
    private const byte NoApsAckRequested = 0x00;

    // deCONZ APS-DATA.request TxOptions bit 2: request an APS-layer acknowledgment (a real
    // delivery receipt from the destination) rather than just the usual MAC-layer confirm.
    private const byte ApsAckRequested = 0x04;
    private const byte UnlimitedRadius = 0x00;

    private readonly ApsSender _sender;
    private readonly DeviceCommandQueue _queue;
    private readonly CommandRetryHandler _retryHandler;
    private readonly KnownDeviceTable _knownDeviceTable;
    private readonly SleepyCommandHold _sleepyHold;
    private readonly ByteSequenceCounter _transactionSequenceNumber = new();
    private readonly ByteSequenceCounter _requestId = new();

    public CommandSender(ApsSender sender)
        : this(
            sender,
            new DeviceCommandQueue(),
            new CommandRetryHandler(new CommandRetryOptions()),
            new KnownDeviceTable(),
            new SleepyCommandHold()
        ) { }

    public CommandSender(ApsSender sender, DeviceCommandQueue queue, CommandRetryHandler retryHandler)
        : this(sender, queue, retryHandler, new KnownDeviceTable(), new SleepyCommandHold()) { }

    public CommandSender(
        ApsSender sender,
        DeviceCommandQueue queue,
        CommandRetryHandler retryHandler,
        KnownDeviceTable knownDeviceTable,
        SleepyCommandHold sleepyHold
    )
    {
        _sender = sender;
        _queue = queue;
        _retryHandler = retryHandler;
        _knownDeviceTable = knownDeviceTable;
        _sleepyHold = sleepyHold;
    }

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        // Unifies ZigbeeOutboundRelay's former "retry once, escalating to APS-ACK" behavior into
        // this single path: any retried attempt (not the first) escalates to request an APS-ACK,
        // regardless of what the original request asked for, so every command type gets this
        // protection from the one retry implementation instead of a second, duplicate pipeline.
        Task<ApsDataConfirm> SendWithRetryAsync(CancellationToken ct) =>
            _retryHandler.ExecuteWithRetryAsync(
                attempt => SendOnceAsync(attempt > 0 ? request with { RequestApsAck = true } : request, ct),
                ct
            );

        if (TryResolveSleepyDeviceKey(request.Destination, out var key))
            return _sleepyHold.HoldAsync(key, SendWithRetryAsync, token);

        return _queue.EnqueueAsync(request.Destination, SendWithRetryAsync, token);
    }

    // Mains-powered devices (IsSleepy false, or simply unknown to KnownDeviceTable) are unaffected
    // and keep flowing through the existing #77 queue/retry path unchanged -- only a device
    // KnownDeviceTable positively marks sleepy is held here instead of enqueued immediately.
    private bool TryResolveSleepyDeviceKey(ApsDestination destination, out DeviceKey key)
    {
        key = default;
        var device = destination.Mode switch
        {
            DeconzAddressMode.Ieee => _knownDeviceTable.TryGet(destination.IeeeAddress, out var ieeeDevice)
                ? ieeeDevice
                : null,
            DeconzAddressMode.Nwk => _knownDeviceTable.TryGetByNetworkAddress(
                destination.ShortAddress,
                out var nwkDevice
            )
                ? nwkDevice
                : null,
            _ => null,
        };
        if (device is not { IsSleepy: true })
            return false;

        key = DeviceKey.FromDestination(destination)!.Value;
        return true;
    }

    private Task<ApsDataConfirm> SendOnceAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        var asdu = ZclCommandFactory.Encode(
            new ZclCommand(
                _transactionSequenceNumber.Next(),
                request.CommandId,
                request.Payload,
                request.DisableDefaultResponse
            )
        );
        var apsRequest = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: _requestId.Next(),
            Destination: request.Destination,
            ProfileId: request.ProfileId,
            ClusterId: request.ClusterId,
            SourceEndpoint: request.SourceEndpoint,
            AsduPayload: asdu,
            TxOptions: request.RequestApsAck ? ApsAckRequested : NoApsAckRequested,
            Radius: UnlimitedRadius
        );
        return _sender.SendAsync(apsRequest, token);
    }
}
