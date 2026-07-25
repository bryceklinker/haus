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
        await _channel.SendAndReceiveAsync(pollRequest, token);
    }
}
