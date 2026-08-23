using System;
using System.Reactive.Linq;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.State;
using Xunit;

namespace Haus.Core.Tests.Zigbee.State;

public class ZigbeeStoreTests
{
    [Fact]
    public void WhenCreatedThenInitialState()
    {
        var store = new ZigbeeStore();

        Assert.Equal(ZigbeeState.Initial, store.Current);
    }

    [Fact]
    public void WhenPublishedThenCurrentStateIsUpdated()
    {
        var status = new ZigbeeConnectionStatusModel(true, "connected", DateTimeOffset.UtcNow);

        var store = new ZigbeeStore();
        store.Publish(store.Current.UpdateConnectionStatus(status));

        Assert.Equal(status, store.Current.ConnectionStatus);
    }

    [Fact]
    public void WhenPublishNextIsUsedAndNewStateIsUnchangedThenNoUpdatesAreSent()
    {
        var publishCount = 0;

        var store = new ZigbeeStore();
        store.Skip(1).Subscribe(s => publishCount++);

        store.PublishNext(s => s);

        Assert.Equal(0, publishCount);
    }
}
