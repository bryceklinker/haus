using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Zdp;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class DeviceInterviewTests
{
    private const ushort ZdpProfile = 0x0000;
    private const ushort HomeAutomationProfile = 0x0104;
    private const ushort DeviceAnnounceCluster = 0x0013;
    private const ushort ActiveEndpointsResponseCluster = 0x8005;
    private const ushort SimpleDescriptorResponseCluster = 0x8004;
    private const ushort BasicCluster = 0x0000;
    private const byte CharacterStringType = 0x42;
    private const byte ReadAttributesResponseCommand = 0x01;
    private const byte GlobalServerToClientControl = 0x18;
    private const ushort ManufacturerNameAttribute = 0x0004;
    private const ushort ModelIdentifierAttribute = 0x0005;

    private readonly FakeDeconzDongle _dongle = new();
    private readonly ApsPollLoop _pollLoop;
    private readonly KnownDeviceTable _knownDeviceTable = new();
    private readonly DeviceInterview _interview;
    private readonly TaskCompletionSource<ZigbeeDeviceJoined> _joined = new();

    public DeviceInterviewTests()
    {
        _pollLoop = new ApsPollLoop(new DeconzChannel(_dongle.PollTransport));
        var sender = new ApsSender(_pollLoop, new DeconzChannel(_dongle.SendTransport));
        _interview = new DeviceInterview(_pollLoop, sender, _knownDeviceTable);
        _interview.DeviceJoined += (_, joined) => _joined.TrySetResult(joined);
    }

    [Fact]
    public async Task WhenADeviceWithNoEndpointsAnnouncesThenItJoinsWithNoEndpointsAndEmptyBasicInfo()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsResponse(device, endpointIds: new byte[0]));

        var joined = await RunInterview();

        Assert.Equal(new IeeeAddress(device.Ieee), joined.IeeeAddress);
        Assert.Equal(device.Nwk, joined.NetworkAddress);
        Assert.Empty(joined.Endpoints);
        Assert.Equal(string.Empty, joined.ManufacturerName);
        Assert.Equal(string.Empty, joined.ModelIdentifier);
    }

    [Fact]
    public async Task WhenTheActiveEndpointsRequestIsNakedThenTheDeviceStillJoinsWithNoEndpoints()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsNak(device, ZdoStatus.DeviceNotFound));

        var joined = await RunInterview();

        Assert.Empty(joined.Endpoints);
    }

    [Fact]
    public async Task WhenTheActiveEndpointsResponseAsduIsTruncatedThenTheDeviceStillJoinsWithNoEndpoints()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(
            sendIndex: 0,
            new IndicationBody(
                device.Nwk,
                SourceEndpoint: 0x00,
                ZdpProfile,
                ActiveEndpointsResponseCluster,
                Asdu: new byte[] { 0x00, 0x00 }
            )
        );

        var joined = await RunInterview();

        Assert.Empty(joined.Endpoints);
    }

    [Fact]
    public async Task WhenTheSimpleDescriptorResponseAsduIsTruncatedThenTheEndpointJoinsWithDefaultDescriptorFields()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsResponse(device, endpointIds: new byte[] { 0x01 }));
        _dongle.ReleaseAfterSend(
            sendIndex: 1,
            new IndicationBody(
                device.Nwk,
                SourceEndpoint: 0x01,
                ZdpProfile,
                SimpleDescriptorResponseCluster,
                Asdu: new byte[] { 0x01, 0x00 }
            )
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 2,
            // ProfileId here just needs to be non-zero, since 0x0000 collides with ZdpProfileId and
            // would make DeviceInterview misinterpret this ZCL response's transaction sequence number.
            BasicReadResponse(device, endpoint: 0x01, profileId: HomeAutomationProfile, manufacturer: "", model: "")
        );

        var joined = await RunInterview();

        var endpoint = Assert.Single(joined.Endpoints);
        Assert.Equal(0x01, endpoint.EndpointId);
        Assert.Equal(0, endpoint.ProfileId);
        Assert.Empty(endpoint.InClusters);
        Assert.Empty(endpoint.OutClusters);
    }

    [Fact]
    public async Task WhenTheSimpleDescriptorRequestIsNakedThenTheEndpointJoinsWithDefaultDescriptorFields()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsResponse(device, endpointIds: new byte[] { 0x01 }));
        _dongle.ReleaseAfterSend(
            sendIndex: 1,
            SimpleDescriptorNak(device, endpoint: 0x01, ZdoStatus.InvalidEndpoint, sequenceNumber: 0x01)
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 2,
            // ProfileId here just needs to be non-zero, since 0x0000 collides with ZdpProfileId and
            // would make DeviceInterview misinterpret this ZCL response's transaction sequence number.
            BasicReadResponse(device, endpoint: 0x01, profileId: HomeAutomationProfile, manufacturer: "", model: "")
        );

        var joined = await RunInterview();

        var endpoint = Assert.Single(joined.Endpoints);
        Assert.Equal(0x01, endpoint.EndpointId);
        Assert.Equal(0, endpoint.ProfileId);
        Assert.Empty(endpoint.InClusters);
        Assert.Empty(endpoint.OutClusters);
    }

    [Fact]
    public async Task WhenADeviceAnnouncesThenItIsRegisteredInTheKnownDeviceTable()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsResponse(device, endpointIds: new byte[0]));

        await RunInterview();

        var known = Assert.Single(_knownDeviceTable.GetDevices());
        Assert.Equal(new IeeeAddress(device.Ieee), known.IeeeAddress);
        Assert.Equal(device.Nwk, known.NetworkAddress);
    }

    [Fact]
    public async Task WhenADeviceIsFullyInterviewedThenTheJoinEventCarriesItsEndpointsAndBasicInfo()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsResponse(device, endpointIds: new byte[] { 0x01 }));
        _dongle.ReleaseAfterSend(
            sendIndex: 1,
            SimpleDescriptorResponse(
                device,
                endpoint: 0x01,
                profileId: HomeAutomationProfile,
                deviceId: 0x0100,
                inClusters: new ushort[] { 0x0000, 0x0006 },
                outClusters: new ushort[] { 0x0019 },
                sequenceNumber: 0x01
            )
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 2,
            BasicReadResponse(device, endpoint: 0x01, profileId: HomeAutomationProfile, "IKEA", "LED1836G9")
        );

        var joined = await RunInterview();

        var endpoint = Assert.Single(joined.Endpoints);
        Assert.Equal(0x01, endpoint.EndpointId);
        Assert.Equal(HomeAutomationProfile, endpoint.ProfileId);
        Assert.Equal(0x0100, endpoint.DeviceId);
        Assert.Equal(new ushort[] { 0x0000, 0x0006 }, endpoint.InClusters);
        Assert.Equal(new ushort[] { 0x0019 }, endpoint.OutClusters);
        Assert.Equal("IKEA", joined.ManufacturerName);
        Assert.Equal("LED1836G9", joined.ModelIdentifier);
    }

    [Fact]
    public async Task WhenNoEndpointExposesTheBasicClusterThenTheFirstEndpointIsReadForBasicInfo()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        _dongle.InjectIndication(Announce(device));
        _dongle.ReleaseAfterSend(sendIndex: 0, ActiveEndpointsResponse(device, endpointIds: new byte[] { 0x0a, 0x0b }));
        _dongle.ReleaseAfterSend(
            sendIndex: 1,
            SimpleDescriptorResponse(
                device,
                endpoint: 0x0a,
                profileId: HomeAutomationProfile,
                deviceId: 0x0100,
                inClusters: new ushort[] { 0x0006 },
                outClusters: new ushort[0],
                sequenceNumber: 0x01
            )
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 2,
            SimpleDescriptorResponse(
                device,
                endpoint: 0x0b,
                profileId: HomeAutomationProfile,
                deviceId: 0x0100,
                inClusters: new ushort[] { 0x0008 },
                outClusters: new ushort[0],
                sequenceNumber: 0x02
            )
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 3,
            BasicReadResponse(device, endpoint: 0x0a, profileId: HomeAutomationProfile, "Aqara", "lumi.sensor")
        );

        var joined = await RunInterview();

        Assert.Equal(2, joined.Endpoints.Count);
        Assert.Equal("Aqara", joined.ManufacturerName);
        Assert.Equal("lumi.sensor", joined.ModelIdentifier);
    }

    [Fact]
    public async Task ReadBasicInfoAsync_ReadsManufacturerAndModelForAnAlreadyKnownDevice()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        var endpoint = new ZigbeeEndpoint(
            0x01,
            HomeAutomationProfile,
            0x0100,
            new[] { BasicCluster },
            Array.Empty<ushort>()
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 0,
            BasicReadResponse(device, endpoint: 0x01, profileId: HomeAutomationProfile, "IKEA", "LED1836G9")
        );

        var info = await RunToCompletion(_interview.ReadBasicInfoAsync(device.Nwk, [endpoint], CancellationToken.None));

        Assert.Equal("IKEA", info.ManufacturerName);
        Assert.Equal("LED1836G9", info.ModelIdentifier);
    }

    [Fact]
    public async Task ReadBasicInfoAsync_CalledTwiceForTheSameDeviceBeforeTheFirstResponseArrives_EachCallGetsItsOwnResponse()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        var endpoint = new ZigbeeEndpoint(
            0x01,
            HomeAutomationProfile,
            0x0100,
            new[] { BasicCluster },
            Array.Empty<ushort>()
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 0,
            BasicReadResponse(device, endpoint: 0x01, profileId: HomeAutomationProfile, "VendorA", "ModelA")
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 1,
            BasicReadResponse(
                device,
                endpoint: 0x01,
                profileId: HomeAutomationProfile,
                "VendorB",
                "ModelB",
                sequenceNumber: 0x01
            )
        );

        // Both calls register their pending response before either response has arrived -- this
        // reproduces a device re-announcing (or a backfill read) racing a live interview for the
        // same device without needing real thread concurrency.
        var first = _interview.ReadBasicInfoAsync(device.Nwk, [endpoint], CancellationToken.None);
        var second = _interview.ReadBasicInfoAsync(device.Nwk, [endpoint], CancellationToken.None);

        var infoA = await RunToCompletion(first);
        var infoB = await RunToCompletion(second);

        Assert.Equal("VendorA", infoA.ManufacturerName);
        Assert.Equal("VendorB", infoB.ManufacturerName);
    }

    [Fact]
    public async Task ReadBasicInfoAsync_NoResponseEverArrivesFromTheDevice_TimesOutInsteadOfHangingForever()
    {
        var sender = new ApsSender(_pollLoop, new DeconzChannel(_dongle.SendTransport));
        var timingOutInterview = new DeviceInterview(
            _pollLoop,
            sender,
            _knownDeviceTable,
            responseTimeout: TimeSpan.FromMilliseconds(50)
        );
        var endpoint = new ZigbeeEndpoint(
            0x01,
            HomeAutomationProfile,
            0x0100,
            new[] { BasicCluster },
            Array.Empty<ushort>()
        );

        var readTask = timingOutInterview.ReadBasicInfoAsync(0x1a2b, [endpoint], CancellationToken.None);
        var completed = await Task.WhenAny(readTask, Task.Delay(TimeSpan.FromSeconds(5)));

        Assert.Same(readTask, completed);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => readTask);
    }

    // Uses its own dongle/poll loop rather than the fixture's, so the announce indication is only
    // ever dispatched to this short-timeout interview and not also to the fixture's _interview.
    [Fact]
    public async Task WhenTheActiveEndpointsRequestNeverReceivesAResponseThenTheInterviewAbortsWithoutJoiningTheDevice()
    {
        var dongle = new FakeDeconzDongle();
        var pollLoop = new ApsPollLoop(new DeconzChannel(dongle.PollTransport));
        var sender = new ApsSender(pollLoop, new DeconzChannel(dongle.SendTransport));
        var knownDeviceTable = new KnownDeviceTable();
        using var interview = new DeviceInterview(
            pollLoop,
            sender,
            knownDeviceTable,
            responseTimeout: TimeSpan.FromMilliseconds(50)
        );
        var joined = new TaskCompletionSource<ZigbeeDeviceJoined>();
        interview.DeviceJoined += (_, e) => joined.TrySetResult(e);

        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        dongle.InjectIndication(Announce(device));
        // No ReleaseAfterSend registered for the active-endpoints request -- it never gets a
        // response, simulating a NAK or lost delivery from the coordinator mid-interview.
        await pollLoop.PollOnceAsync(CancellationToken.None);

        await Task.Delay(TimeSpan.FromMilliseconds(500));

        Assert.False(joined.Task.IsCompleted);
        Assert.Empty(knownDeviceTable.GetDevices());
    }

    // Reproduces a device sending a second announce/join while its first full interview (endpoint
    // discovery through basic-info read) is still in progress -- not just a second ReadBasicInfoAsync
    // call, but a second announce dispatched through OnIndicationReceived's Forget(InterviewAsync(...))
    // path while the first interview is mid-flight. Per DeviceInterview's correlation comment, each
    // interview's requests carry their own transaction sequence number, so the two should complete
    // independently rather than the second orphaning or corrupting the first.
    [Fact]
    public async Task WhenADeviceReannouncesWhileItsFirstInterviewIsStillInProgressThenBothInterviewsCompleteIndependently()
    {
        var device = new DeviceScript(Nwk: 0x1a2b, Ieee: 0x00124b0001aabbcc);
        var joinedEvents = new List<ZigbeeDeviceJoined>();
        _interview.DeviceJoined += (_, joined) => joinedEvents.Add(joined);

        _dongle.InjectIndication(Announce(device));
        _dongle.InjectIndication(Announce(device));

        // First interview's ActiveEndpointsRequest is sendIndex 0; it reports one endpoint, so this
        // interview needs two more requests (SimpleDescriptor, then Basic read) before it completes --
        // giving the second interview room to start and finish first.
        _dongle.ReleaseAfterSend(
            sendIndex: 0,
            ActiveEndpointsResponse(device, endpointIds: new byte[] { 0x01 }, sequenceNumber: 0x00)
        );
        // Second interview's ActiveEndpointsRequest is sendIndex 1; scripted with no endpoints so it
        // completes immediately after this single response.
        _dongle.ReleaseAfterSend(
            sendIndex: 1,
            ActiveEndpointsResponse(device, endpointIds: new byte[0], sequenceNumber: 0x01)
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 2,
            SimpleDescriptorResponse(
                device,
                endpoint: 0x01,
                profileId: HomeAutomationProfile,
                deviceId: 0x0100,
                inClusters: new ushort[] { BasicCluster },
                outClusters: new ushort[0],
                sequenceNumber: 0x02
            )
        );
        _dongle.ReleaseAfterSend(
            sendIndex: 3,
            BasicReadResponse(device, endpoint: 0x01, profileId: HomeAutomationProfile, "VendorA", "ModelA")
        );

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (joinedEvents.Count < 2 && !timeout.IsCancellationRequested)
        {
            await _pollLoop.PollOnceAsync(CancellationToken.None);
            if (joinedEvents.Count < 2)
                await Task.Delay(1);
        }

        Assert.Equal(2, joinedEvents.Count);
        var withoutEndpoints = joinedEvents.Single(joined => joined.Endpoints.Count == 0);
        var withEndpoint = joinedEvents.Single(joined => joined.Endpoints.Count == 1);
        Assert.Equal(new IeeeAddress(device.Ieee), withoutEndpoints.IeeeAddress);
        Assert.Equal(string.Empty, withoutEndpoints.ManufacturerName);
        Assert.Equal(new IeeeAddress(device.Ieee), withEndpoint.IeeeAddress);
        Assert.Equal("VendorA", withEndpoint.ManufacturerName);
        Assert.Equal("ModelA", withEndpoint.ModelIdentifier);

        var known = Assert.Single(_knownDeviceTable.GetDevices());
        Assert.Equal(new IeeeAddress(device.Ieee), known.IeeeAddress);
    }

    [Fact]
    public async Task ReadBasicInfoAsync_NoEndpoints_ReturnsEmptyInfoWithoutSendingAnything()
    {
        var info = await _interview.ReadBasicInfoAsync(0x1a2b, [], CancellationToken.None);

        Assert.Equal(string.Empty, info.ManufacturerName);
        Assert.Equal(string.Empty, info.ModelIdentifier);
    }

    private async Task<ZigbeeDeviceJoined> RunInterview()
    {
        return await RunToCompletion(_joined.Task);
    }

    private async Task<T> RunToCompletion<T>(Task<T> task)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!task.IsCompleted && !timeout.IsCancellationRequested)
        {
            await _pollLoop.PollOnceAsync(CancellationToken.None);
            if (!task.IsCompleted)
                await Task.Delay(1);
        }

        Assert.True(task.IsCompleted, "did not complete within the timeout");
        return await task;
    }

    private static IndicationBody Announce(DeviceScript device)
    {
        var asdu = new List<byte> { 0x00 };
        AddUInt16(asdu, device.Nwk);
        AddUInt64(asdu, device.Ieee);
        asdu.Add(0x80);
        return new IndicationBody(device.Nwk, SourceEndpoint: 0x00, ZdpProfile, DeviceAnnounceCluster, asdu.ToArray());
    }

    // A real device echoes the request's own transaction sequence number back in its response, and
    // DeviceInterview now correlates on that number, so these fixtures take it explicitly rather
    // than hardcoding 0x00.
    private static IndicationBody ActiveEndpointsResponse(
        DeviceScript device,
        byte[] endpointIds,
        byte sequenceNumber = 0x00
    )
    {
        var asdu = new List<byte> { sequenceNumber, 0x00 };
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

    private static IndicationBody ActiveEndpointsNak(DeviceScript device, ZdoStatus status, byte sequenceNumber = 0x00)
    {
        var asdu = new List<byte> { sequenceNumber, (byte)status };
        return new IndicationBody(
            device.Nwk,
            SourceEndpoint: 0x00,
            ZdpProfile,
            ActiveEndpointsResponseCluster,
            asdu.ToArray()
        );
    }

    private static IndicationBody SimpleDescriptorResponse(
        DeviceScript device,
        byte endpoint,
        ushort profileId,
        ushort deviceId,
        ushort[] inClusters,
        ushort[] outClusters,
        byte sequenceNumber
    )
    {
        var descriptor = new List<byte> { endpoint };
        AddUInt16(descriptor, profileId);
        AddUInt16(descriptor, deviceId);
        descriptor.Add(0x00);
        AddClusterList(descriptor, inClusters);
        AddClusterList(descriptor, outClusters);

        var asdu = new List<byte> { sequenceNumber, 0x00 };
        AddUInt16(asdu, device.Nwk);
        asdu.Add((byte)descriptor.Count);
        asdu.AddRange(descriptor);
        return new IndicationBody(device.Nwk, endpoint, ZdpProfile, SimpleDescriptorResponseCluster, asdu.ToArray());
    }

    private static IndicationBody SimpleDescriptorNak(
        DeviceScript device,
        byte endpoint,
        ZdoStatus status,
        byte sequenceNumber = 0x00
    )
    {
        var asdu = new List<byte> { sequenceNumber, (byte)status };
        return new IndicationBody(device.Nwk, endpoint, ZdpProfile, SimpleDescriptorResponseCluster, asdu.ToArray());
    }

    private static IndicationBody BasicReadResponse(
        DeviceScript device,
        byte endpoint,
        ushort profileId,
        string manufacturer,
        string model,
        byte sequenceNumber = 0x00
    )
    {
        var frame = new List<byte> { GlobalServerToClientControl, sequenceNumber, ReadAttributesResponseCommand };
        AddStringAttribute(frame, ManufacturerNameAttribute, manufacturer);
        AddStringAttribute(frame, ModelIdentifierAttribute, model);
        return new IndicationBody(device.Nwk, endpoint, profileId, BasicCluster, frame.ToArray());
    }

    private static void AddStringAttribute(List<byte> frame, ushort attributeId, string value)
    {
        AddUInt16(frame, attributeId);
        frame.Add(0x00);
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

    private record DeviceScript(ushort Nwk, ulong Ieee);
}
