using System;
using System.Collections.Immutable;
using System.Linq;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.State;
using Xunit;

namespace Haus.Core.Tests.Zigbee.State;

public class ZigbeeStateTests
{
    [Fact]
    public void WhenInitialThenConnectionStatusIsUnknown()
    {
        Assert.Equal(ZigbeeConnectionStatusModel.Unknown, ZigbeeState.Initial.ConnectionStatus);
    }

    [Fact]
    public void WhenConnectionStatusIsUpdatedThenStateHasNewStatus()
    {
        var status = new ZigbeeConnectionStatusModel(true, "connected", DateTimeOffset.UtcNow);

        var state = ZigbeeState.Initial.UpdateConnectionStatus(status);

        Assert.Equal(status, state.ConnectionStatus);
    }

    [Fact]
    public void WhenActivityIsRecordedThenStateHasActivity()
    {
        var entry = new ZigbeeActivityEntryModel("some_type", DateTimeOffset.UtcNow, new { });

        var state = ZigbeeState.Initial.RecordActivity(entry);

        Assert.Contains(entry, state.RecentActivity);
    }

    [Fact]
    public void WhenActivityCountExceedsMaxThenOldestActivityIsDropped()
    {
        var state = Enumerable
            .Range(0, ZigbeeState.MaxRecentActivity + 1)
            .Aggregate(
                (IZigbeeState)ZigbeeState.Initial,
                (s, i) => s.RecordActivity(new ZigbeeActivityEntryModel($"type-{i}", DateTimeOffset.UtcNow, i))
            );

        Assert.Equal(ZigbeeState.MaxRecentActivity, state.RecentActivity.Length);
        Assert.DoesNotContain(state.RecentActivity, e => e.EventType == "type-0");
        Assert.Contains(state.RecentActivity, e => e.EventType == $"type-{ZigbeeState.MaxRecentActivity}");
    }

    [Fact]
    public void WhenDeviceJoinedThenKnownDevicesHasDeviceWithNetworkAddress()
    {
        var seenAt = DateTimeOffset.UtcNow;

        var state = ZigbeeState.Initial.RecordDeviceJoined("ieee-1", 42, seenAt);

        var device = Assert.Single(state.KnownDevices.Values);
        Assert.Equal("ieee-1", device.IeeeAddress);
        Assert.Equal((ushort)42, device.NetworkAddress);
        Assert.Equal(seenAt, device.LastSeenAt);
    }

    [Fact]
    public void WhenDeviceInfoDiscoveredAfterJoinThenExistingDeviceIsAugmentedNotReplaced()
    {
        var joinedAt = DateTimeOffset.UtcNow;
        var discoveredAt = joinedAt.AddSeconds(1);
        var endpoints = ImmutableArray<ZigbeeEndpointModel>.Empty;

        var state = ZigbeeState
            .Initial.RecordDeviceJoined("ieee-1", 42, joinedAt)
            .RecordDeviceInfoDiscovered("ieee-1", "Acme", "Widget", endpoints, discoveredAt);

        var device = Assert.Single(state.KnownDevices.Values);
        Assert.Equal((ushort)42, device.NetworkAddress);
        Assert.Equal("Acme", device.ManufacturerName);
        Assert.Equal("Widget", device.ModelIdentifier);
        Assert.Equal(discoveredAt, device.LastSeenAt);
    }

    [Fact]
    public void WhenDeviceInfoDiscoveredWithoutPriorJoinThenDeviceIsAddedWithNoNetworkAddress()
    {
        var state = ZigbeeState.Initial.RecordDeviceInfoDiscovered(
            "ieee-2",
            "Acme",
            "Widget",
            ImmutableArray<ZigbeeEndpointModel>.Empty,
            DateTimeOffset.UtcNow
        );

        var device = Assert.Single(state.KnownDevices.Values);
        Assert.Null(device.NetworkAddress);
    }
}
