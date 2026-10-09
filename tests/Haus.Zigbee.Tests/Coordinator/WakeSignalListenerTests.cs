using System;
using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Coordinator;
using Haus.Zigbee.Serial.Frames;
using Haus.Zigbee.Simulator;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class WakeSignalListenerTests
{
    [Fact]
    public async Task WhenAnyIndicationArrivesFromAHeldDevicesAddressThenItsHeldCommandIsReleased()
    {
        var dongle = new FakeDeconzDongle();
        var pollLoop = new ApsPollLoop(new DeconzChannel(dongle.PollTransport));
        var sleepyHold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.Delay(Timeout.Infinite));
        using var listener = new WakeSignalListener(pollLoop, sleepyHold);
        var key = DeviceKey.FromNwk(0x1a2b);
        var expected = AnyConfirm();

        var holdTask = sleepyHold.HoldAsync(key, _ => Task.FromResult(expected), CancellationToken.None);

        dongle.InjectIndication(
            new IndicationBody(SourceNwk: 0x1a2b, SourceEndpoint: 0x01, ProfileId: 0x0104, ClusterId: 0x0006, Asdu: [])
        );
        await pollLoop.PollOnceAsync(CancellationToken.None);

        var result = await holdTask;
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task WhenAnIndicationArrivesFromADeviceWithNoHeldCommandThenNothingHappens()
    {
        var dongle = new FakeDeconzDongle();
        var pollLoop = new ApsPollLoop(new DeconzChannel(dongle.PollTransport));
        var sleepyHold = new SleepyCommandHold(new SleepyHoldOptions(), _ => Task.Delay(Timeout.Infinite));
        using var listener = new WakeSignalListener(pollLoop, sleepyHold);

        dongle.InjectIndication(
            new IndicationBody(SourceNwk: 0x9999, SourceEndpoint: 0x01, ProfileId: 0x0104, ClusterId: 0x0006, Asdu: [])
        );

        await pollLoop.PollOnceAsync(CancellationToken.None);
    }

    private static ApsDataConfirm AnyConfirm()
    {
        return new ApsDataConfirm(
            SequenceNumber: 0x00,
            DeviceState: 0x00,
            RequestId: 0x00,
            DestinationAddressMode: DeconzAddressMode.Nwk,
            DestinationShortAddress: 0x1a2b,
            DestinationIeeeAddress: null,
            DestinationEndpoint: 0x01,
            SourceEndpoint: 0x01,
            ConfirmStatus: 0x00
        );
    }
}
