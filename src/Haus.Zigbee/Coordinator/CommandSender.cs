using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Coordinator;

// Builds a ZCL command frame, wraps it in an APS-DATA.request, and sends it. The ZCL
// transaction sequence number and the APS request id are distinct concerns, so each layer
// owns its own counter here.
public class CommandSender
{
    private readonly ApsSender _sender;
    private byte _transactionSequenceNumber;
    private byte _requestId;

    public CommandSender(ApsSender sender)
    {
        _sender = sender;
    }

    public Task<ApsDataConfirm> SendCommandAsync(ZigbeeCommandRequest request, CancellationToken token)
    {
        var asdu = ZclCommandBuilder.Build(
            new ZclCommand(
                _transactionSequenceNumber++,
                request.CommandId,
                request.Payload,
                request.DisableDefaultResponse
            )
        );
        var apsRequest = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: _requestId++,
            Destination: request.Destination,
            ProfileId: request.ProfileId,
            ClusterId: request.ClusterId,
            SourceEndpoint: request.SourceEndpoint,
            AsduPayload: asdu,
            TxOptions: 0x00,
            Radius: 0x00
        );
        return _sender.SendAsync(apsRequest, token);
    }
}
