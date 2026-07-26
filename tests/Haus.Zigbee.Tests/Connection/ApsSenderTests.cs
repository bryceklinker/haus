using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Xunit;

namespace Haus.Zigbee.Tests.Connection;

public class ApsSenderTests
{
    private const byte SuccessStatus = 0x00;
    private const byte NwkAddressMode = 0x02;
    private const byte ConfirmAvailable = 0x04;

    private readonly ScriptedSerialTransport _senderTransport = new();
    private readonly ScriptedSerialTransport _pollTransport = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly ApsSender _sender;

    public ApsSenderTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_pollTransport));
        _sender = new ApsSender(_pollLoop, new DeconzChannel(_senderTransport));
    }

    [Fact]
    public async Task WhenSendingThenItReturnsTheConfirmThatMatchesTheRequestId()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x42)));

        var sendTask = _sender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        await _pollLoop.PollOnceAsync(CancellationToken.None);
        var confirm = await sendTask;

        Assert.Equal(0x42, confirm.RequestId);
    }

    [Fact]
    public async Task WhenAConfirmForAnotherRequestArrivesThenTheSendKeepsWaitingForItsOwn()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x99)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 2, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 3, requestId: 0x42)));

        var sendTask = _sender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        await _pollLoop.PollOnceAsync(CancellationToken.None);
        Assert.False(sendTask.IsCompleted);

        await _pollLoop.PollOnceAsync(CancellationToken.None);
        Assert.Equal(0x42, (await sendTask).RequestId);
    }

    [Fact]
    public async Task WhenDisposedThenAConfirmNoLongerCompletesAPendingSend()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x42)));

        var sendTask = _sender.SendAsync(Request(requestId: 0x42), CancellationToken.None);
        _sender.Dispose();
        await _pollLoop.PollOnceAsync(CancellationToken.None);

        var settled = await Task.WhenAny(sendTask, Task.Delay(200));
        Assert.NotEqual(sendTask, settled);
    }

    [Fact]
    public void WhenSendingThenTheCommandFrameUsesTheSendersOwnSequenceNumber()
    {
        var abandoned = Cancelled();

        _ = _sender.SendAsync(Request(requestId: 0x42, sequenceNumber: 99), abandoned);

        var expected = Framed(ApsDataRequestFrameCodec.Encode(Request(requestId: 0x42, sequenceNumber: 0)));
        Assert.Equal(expected, _senderTransport.WrittenBytes);
    }

    private static CancellationToken Cancelled()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        return source.Token;
    }

    private static ApsDataRequestFrame Request(byte requestId, byte sequenceNumber = 0)
    {
        return new ApsDataRequestFrame(
            SequenceNumber: sequenceNumber,
            RequestId: requestId,
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x01,
            AsduPayload: new byte[] { 0x01 },
            TxOptions: 0x00,
            Radius: 0x00
        );
    }

    private static byte[] DeconzAck(byte sequenceNumber)
    {
        return new byte[] { 0x12, sequenceNumber, SuccessStatus, 0x00, 0x00 };
    }

    private static byte[] DeviceStateResponse(byte sequenceNumber, byte deviceState)
    {
        return new byte[] { 0x07, sequenceNumber, 0x00, 0x00, 0x00, deviceState };
    }

    private static byte[] ConfirmResponse(byte sequenceNumber, byte requestId)
    {
        var header = new byte[] { 0x04, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var requestAndAddress = new byte[] { requestId, NwkAddressMode, 0x34, 0x12 };
        var endpointsAndStatus = new byte[] { 0x01, 0x01, 0x00 };
        return header.Concat(requestAndAddress).Concat(endpointsAndStatus).ToArray();
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzChecksum.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
