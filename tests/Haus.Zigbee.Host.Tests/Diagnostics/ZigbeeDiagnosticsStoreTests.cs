using System;
using Haus.Zigbee.Host.Diagnostics;
using Haus.Zigbee.Models.Diagnostics;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Diagnostics;

public class ZigbeeDiagnosticsStoreTests
{
    private readonly ZigbeeDiagnosticsStore _store = new();

    [Fact]
    public void WhenCreatedThenCurrentIsInitial()
    {
        Assert.Equal(ZigbeeConnectionStatusModel.Unknown, _store.Current.ConnectionStatus);
        Assert.Empty(_store.Current.RecentActivity);
    }

    [Fact]
    public void WhenConnectionStatusUpdatedThenCurrentReflectsIt()
    {
        var status = new ZigbeeConnectionStatusModel(true, "connected", DateTimeOffset.UtcNow, null);

        _store.Update(s => s.UpdateConnectionStatus(status));

        Assert.Equal(status, _store.Current.ConnectionStatus);
    }

    [Fact]
    public void WhenActivityRecordedThenItIsAppendedInOrder()
    {
        var first = new ZigbeeActivityEntryModel("first", DateTimeOffset.UtcNow, new object());
        var second = new ZigbeeActivityEntryModel("second", DateTimeOffset.UtcNow, new object());

        _store.Update(s => s.RecordActivity(first));
        _store.Update(s => s.RecordActivity(second));

        Assert.Equal(["first", "second"], [.. CollectEventTypes()]);
    }

    [Fact]
    public void WhenActivityExceedsMaxThenOldestEntriesAreDropped()
    {
        for (var i = 0; i < ZigbeeDiagnosticsState.MaxRecentActivity + 5; i++)
        {
            var index = i;
            _store.Update(s =>
                s.RecordActivity(new ZigbeeActivityEntryModel($"{index}", DateTimeOffset.UtcNow, index))
            );
        }

        Assert.Equal(ZigbeeDiagnosticsState.MaxRecentActivity, _store.Current.RecentActivity.Length);
        Assert.Equal("5", _store.Current.RecentActivity[0].EventType);
        Assert.Equal($"{ZigbeeDiagnosticsState.MaxRecentActivity + 4}", _store.Current.RecentActivity[^1].EventType);
    }

    private string[] CollectEventTypes()
    {
        var types = new string[_store.Current.RecentActivity.Length];
        for (var i = 0; i < types.Length; i++)
            types[i] = _store.Current.RecentActivity[i].EventType;
        return types;
    }
}
