using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Models;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Tests.Connection;
using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class CommandSenderTests
{
    private const byte SuccessStatus = 0x00;
    private const byte NwkAddressMode = 0x02;
    private const byte ConfirmAvailable = 0x04;

    private readonly ScriptedSerialTransport _senderTransport = new();
    private readonly ScriptedSerialTransport _pollTransport = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly CommandSender _commandSender;

    public CommandSenderTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_pollTransport));
        _commandSender = new CommandSender(new ApsSender(_pollLoop, new DeconzChannel(_senderTransport)));
    }

    [Fact]
    public void WhenSendingCommandThenItWritesAnApsRequestWhoseAsduIsTheBuiltZclFrame()
    {
        var request = new ZigbeeCommandRequest(
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            SourceEndpoint: 0x01,
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            CommandId: 0x01,
            Payload: new byte[] { 0xaa, 0xbb },
            DisableDefaultResponse: true
        );

        // Never awaited/completed and no response is ever queued, so its internal wait would
        // otherwise spin forever -- bound it (deliberately not disposed: it must outlive this
        // method to still fire) so the abandoned task actually terminates instead of pinning a
        // thread pool thread for the rest of the process's life. The write itself still happens
        // synchronously before this returns, which is all this test checks.
        var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        _ = _commandSender.SendCommandAsync(request, timeout.Token);

        var expectedAsdu = ZclCommandFactory.Encode(
            new ZclCommand(
                TransactionSequenceNumber: 0,
                CommandId: 0x01,
                Payload: new byte[] { 0xaa, 0xbb },
                DisableDefaultResponse: true
            )
        );
        var expectedFrame = new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: 0,
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            SourceEndpoint: 0x01,
            AsduPayload: expectedAsdu,
            TxOptions: 0x00,
            Radius: 0x00
        );
        Assert.Equal(Framed(ApsDataRequestFrameCodec.Encode(expectedFrame)), _senderTransport.WrittenBytes);
    }

    [Fact]
    public async Task WhenTheCoordinatorConfirmsDeliveryThenThatConfirmIsReturnedToTheCaller()
    {
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: ConfirmAvailable)));
        _pollTransport.QueueResponse(Framed(ConfirmResponse(sequenceNumber: 1, requestId: 0x00, confirmStatus: 0xd0)));

        var sendTask = _commandSender.SendCommandAsync(AnyRequest(), CancellationToken.None);
        await _pollLoop.PollOnceAsync(CancellationToken.None);
        var confirm = await sendTask;

        Assert.Equal(0x00, confirm.RequestId);
        Assert.Equal(0xd0, confirm.ConfirmStatus);
    }

    private static ZigbeeCommandRequest AnyRequest()
    {
        return new ZigbeeCommandRequest(
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            SourceEndpoint: 0x01,
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            CommandId: 0x01,
            Payload: new byte[] { 0x00 },
            DisableDefaultResponse: false
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

    private static byte[] ConfirmResponse(byte sequenceNumber, byte requestId, byte confirmStatus)
    {
        var header = new byte[] { 0x04, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var requestAndAddress = new byte[] { requestId, NwkAddressMode, 0x34, 0x12 };
        var endpointsAndStatus = new byte[] { 0x01, 0x01, confirmStatus };
        return header.Concat(requestAndAddress).Concat(endpointsAndStatus).ToArray();
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzChecksum.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
