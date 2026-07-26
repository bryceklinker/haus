using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Zcl;
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

        _ = coordinator.SendCommandAsync(request, Cancelled());

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

    private static CancellationToken Cancelled()
    {
        var source = new CancellationTokenSource();
        source.Cancel();
        return source.Token;
    }
}
