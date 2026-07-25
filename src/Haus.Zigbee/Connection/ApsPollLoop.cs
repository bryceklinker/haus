using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

public sealed class ApsPollLoop
{
    private readonly DeconzChannel _channel;
    private byte _sequenceNumber;

    public ApsPollLoop(DeconzChannel channel)
    {
        _channel = channel;
    }

    public async Task PollOnceAsync(CancellationToken token)
    {
        var pollRequest = DeviceStateCodec.EncodePollRequest(_sequenceNumber++);
        var pollResponse = await _channel.SendAndReceiveAsync(pollRequest, token);
        var deviceState = DeviceStateCodec.Decode(pollResponse);

        if (deviceState.ApsDataIndicationAvailable)
            await DrainIndicationAsync(token);
    }

    private async Task DrainIndicationAsync(CancellationToken token)
    {
        var readRequest = ReadIndicationRequest(_sequenceNumber++);
        await _channel.SendAndReceiveAsync(readRequest, token);
    }

    private static byte[] ReadIndicationRequest(byte sequenceNumber)
    {
        return [0x17, sequenceNumber, 0x00, 0x08, 0x00, 0x01, 0x00, 0x01];
    }
}
