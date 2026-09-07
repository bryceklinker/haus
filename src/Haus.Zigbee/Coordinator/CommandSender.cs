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
    private readonly ByteSequenceCounter _transactionSequenceNumber = new();
    private readonly ByteSequenceCounter _requestId = new();

    public CommandSender(ApsSender sender)
        : this(sender, new DeviceCommandQueue(), new CommandRetryHandler(new CommandRetryOptions())) { }

    public CommandSender(ApsSender sender, DeviceCommandQueue queue, CommandRetryHandler retryHandler)
    {
        _sender = sender;
        _queue = queue;
        _retryHandler = retryHandler;
    }

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        return _queue.EnqueueAsync(
            request.Destination,
            ct => _retryHandler.ExecuteWithRetryAsync(_ => SendOnceAsync(request, ct), ct),
            token
        );
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
