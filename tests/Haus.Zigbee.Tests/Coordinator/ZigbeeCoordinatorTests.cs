using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Models;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Simulator;
using Haus.Zigbee.Transport;
using Haus.Zigbee.Zcl;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class ZigbeeCoordinatorTests
{
    private const byte MacAddressParameterId = 0x01;
    private const byte PanIdParameterId = 0x05;
    private const byte ChannelParameterId = 0x1C;
    private const byte PermitJoinParameterId = 0x21;

    private readonly FakeDeconzCoordinator _dongle = new();

    [Fact]
    public async Task WhenConnectedThenItReportsTheNetworkConfigReadFromTheCoordinator()
    {
        _dongle.SetParameter(MacAddressParameterId, new byte[] { 0x22, 0x11, 0x00, 0xff, 0xff, 0x2e, 0x21, 0x00 });
        _dongle.SetParameter(PanIdParameterId, new byte[] { 0x62, 0x1a });
        _dongle.SetParameter(ChannelParameterId, new byte[] { 0x0f });
        using var coordinator = new ZigbeeCoordinator(_dongle);

        await coordinator.ConnectAsync(CancellationToken.None);

        Assert.True(coordinator.IsConnected);
        var config = coordinator.NetworkConfig;
        Assert.NotNull(config);
        Assert.Equal(new IeeeAddress(0x00212effff001122), config.MacAddress);
        Assert.Equal((ushort)0x1a62, config.PanId);
        Assert.Equal((byte)0x0f, config.Channel);
    }

    [Fact]
    public async Task WhenNoDeviceHasJoinedThenGetDevicesReturnsAnEmptyList()
    {
        using var coordinator = new ZigbeeCoordinator(_dongle);

        var devices = await coordinator.GetDevicesAsync(CancellationToken.None);

        Assert.Empty(devices);
    }

    [Fact]
    public async Task WhenEnablingPermitJoinThenItWritesToThePermitJoinParameter()
    {
        using var coordinator = new ZigbeeCoordinator(_dongle);

        await coordinator.SetPermitJoinAsync(true, CancellationToken.None);

        var written = Assert.Single(_dongle.WrittenParameters);
        Assert.Equal(PermitJoinParameterId, written.ParameterId);
        Assert.Equal(new byte[] { 0xFF }, written.Value);
    }

    [Fact]
    public void WhenSendingACommandThenItWritesTheApsRequestForTheBuiltZclFrame()
    {
        using var coordinator = new ZigbeeCoordinator(_dongle);
        var request = new ZigbeeCommandRequest(
            Destination: ApsDestination.Nwk(0x1234, 0x01),
            SourceEndpoint: 0x01,
            ProfileId: 0x0104,
            ClusterId: 0x0006,
            CommandId: 0x01,
            Payload: new byte[] { 0xaa, 0xbb },
            DisableDefaultResponse: true
        );

        // Never awaited/completed (no confirm is ever queued) -- this only checks the bytes
        // written for the encoded request, not the eventual result. ConnectAsync/polling was
        // never started for this coordinator, so nothing else contends for the channel.
        _ = coordinator.SendCommandAsync(request, CancellationToken.None);

        var written = Assert.Single(_dongle.ApsRequestFrames);
        Assert.Equal(ApsDataRequestFrameCodec.Encode(ExpectedApsFrame(request)), written);
    }

    private static ApsDataRequestFrame ExpectedApsFrame(ZigbeeCommandRequest request)
    {
        var asdu = ZclCommandFactory.Encode(
            new ZclCommand(
                TransactionSequenceNumber: 0,
                request.CommandId,
                request.Payload,
                request.DisableDefaultResponse
            )
        );
        return new ApsDataRequestFrame(
            SequenceNumber: 0,
            RequestId: 0,
            request.Destination,
            request.ProfileId,
            request.ClusterId,
            request.SourceEndpoint,
            AsduPayload: asdu,
            TxOptions: 0x00,
            Radius: 0x00
        );
    }

    [Fact]
    public async Task WhenAnAttributeReportArrivesWhileConnectedThenItIsRelayedThroughTheAttributeReportedEvent()
    {
        SetNetworkParameters();
        using var coordinator = new ZigbeeCoordinator(_dongle);
        var reported = new TaskCompletionSource<ZigbeeAttributeReport>();
        coordinator.AttributeReported += (_, report) => reported.TrySetResult(report);
        await coordinator.ConnectAsync(CancellationToken.None);

        _dongle.InjectIndication(ReportAttributesIndication(sourceNwk: 0x1234, clusterId: 0x0400, attributeId: 0x0000));

        var report = await WaitFor(reported.Task);
        Assert.Equal(0x1234, report.SourceNwkAddress);
        Assert.Equal(0x0400, report.ClusterId);
        Assert.Equal(0x0000, report.AttributeId);
    }

    [Fact]
    public async Task WhenDisposedThenTheBackgroundPollLoopStops()
    {
        SetNetworkParameters();
        var coordinator = new ZigbeeCoordinator(_dongle);
        await coordinator.ConnectAsync(CancellationToken.None);
        await WaitUntilPolledAtLeastOnce();

        coordinator.Dispose();
        await Task.Delay(TimeSpan.FromMilliseconds(300));
        var pollsAfterDispose = _dongle.DeviceStatePollCount;
        await Task.Delay(TimeSpan.FromMilliseconds(300));

        Assert.Equal(pollsAfterDispose, _dongle.DeviceStatePollCount);
    }

    [Fact]
    public async Task WhenDisposedTwiceThenItDoesNotThrow()
    {
        SetNetworkParameters();
        var coordinator = new ZigbeeCoordinator(_dongle);
        await coordinator.ConnectAsync(CancellationToken.None);

        coordinator.Dispose();
        var secondDispose = Record.Exception(coordinator.Dispose);

        Assert.Null(secondDispose);
    }

    private async Task WaitUntilPolledAtLeastOnce()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (_dongle.DeviceStatePollCount == 0 && !timeout.IsCancellationRequested)
            await Task.Delay(10);
        Assert.True(_dongle.DeviceStatePollCount > 0, "the poll loop never polled");
    }

    [Fact]
    public async Task WhenAPollIterationThrowsThenTheFailureIsLoggedAndPollingContinues()
    {
        SetNetworkParameters();
        var transport = new FailOnceOnReadTransport(_dongle, failOnReadCall: 4);
        var loggerFactory = new CapturingLoggerFactory();
        using var coordinator = new ZigbeeCoordinator(transport, loggerFactory);

        await coordinator.ConnectAsync(CancellationToken.None);
        await WaitUntilPolledAtLeast(2);

        Assert.Contains(
            loggerFactory.Entries,
            entry => entry.Category == typeof(ZigbeeCoordinator).FullName && entry.Exception is not null
        );
    }

    private async Task WaitUntilPolledAtLeast(int count)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (_dongle.DeviceStatePollCount < count && !timeout.IsCancellationRequested)
            await Task.Delay(10);
        Assert.True(_dongle.DeviceStatePollCount >= count, "the poll loop did not poll enough times");
    }

    // Throws once, on a chosen ReadAsync call, to simulate a transient protocol-level hiccup (e.g.
    // a garbled/truncated frame) during a poll iteration -- proving PollSafelyAsync's catch both
    // logs it and lets the loop continue. Deliberately not IOException/ObjectDisposedException/
    // SerialTransportTimeoutException: those are classified as fatal (the transport itself is
    // dead), whereas this represents a one-off parse failure that has nothing wrong with the
    // transport and should simply be retried on the next poll.
    private class FailOnceOnReadTransport(ISerialTransport inner, int failOnReadCall) : ISerialTransport
    {
        private int _readCallCount;

        public Task OpenAsync(CancellationToken token) => inner.OpenAsync(token);

        public Task CloseAsync(CancellationToken token) => inner.CloseAsync(token);

        public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token) => inner.WriteAsync(buffer, token);

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token)
        {
            _readCallCount++;
            if (_readCallCount == failOnReadCall)
                throw new FormatException("Simulated garbled frame");
            return inner.ReadAsync(buffer, token);
        }

        public void Dispose() => inner.Dispose();
    }

    // Each CreateLogger call gets its own logger instance tagged with its own category, all
    // writing into one shared, lock-protected list -- so entries stay attributed to whichever
    // type actually logged them, even though several types share this one factory.
    private class CapturingLoggerFactory : ILoggerFactory
    {
        private readonly List<LogEntry> _entries = new();

        public IReadOnlyList<LogEntry> Entries
        {
            get
            {
                lock (_entries)
                    return _entries.ToList();
            }
        }

        public ILogger CreateLogger(string categoryName) => new CategoryLogger(categoryName, _entries);

        public void AddProvider(ILoggerProvider provider) { }

        public void Dispose() { }

        private class CategoryLogger(string category, List<LogEntry> entries) : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
                where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter
            )
            {
                lock (entries)
                    entries.Add(new LogEntry(category, logLevel, exception));
            }
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose() { }
        }
    }

    private record LogEntry(string Category, LogLevel Level, Exception? Exception);

    [Fact]
    public async Task WhenADeviceAnnouncesWhileConnectedThenItIsRelayedThroughTheDeviceJoinedEvent()
    {
        SetNetworkParameters();
        using var coordinator = new ZigbeeCoordinator(_dongle);
        var joined = new TaskCompletionSource<ZigbeeDeviceJoined>();
        coordinator.DeviceJoined += (_, device) => joined.TrySetResult(device);
        await coordinator.ConnectAsync(CancellationToken.None);

        DriveJoinWithNoEndpoints(networkAddress: 0x1a2b, ieee: 0x00124b0001aabbcc);

        var device = await WaitFor(joined.Task);
        Assert.Equal(new IeeeAddress(0x00124b0001aabbcc), device.IeeeAddress);
        Assert.Equal((ushort)0x1a2b, device.NetworkAddress);
        Assert.Empty(device.Endpoints);
    }

    [Fact]
    public async Task WhenADeviceHasJoinedThenGetDevicesReflectsTheKnownDeviceTable()
    {
        SetNetworkParameters();
        using var coordinator = new ZigbeeCoordinator(_dongle);
        var joined = new TaskCompletionSource<ZigbeeDeviceJoined>();
        coordinator.DeviceJoined += (_, device) => joined.TrySetResult(device);
        await coordinator.ConnectAsync(CancellationToken.None);

        DriveJoinWithNoEndpoints(networkAddress: 0x1a2b, ieee: 0x00124b0001aabbcc);
        await WaitFor(joined.Task);

        var devices = await coordinator.GetDevicesAsync(CancellationToken.None);
        var device = Assert.Single(devices);
        Assert.Equal(new IeeeAddress(0x00124b0001aabbcc), device.IeeeAddress);
    }

    [Fact]
    public async Task WhenAddressIsNotKnownThenReadDeviceInfoReturnsNull()
    {
        using var coordinator = new ZigbeeCoordinator(_dongle);

        var info = await coordinator.ReadDeviceInfoAsync(new IeeeAddress(1), CancellationToken.None);

        Assert.Null(info);
    }

    [Fact]
    public async Task WhenDeviceIsKnownButHasNoEndpointsThenReadDeviceInfoReturnsEmptyInfo()
    {
        SetNetworkParameters();
        using var coordinator = new ZigbeeCoordinator(_dongle);
        var joined = new TaskCompletionSource<ZigbeeDeviceJoined>();
        coordinator.DeviceJoined += (_, device) => joined.TrySetResult(device);
        await coordinator.ConnectAsync(CancellationToken.None);
        DriveJoinWithNoEndpoints(networkAddress: 0x1a2b, ieee: 0x00124b0001aabbcc);
        await WaitFor(joined.Task);

        var info = await coordinator.ReadDeviceInfoAsync(new IeeeAddress(0x00124b0001aabbcc), CancellationToken.None);

        Assert.NotNull(info);
        Assert.Equal(string.Empty, info!.ManufacturerName);
        Assert.Equal(string.Empty, info.ModelIdentifier);
    }

    private void DriveJoinWithNoEndpoints(ushort networkAddress, ulong ieee)
    {
        _dongle.InjectIndication(AnnounceIndication(networkAddress, ieee));
        _dongle.ReleaseAfterApsRequest(apsRequestIndex: 0, ActiveEndpointsIndication(networkAddress));
    }

    private static IndicationBody AnnounceIndication(ushort networkAddress, ulong ieee)
    {
        const ushort zdpProfile = 0x0000;
        const ushort deviceAnnounceCluster = 0x0013;
        const byte macCapability = 0x80;
        var asdu = new List<byte> { 0x00 };
        AddUInt16(asdu, networkAddress);
        AddUInt64(asdu, ieee);
        asdu.Add(macCapability);
        return new IndicationBody(
            networkAddress,
            SourceEndpoint: 0x00,
            zdpProfile,
            deviceAnnounceCluster,
            asdu.ToArray()
        );
    }

    private static IndicationBody ActiveEndpointsIndication(ushort networkAddress)
    {
        const ushort zdpProfile = 0x0000;
        const ushort activeEndpointsResponseCluster = 0x8005;
        const byte successStatus = 0x00;
        const byte endpointCount = 0x00;
        var asdu = new List<byte> { 0x00, successStatus };
        AddUInt16(asdu, networkAddress);
        asdu.Add(endpointCount);
        return new IndicationBody(
            networkAddress,
            SourceEndpoint: 0x00,
            zdpProfile,
            activeEndpointsResponseCluster,
            asdu.ToArray()
        );
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

    private void SetNetworkParameters()
    {
        _dongle.SetParameter(MacAddressParameterId, new byte[] { 0x22, 0x11, 0x00, 0xff, 0xff, 0x2e, 0x21, 0x00 });
        _dongle.SetParameter(PanIdParameterId, new byte[] { 0x62, 0x1a });
        _dongle.SetParameter(ChannelParameterId, new byte[] { 0x0f });
    }

    private static IndicationBody ReportAttributesIndication(ushort sourceNwk, ushort clusterId, ushort attributeId)
    {
        const byte globalFrameControl = 0x00;
        const byte transactionSequenceNumber = 0x01;
        const byte reportAttributesCommand = 0x0a;
        var zclFrame = new byte[]
        {
            globalFrameControl,
            transactionSequenceNumber,
            reportAttributesCommand,
            (byte)(attributeId & 0xff),
            (byte)(attributeId >> 8),
            (byte)ZclDataType.Uint16,
            0x34,
            0x12,
        };
        return new IndicationBody(sourceNwk, SourceEndpoint: 0x05, ProfileId: 0x0104, clusterId, zclFrame);
    }

    private static async Task<T> WaitFor<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.Same(task, completed);
        return await task;
    }
}
