using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial;
using Haus.Zigbee.Tests.Connection;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class DeviceInterviewTests
{
    private const byte IndicationAvailable = 0x08;
    private const ushort ZdpProfile = 0x0000;
    private const ushort DeviceAnnounceCluster = 0x0013;
    private const ushort ActiveEndpointsResponseCluster = 0x8005;

    private readonly ScriptedSerialTransport _senderTransport = new();
    private readonly ScriptedSerialTransport _pollTransport = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly KnownDeviceTable _knownDeviceTable = new();
    private readonly DeviceInterview _interview;
    private readonly List<ZigbeeDeviceJoined> _joined = new();

    public DeviceInterviewTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_pollTransport));
        var sender = new ApsSender(_pollLoop, new DeconzChannel(_senderTransport));
        _interview = new DeviceInterview(_pollLoop, sender, _knownDeviceTable);
        _interview.DeviceJoined += (_, joined) => _joined.Add(joined);
    }

    [Fact]
    public async Task WhenADeviceWithNoEndpointsAnnouncesThenItJoinsWithNoEndpointsAndEmptyBasicInfo()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _senderTransport.QueueResponse(Framed(DeconzAck(sequenceNumber: 0)));
        QueueIndication(pollSequenceNumber: 0, Announce(device));
        QueueIndication(pollSequenceNumber: 2, ActiveEndpointsResponse(device, endpointIds: new byte[0]));

        await _pollLoop.PollOnceAsync(CancellationToken.None);
        await _pollLoop.PollOnceAsync(CancellationToken.None);

        var joined = Assert.Single(_joined);
        Assert.Equal(new IeeeAddress(device.Ieee), joined.IeeeAddress);
        Assert.Equal(device.Nwk, joined.NetworkAddress);
        Assert.Empty(joined.Endpoints);
        Assert.Equal(string.Empty, joined.ManufacturerName);
        Assert.Equal(string.Empty, joined.ModelIdentifier);
    }

    private void QueueIndication(byte pollSequenceNumber, IndicationBody body)
    {
        _pollTransport.QueueResponse(Framed(DeviceStateResponse(pollSequenceNumber, deviceState: IndicationAvailable)));
        _pollTransport.QueueResponse(Framed(IndicationFrame((byte)(pollSequenceNumber + 1), body)));
    }

    private static IndicationBody Announce(DeviceScript device)
    {
        var asdu = new List<byte> { 0x00 };
        AddUInt16(asdu, device.Nwk);
        AddUInt64(asdu, device.Ieee);
        asdu.Add(0x80);
        return new IndicationBody(device.Nwk, SourceEndpoint: 0x00, ZdpProfile, DeviceAnnounceCluster, asdu.ToArray());
    }

    private static IndicationBody ActiveEndpointsResponse(DeviceScript device, byte[] endpointIds)
    {
        var asdu = new List<byte> { 0x00, 0x00 };
        AddUInt16(asdu, device.Nwk);
        asdu.Add((byte)endpointIds.Length);
        asdu.AddRange(endpointIds);
        return new IndicationBody(
            device.Nwk,
            SourceEndpoint: 0x00,
            ZdpProfile,
            ActiveEndpointsResponseCluster,
            asdu.ToArray()
        );
    }

    private static byte[] DeviceStateResponse(byte sequenceNumber, byte deviceState)
    {
        return new byte[] { 0x07, sequenceNumber, 0x00, 0x00, 0x00, deviceState };
    }

    private static byte[] IndicationFrame(byte sequenceNumber, IndicationBody body)
    {
        var header = new byte[] { 0x17, sequenceNumber, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var destination = new byte[] { 0x02, 0x00, 0x00, 0x01 };
        var source = new byte[]
        {
            0x02,
            (byte)(body.SourceNwk & 0xff),
            (byte)(body.SourceNwk >> 8),
            body.SourceEndpoint,
        };
        var profileAndCluster = new byte[]
        {
            (byte)(body.ProfileId & 0xff),
            (byte)(body.ProfileId >> 8),
            (byte)(body.ClusterId & 0xff),
            (byte)(body.ClusterId >> 8),
        };
        var asdu = Concat(new byte[] { (byte)(body.Asdu.Length & 0xff), (byte)(body.Asdu.Length >> 8) }, body.Asdu);
        var reservedAndLinkQuality = new byte[] { 0x00, 0x00, 0xff };
        return Concat(header, destination, source, profileAndCluster, asdu, reservedAndLinkQuality);
    }

    private static byte[] DeconzAck(byte sequenceNumber)
    {
        return new byte[] { 0x12, sequenceNumber, 0x00, 0x00, 0x00 };
    }

    private static void AddUInt16(List<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xff));
        bytes.Add((byte)(value >> 8));
    }

    private static void AddUInt64(List<byte> bytes, ulong value)
    {
        for (var shift = 0; shift < 64; shift += 8)
            bytes.Add((byte)((value >> shift) & 0xff));
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

    private sealed record DeviceScript(ushort Nwk, ulong Ieee);

    private sealed record IndicationBody(
        ushort SourceNwk,
        byte SourceEndpoint,
        ushort ProfileId,
        ushort ClusterId,
        byte[] Asdu
    );
}
