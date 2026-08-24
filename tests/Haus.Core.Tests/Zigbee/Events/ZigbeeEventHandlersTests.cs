using System.Threading.Tasks;
using Haus.Core.Common.Events;
using Haus.Core.Models.Zigbee;
using Haus.Core.Models.Zigbee.Events;
using Haus.Core.Zigbee.State;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Zigbee.Events;

public class ZigbeeEventHandlersTests
{
    private readonly IHausBus _hausBus;
    private readonly IZigbeeStore _store;

    public ZigbeeEventHandlersTests()
    {
        _store = new ZigbeeStore();
        _hausBus = HausBusFactory.Create(configureServices: services => services.Replace<IZigbeeStore>(_store));
    }

    [Fact]
    public async Task WhenConnectionStatusChangedEventReceivedThenStateHasNewConnectionStatus()
    {
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(new ZigbeeConnectionStatusChangedEvent(true, null, "connected"))
        );

        Assert.True(_store.Current.ConnectionStatus.IsConnected);
        Assert.Equal("connected", _store.Current.ConnectionStatus.Reason);
    }

    [Fact]
    public async Task WhenConnectionStatusChangedEventReceivedThenActivityIsRecorded()
    {
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(new ZigbeeConnectionStatusChangedEvent(true, null, "connected"))
        );

        Assert.Contains(_store.Current.RecentActivity, e => e.EventType == ZigbeeConnectionStatusChangedEvent.Type);
    }

    [Fact]
    public async Task WhenDeviceJoinedEventReceivedThenStateHasKnownDevice()
    {
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(new ZigbeeDeviceJoinedEvent("ieee-1", 42)));

        var device = Assert.Single(_store.Current.KnownDevices.Values);
        Assert.Equal("ieee-1", device.IeeeAddress);
        Assert.Equal((ushort)42, device.NetworkAddress);
    }

    [Fact]
    public async Task WhenDeviceJoinedEventReceivedThenActivityIsRecorded()
    {
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(new ZigbeeDeviceJoinedEvent("ieee-1", 42)));

        Assert.Contains(_store.Current.RecentActivity, e => e.EventType == ZigbeeDeviceJoinedEvent.Type);
    }

    [Fact]
    public async Task WhenDeviceInfoDiscoveredEventReceivedThenStateHasKnownDeviceInfo()
    {
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(new ZigbeeDeviceInfoDiscoveredEvent("ieee-1", "Acme", "Widget", []))
        );

        var device = Assert.Single(_store.Current.KnownDevices.Values);
        Assert.Equal("Acme", device.ManufacturerName);
        Assert.Equal("Widget", device.ModelIdentifier);
    }

    [Fact]
    public async Task WhenAttributeReportReceivedEventReceivedThenActivityIsRecorded()
    {
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(new ZigbeeAttributeReportReceivedEvent(1, "ieee-1", 1, 1, 0, 0, null))
        );

        Assert.Contains(_store.Current.RecentActivity, e => e.EventType == ZigbeeAttributeReportReceivedEvent.Type);
    }

    [Fact]
    public async Task WhenCommandSentEventReceivedThenActivityIsRecorded()
    {
        await _hausBus.PublishAsync(RoutableEvent.FromEvent(new ZigbeeCommandSentEvent(1, "ieee-1", 1, 1)));

        Assert.Contains(_store.Current.RecentActivity, e => e.EventType == ZigbeeCommandSentEvent.Type);
    }

    [Fact]
    public async Task WhenTransportErrorEventReceivedThenActivityIsRecorded()
    {
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(new ZigbeeTransportErrorEvent("timeout", "boom", 1, "ieee-1"))
        );

        Assert.Contains(_store.Current.RecentActivity, e => e.EventType == ZigbeeTransportErrorEvent.Type);
    }

    [Fact]
    public async Task WhenCommandDroppedEventReceivedThenActivityIsRecorded()
    {
        await _hausBus.PublishAsync(
            RoutableEvent.FromEvent(new ZigbeeCommandDroppedEvent("ext-1", "no known network address"))
        );

        Assert.Contains(_store.Current.RecentActivity, e => e.EventType == ZigbeeCommandDroppedEvent.Type);
    }
}
