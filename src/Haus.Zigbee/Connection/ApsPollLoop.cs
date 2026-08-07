using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Serial.Frames;

namespace Haus.Zigbee.Connection;

public class ApsPollLoop(DeconzChannel channel)
{
    private const byte ReadIndicationCommandId = 0x17;
    private const byte ReadConfirmCommandId = 0x04;
    private const byte RequestStatus = 0x00;
    private const byte ReadIndicationFrameLength = 0x08;
    private const byte ReadConfirmFrameLength = 0x07;

    private readonly DeconzChannel _channel = channel;
    private byte _sequenceNumber;

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
        // The three bytes after the frame length are a fixed template for this request in
        // the deCONZ protocol; the reference implementation doesn't decompose them further.
        return
        [
            ReadIndicationCommandId,
            sequenceNumber,
            RequestStatus,
            ReadIndicationFrameLength,
            0x00,
            0x01,
            0x00,
            0x01,
        ];
    }

    private static byte[] ReadConfirmRequest(byte sequenceNumber)
    {
        return [ReadConfirmCommandId, sequenceNumber, RequestStatus, ReadConfirmFrameLength, 0x00, 0x00, 0x00];
    }
}
