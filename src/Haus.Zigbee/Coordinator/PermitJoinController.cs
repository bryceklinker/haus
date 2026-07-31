using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Coordinator;

// Opens or closes the deCONZ coordinator's join window by writing the STK_PERMIT_JOIN parameter:
// 0xFF permits joining indefinitely, 0x00 disables it. Nothing else about the network changes.
public class PermitJoinController
{
    private const byte PermitJoinParameterId = 0x21;
    private const byte PermitIndefinitely = 0xFF;
    private const byte PermitDisabled = 0x00;

    private readonly DeconzChannel _channel;
    private readonly ByteSequenceCounter _nextSequenceNumber = new();

    public PermitJoinController(DeconzChannel channel)
    {
        _channel = channel;
    }

    public async Task SetPermitJoinAsync(bool enabled, CancellationToken token)
    {
        var duration = enabled ? PermitIndefinitely : PermitDisabled;
        var request = new WriteParameterRequest(_nextSequenceNumber.Next(), PermitJoinParameterId, new[] { duration });
        await _channel.SendAndReceiveAsync(WriteParameterFrame.Encode(request), token);
    }
}
