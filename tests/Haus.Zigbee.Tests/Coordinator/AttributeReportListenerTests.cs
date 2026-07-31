using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
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

    [Fact]
    public async Task WhenAReadAttributesResponseIndicationArrivesThenEachSucceededAttributeIsRaised()
    {
        var zclFrame = new byte[]
        {
            GlobalFrameControl,
            0x01,
            ReadAttributesResponseCommand,
            0x00,
            0x00,
            SuccessStatus,
            (byte)ZclDataType.Int16,
            0x9c,
            0xff,
        };

        var reports = await ListenToIndication(sourceNwk: 0x00aa, sourceEndpoint: 0x01, clusterId: 0x0402, zclFrame);

        var report = Assert.Single(reports);
        Assert.Equal(0x00aa, report.SourceNwkAddress);
        Assert.Equal(0x01, report.Endpoint);
        Assert.Equal(0x0402, report.ClusterId);
        Assert.Equal(0x0000, report.AttributeId);
        Assert.Equal(ZclDataType.Int16, report.Value.DataType);
        Assert.Equal(-100, report.Value.AsSigned());
    }

    [Fact]
    public async Task WhenAReportAttributesIndicationCarriesMultipleAttributesThenOneEventIsRaisedPerAttribute()
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
            0x01,
            0x00,
            (byte)ZclDataType.Bool,
            0x01,
        };

        var reports = await ListenToIndication(sourceNwk: 0x1234, sourceEndpoint: 0x05, clusterId: 0x0400, zclFrame);

        Assert.Equal(2, reports.Count);
        Assert.Equal(0x0000, reports[0].AttributeId);
        Assert.Equal(0x1234ul, reports[0].Value.AsUnsigned());
        Assert.Equal(0x0001, reports[1].AttributeId);
        Assert.True(reports[1].Value.AsBool());
    }

    [Fact]
    public async Task WhenAClusterSpecificCommandReusesTheReportCommandIdThenNothingIsRaised()
    {
        var zclFrame = new byte[]
        {
            (byte)ZclFrameType.ClusterSpecific,
            0x01,
            ReportAttributesCommand,
            0x00,
            0x00,
            (byte)ZclDataType.Uint16,
            0x34,
            0x12,
        };

        var reports = await ListenToIndication(sourceNwk: 0x1234, sourceEndpoint: 0x05, clusterId: 0x0400, zclFrame);

        Assert.Empty(reports);
    }

    [Fact]
    public async Task WhenAGlobalCommandIsNotAnAttributeReportOrReadResponseThenNothingIsRaised()
    {
        const byte configureReportingResponseCommand = 0x07;
        var zclFrame = new byte[] { GlobalFrameControl, 0x01, configureReportingResponseCommand, 0x00 };

        var reports = await ListenToIndication(sourceNwk: 0x1234, sourceEndpoint: 0x05, clusterId: 0x0400, zclFrame);

        Assert.Empty(reports);
    }

    [Fact]
    public async Task WhenAZdpProfileIndicationArrivesThenItIsIgnoredWithoutThrowing()
    {
        const ushort zdpProfile = 0x0000;
        const ushort activeEndpointsResponseCluster = 0x8005;

        // A real ZDP response's first byte is just its transaction sequence number, not a ZCL
        // frame-control byte -- but nothing stops that byte's value from having the
        // manufacturer-specific bit set (bit 2), which would make ZclFrameHeaderCodec read three
        // more bytes than this single-byte payload has, throwing IndexOutOfRangeException if this
        // indication is ever handed to it. AttributeReportListener must recognize ZDP-profile
        // indications and skip them before that ever happens -- both because a thrown exception
        // here would silently stop every other IndicationReceived subscriber (like DeviceInterview)
        // from running for this event, and because ZDP payloads are simply never ZCL frames.
        var manufacturerSpecificBitSet = new byte[] { 0x04 };

        var reports = await ListenToIndication(
            sourceNwk: 0x1234,
            sourceEndpoint: 0x00,
            clusterId: activeEndpointsResponseCluster,
            manufacturerSpecificBitSet,
            profileId: zdpProfile
        );

        Assert.Empty(reports);
    }

    [Fact]
    public async Task WhenANonZdpIndicationsAsduIsTooShortToBeAZclFrameThenItIsIgnoredWithoutThrowing()
    {
        // A genuinely malformed/truncated report from a flaky real device, not a ZDP profile --
        // that guard doesn't cover this. ZclFrameHeaderCodec.Decode must return null for a payload
        // too short to hold a frame-control byte plus sequence number and command id, and this
        // listener must treat that the same way as any other indication it doesn't understand.
        var reports = await ListenToIndication(sourceNwk: 0x1234, sourceEndpoint: 0x05, clusterId: 0x0400, [0x00]);

        Assert.Empty(reports);
    }

    [Fact]
    public async Task WhenDisposedThenAnArrivingIndicationNoLongerRaisesAnAttributeReport()
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

        var transport = new ScriptedSerialTransport();
        var loop = new ApsPollLoop(new DeconzChannel(transport));
        var listener = new AttributeReportListener(loop);
        var reports = new List<ZigbeeAttributeReport>();
        listener.AttributeReported += (_, report) => reports.Add(report);
        transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: IndicationAvailable))
        );
        transport.QueueResponse(
            DeconzFrames.Framed(
                IndicationResponse(
                    sequenceNumber: 1,
                    sourceNwk: 0x1234,
                    sourceEndpoint: 0x05,
                    clusterId: 0x0400,
                    zclFrame
                )
            )
        );

        listener.Dispose();
        await loop.PollOnceAsync(CancellationToken.None);

        Assert.Empty(reports);
    }

    private static async Task<IReadOnlyList<ZigbeeAttributeReport>> ListenToIndication(
        ushort sourceNwk,
        byte sourceEndpoint,
        ushort clusterId,
        byte[] zclFrame,
        ushort profileId = 0x0104
    )
    {
        var transport = new ScriptedSerialTransport();
        var loop = new ApsPollLoop(new DeconzChannel(transport));
        var listener = new AttributeReportListener(loop);
        var reports = new List<ZigbeeAttributeReport>();
        listener.AttributeReported += (_, report) => reports.Add(report);

        transport.QueueResponse(
            DeconzFrames.Framed(DeviceStateResponse(sequenceNumber: 0, deviceState: IndicationAvailable))
        );
        transport.QueueResponse(
            DeconzFrames.Framed(
                IndicationResponse(sequenceNumber: 1, sourceNwk, sourceEndpoint, clusterId, zclFrame, profileId)
            )
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
        byte[] zclFrame,
        ushort profileId = 0x0104
    )
    {
        var header = new byte[] { 0x17, sequenceNumber, SuccessStatus, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var destination = new byte[] { NwkAddressMode, 0x00, 0x00, 0x01 };
        var source = new byte[] { NwkAddressMode, (byte)(sourceNwk & 0xff), (byte)(sourceNwk >> 8), sourceEndpoint };
        var profileAndCluster = new byte[]
        {
            (byte)(profileId & 0xff),
            (byte)(profileId >> 8),
            (byte)(clusterId & 0xff),
            (byte)(clusterId >> 8),
        };
        var asdu = Concat(new byte[] { (byte)(zclFrame.Length & 0xff), (byte)(zclFrame.Length >> 8) }, zclFrame);
        var reservedAndLinkQuality = new byte[] { 0x00, 0x00, 0xFF };
        return Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality);
    }

    private static byte[] Concat(params byte[][] segments)
    {
        return segments.SelectMany(segment => segment).ToArray();
    }
}
