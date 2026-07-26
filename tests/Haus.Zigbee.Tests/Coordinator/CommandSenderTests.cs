using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
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

        _ = _commandSender.SendCommandAsync(request, Cancelled());

        var expectedAsdu = ZclCommandBuilder.Build(
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

    private static CancellationToken Cancelled()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        return source.Token;
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
