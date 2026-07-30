using System.Collections.Generic;
using System.Text;
using Haus.Zigbee;

namespace Haus.Zigbee.Simulator;

// Composes the exact response sequence a real device-interview expects (active-endpoints ->
// simple-descriptor -> Basic-cluster read) and schedules it on the responder, keyed to whatever
// APS-request count is already in progress so back-to-back simulated joins keep their responses
// in the right order relative to each other.
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

    // ZDP and ZCL each have their own transaction-sequence-number namespace; the simulator never
    // varies either, but they're named separately so a reader doesn't mistake them for one field.
    private const byte ZdpTransactionSequenceNumber = 0x00;
    private const byte ZclTransactionSequenceNumber = 0x00;

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
        var baseIndex = responder.ApsRequestCount;
        responder.EnqueueIndication(AnnounceIndication(networkAddress, ieeeAddress));
        responder.ReleaseAfterApsRequest(baseIndex, ActiveEndpointsIndication(networkAddress));
        responder.ReleaseAfterApsRequest(baseIndex + 1, SimpleDescriptorIndication(networkAddress));
        responder.ReleaseAfterApsRequest(baseIndex + 2, BasicReadIndication(networkAddress, vendor, model));
    }

    private static IndicationBody AnnounceIndication(ushort networkAddress, IeeeAddress ieeeAddress)
    {
        var asdu = new List<byte> { ZdpTransactionSequenceNumber };
        AddUInt16(asdu, networkAddress);
        AddUInt64(asdu, ieeeAddress.Value);
        asdu.Add(MacCapability);
        return new IndicationBody(networkAddress, ZdpEndpoint, ZdpProfile, DeviceAnnounceCluster, asdu.ToArray());
    }

    private static IndicationBody ActiveEndpointsIndication(ushort networkAddress)
    {
        var asdu = new List<byte> { ZdpTransactionSequenceNumber, SuccessStatus };
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

    private static IndicationBody SimpleDescriptorIndication(ushort networkAddress)
    {
        var descriptor = new List<byte> { SimulatedEndpointId };
        AddUInt16(descriptor, HomeAutomationProfile);
        AddUInt16(descriptor, SimulatedDeviceId);
        descriptor.Add(SimulatedDeviceVersion);
        AddClusterList(descriptor, [BasicCluster, OnOffCluster]);
        AddClusterList(descriptor, []);

        var asdu = new List<byte> { ZdpTransactionSequenceNumber, SuccessStatus };
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

    private static IndicationBody BasicReadIndication(ushort networkAddress, string vendor, string model)
    {
        var frame = new List<byte> { GlobalFrameControl, ZclTransactionSequenceNumber, ReadAttributesResponseCommand };
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
