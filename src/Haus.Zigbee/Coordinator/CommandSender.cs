using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Coordinator;

// Builds a ZCL command frame, wraps it in an APS-DATA.request, and sends it. The ZCL
// transaction sequence number and the APS request id are distinct concerns, so each layer
// owns its own counter here.
public class CommandSender(ApsSender sender)
{
    private const byte NoApsAckRequested = 0x00;
    private const byte UnlimitedRadius = 0x00;

    private readonly ApsSender _sender = sender;
    private readonly ByteSequenceCounter _transactionSequenceNumber = new();
    private readonly ByteSequenceCounter _requestId = new();

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
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
            TxOptions: NoApsAckRequested,
            Radius: UnlimitedRadius
        );
        return _sender.SendAsync(apsRequest, token);
    }
}
