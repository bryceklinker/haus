using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Tests.Connection;
using Haus.Zigbee.Zcl;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class AttributeReportListenerTests
{
    private const byte SuccessStatus = 0x00;
    private const byte NwkAddressMode = 0x02;
    private const byte IndicationAvailable = 0x08;

    private const byte GlobalFrameControl = 0x00;
    private const byte ReportAttributesCommand = 0x0a;
    private const byte ReadAttributesResponseCommand = 0x01;

    [Fact]
    public async Task WhenAReportAttributesIndicationArrivesThenItIsRaisedAsAZigbeeAttributeReport()
    {
        var zclFrame = new byte[]
        {
            GlobalFrameControl,
            0x01,
            ReportAttributesCommand,
            0x00,
            0x00,
            (byte)ZclDataType.Uint16,
            0x34,
            0x12,
        };

        var reports = await ListenToIndication(sourceNwk: 0x1234, sourceEndpoint: 0x05, clusterId: 0x0400, zclFrame);

        var report = Assert.Single(reports);
        Assert.Equal(0x1234, report.SourceNwkAddress);
        Assert.Equal(0x05, report.Endpoint);
        Assert.Equal(0x0400, report.ClusterId);
        Assert.Equal(0x0000, report.AttributeId);
        Assert.Equal(ZclDataType.Uint16, report.Value.DataType);
        Assert.Equal(0x1234ul, report.Value.AsUnsigned());
    }

    private static async Task<IReadOnlyList<ZigbeeAttributeReport>> ListenToIndication(
        ushort sourceNwk,
        byte sourceEndpoint,
        ushort clusterId,
        byte[] zclFrame
    )
    {
        var transport = new ScriptedSerialTransport();
        var loop = new ApsPollLoop(new DeconzChannel(transport));
        var listener = new AttributeReportListener(loop);
        var reports = new List<ZigbeeAttributeReport>();
        listener.AttributeReported += (_, report) => reports.Add(report);

        transport.QueueResponse(Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: IndicationAvailable)));
        transport.QueueResponse(
            Framed(IndicationResponse(sequenceNumber: 1, sourceNwk, sourceEndpoint, clusterId, zclFrame))
        );

        await loop.PollOnceAsync(CancellationToken.None);
        return reports;
    }

    private static byte[] DeviceStateResponse(byte sequenceNumber, byte deviceState)
    {
        return new byte[] { 0x07, sequenceNumber, 0x00, 0x00, 0x00, deviceState };
    }

    private static byte[] IndicationResponse(
        byte sequenceNumber,
        ushort sourceNwk,
        byte sourceEndpoint,
        ushort clusterId,
        byte[] zclFrame
    )
    {
        var header = new byte[] { 0x17, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var destination = new byte[] { NwkAddressMode, 0x00, 0x00, 0x01 };
        var source = new byte[] { NwkAddressMode, (byte)(sourceNwk & 0xff), (byte)(sourceNwk >> 8), sourceEndpoint };
        var profileAndCluster = new byte[] { 0x04, 0x01, (byte)(clusterId & 0xff), (byte)(clusterId >> 8) };
        var asdu = Concat(new byte[] { (byte)(zclFrame.Length & 0xff), (byte)(zclFrame.Length >> 8) }, zclFrame);
        var reservedAndLinkQuality = new byte[] { 0x00, 0x00, 0xFF };
        return Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality);
    }

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }

    private static byte[] Framed(byte[] frame)
    {
        var checksum = DeconzCrc.Compute(frame);
        var withChecksum = new List<byte>(frame) { (byte)(checksum & 0xff), (byte)(checksum >> 8) };
        return new SlipEncoder().Encode(withChecksum.ToArray());
    }
}
