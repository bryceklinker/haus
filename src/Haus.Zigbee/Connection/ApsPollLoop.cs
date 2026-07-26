using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

public class ApsPollLoop
{
    private readonly DeconzChannel _channel;
    private byte _sequenceNumber;

    public ApsPollLoop(DeconzChannel channel)
    {
        _channel = channel;
    }

    public event EventHandler<ApsIndicationReceived>? IndicationReceived;
    public event EventHandler<ApsDataConfirm>? ConfirmReceived;

    public async Task PollOnceAsync(CancellationToken token)
    {
        var pollRequest = DeviceStateCodec.EncodePollRequest(_sequenceNumber++);
        var pollResponse = await _channel.SendAndReceiveAsync(pollRequest, token);
        var deviceState = DeviceStateCodec.Decode(pollResponse);

        if (deviceState.ApsDataIndicationAvailable)
            await DrainIndicationAsync(token);

        if (deviceState.ApsDataConfirmAvailable)
            await DrainConfirmAsync(token);
    }

    private async Task DrainIndicationAsync(CancellationToken token)
    {
        var readRequest = ReadIndicationRequest(_sequenceNumber++);
        var response = await _channel.SendAndReceiveAsync(readRequest, token);

        var indication = ApsDataIndicationFrameCodec.Decode(response);
        if (indication is not null)
            IndicationReceived?.Invoke(this, new ApsIndicationReceived(indication));
    }

    private async Task DrainConfirmAsync(CancellationToken token)
    {
        var readRequest = ReadConfirmRequest(_sequenceNumber++);
        var response = await _channel.SendAndReceiveAsync(readRequest, token);

        var decoding = ApsDataConfirmCodec.Decode(response);
        if (decoding.Confirm is { } confirm)
            ConfirmReceived?.Invoke(this, confirm);
    }

    private static byte[] ReadIndicationRequest(byte sequenceNumber)
    {
        return [0x17, sequenceNumber, 0x00, 0x08, 0x00, 0x01, 0x00, 0x01];
    }

    private static byte[] ReadConfirmRequest(byte sequenceNumber)
    {
        return [0x04, sequenceNumber, 0x00, 0x07, 0x00, 0x00, 0x00];
    }
}
