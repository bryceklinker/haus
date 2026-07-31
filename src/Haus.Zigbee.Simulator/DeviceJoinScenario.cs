using System.Collections.Generic;
using System.Text;
using Haus.Zigbee.Models;
using Haus.Zigbee.Zcl;

namespace Haus.Zigbee.Simulator;

// Composes the exact response sequence a real device-interview expects (active-endpoints ->
// simple-descriptor -> Basic-cluster read) and schedules it on the responder, keyed per-device
// (network address + that device's own request step) so concurrent simulated joins for different
// devices never collide, however their individual interview steps happen to interleave.
public static class DeviceJoinScenario
{
    private const ushort ZdpProfile = 0x0000;
    private const ushort DeviceAnnounceCluster = 0x0013;
    private const ushort ActiveEndpointsResponseCluster = 0x8005;
    private const ushort SimpleDescriptorResponseCluster = 0x8004;
    private const ushort BasicCluster = 0x0000;
    private const ushort HomeAutomationProfile = 0x0104;
    private const ushort OnOffCluster = 0x0006;
    private const byte SimulatedEndpointId = 0x01;
    private const byte MacCapability = 0x80;
    private const byte SuccessStatus = 0x00;
    private const byte GlobalFrameControl = 0x00;
    private const byte ReadAttributesResponseCommand = 0x01;
    private const byte CharacterStringType = 0x42;
    private const ushort ManufacturerNameAttribute = 0x0004;
    private const ushort ModelIdentifierAttribute = 0x0005;

    // The announce indication isn't a response to any request, so it has no sequence number to
    // echo -- unlike the other three indications below, which must echo back whatever sequence
    // number the coordinator's actual request used (DeviceInterview correlates responses on it).
    private const byte AnnounceTransactionSequenceNumber = 0x00;

    // ZDP requests/responses always address endpoint 0 (the ZDO), never a real application endpoint.
    private const byte ZdpEndpoint = 0x00;

    private const byte SimulatedEndpointCount = 1;

    // Arbitrary ZCL device-id/version for the one simulated endpoint's simple descriptor -- nothing
    // in the interview flow inspects these beyond passing them through.
    private const ushort SimulatedDeviceId = 0x0100;
    private const byte SimulatedDeviceVersion = 0x00;

    public const int ApsRequestsPerJoin = 3;

    public static void SimulateJoin(
        DeconzResponder responder,
        IeeeAddress ieeeAddress,
        ushort networkAddress,
        string vendor,
        string model
    )
    {
        responder.EnqueueIndication(AnnounceIndication(networkAddress, ieeeAddress));
        responder.ReleaseAfterApsRequest(
            networkAddress,
            step: 0,
            request => ActiveEndpointsIndication(networkAddress, ExtractZdpSequenceNumber(request))
        );
        responder.ReleaseAfterApsRequest(
            networkAddress,
            step: 1,
            request => SimpleDescriptorIndication(networkAddress, ExtractZdpSequenceNumber(request))
        );
        responder.ReleaseAfterApsRequest(
            networkAddress,
            step: 2,
            request => BasicReadIndication(networkAddress, vendor, model, ExtractZclSequenceNumber(request))
        );
    }

    // A real device echoes the request's own transaction sequence number back in its response;
    // DeviceInterview correlates on that number, so these read it out of the actual request rather
    // than guessing what the coordinator's counter is at (which depends on how many other devices
    // it has already interviewed this session).
    private static byte ExtractZdpSequenceNumber(byte[] apsDataRequest)
    {
        return DeconzResponder.ExtractAsduPayload(apsDataRequest)[0];
    }

    private static byte ExtractZclSequenceNumber(byte[] apsDataRequest)
    {
        var asdu = DeconzResponder.ExtractAsduPayload(apsDataRequest);
        return ZclFrameHeaderCodec.Decode(asdu).Header.TransactionSequenceNumber;
    }

    private static IndicationBody AnnounceIndication(ushort networkAddress, IeeeAddress ieeeAddress)
    {
        var asdu = new List<byte> { AnnounceTransactionSequenceNumber };
        AddUInt16(asdu, networkAddress);
        AddUInt64(asdu, ieeeAddress.Value);
        asdu.Add(MacCapability);
        return new IndicationBody(networkAddress, ZdpEndpoint, ZdpProfile, DeviceAnnounceCluster, asdu.ToArray());
    }

    private static IndicationBody ActiveEndpointsIndication(ushort networkAddress, byte sequenceNumber)
    {
        var asdu = new List<byte> { sequenceNumber, SuccessStatus };
        AddUInt16(asdu, networkAddress);
        asdu.Add(SimulatedEndpointCount);
        asdu.Add(SimulatedEndpointId);
        return new IndicationBody(
            networkAddress,
            ZdpEndpoint,
            ZdpProfile,
            ActiveEndpointsResponseCluster,
            asdu.ToArray()
        );
    }

    private static IndicationBody SimpleDescriptorIndication(ushort networkAddress, byte sequenceNumber)
    {
        var descriptor = new List<byte> { SimulatedEndpointId };
        AddUInt16(descriptor, HomeAutomationProfile);
        AddUInt16(descriptor, SimulatedDeviceId);
        descriptor.Add(SimulatedDeviceVersion);
        AddClusterList(descriptor, [BasicCluster, OnOffCluster]);
        AddClusterList(descriptor, []);

        var asdu = new List<byte> { sequenceNumber, SuccessStatus };
        AddUInt16(asdu, networkAddress);
        asdu.Add((byte)descriptor.Count);
        asdu.AddRange(descriptor);
        return new IndicationBody(
            networkAddress,
            SimulatedEndpointId,
            ZdpProfile,
            SimpleDescriptorResponseCluster,
            asdu.ToArray()
        );
    }

    private static IndicationBody BasicReadIndication(
        ushort networkAddress,
        string vendor,
        string model,
        byte sequenceNumber
    )
    {
        var frame = new List<byte> { GlobalFrameControl, sequenceNumber, ReadAttributesResponseCommand };
        AddStringAttribute(frame, ManufacturerNameAttribute, vendor);
        AddStringAttribute(frame, ModelIdentifierAttribute, model);
        return new IndicationBody(
            networkAddress,
            SimulatedEndpointId,
            HomeAutomationProfile,
            BasicCluster,
            frame.ToArray()
        );
    }

    private static void AddStringAttribute(List<byte> frame, ushort attributeId, string value)
    {
        AddUInt16(frame, attributeId);
        frame.Add(SuccessStatus);
        frame.Add(CharacterStringType);
        var bytes = Encoding.ASCII.GetBytes(value);
        frame.Add((byte)bytes.Length);
        frame.AddRange(bytes);
    }

    private static void AddClusterList(List<byte> bytes, ushort[] clusters)
    {
        bytes.Add((byte)clusters.Length);
        foreach (var cluster in clusters)
            AddUInt16(bytes, cluster);
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
}
