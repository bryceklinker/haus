using System;
using System.Threading.Tasks;
using Haus.Core.Models.Zigbee;
using Haus.Core.Zigbee.Queries;
using Haus.Core.Zigbee.State;
using Haus.Cqrs;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Zigbee.Queries;

public class ZigbeeQueriesTests
{
    private readonly IHausBus _hausBus;
    private readonly IZigbeeStore _store;

    public ZigbeeQueriesTests()
    {
        _store = new ZigbeeStore();
        _hausBus = HausBusFactory.Create(configureServices: services => services.Replace<IZigbeeStore>(_store));
    }

    [Fact]
    public async Task WhenGettingConnectionStatusThenReturnsCurrentConnectionStatus()
    {
        var status = new ZigbeeConnectionStatusModel(true, "connected", DateTimeOffset.UtcNow);
        _store.Publish(_store.Current.UpdateConnectionStatus(status));

        var result = await _hausBus.ExecuteQueryAsync(new GetZigbeeConnectionStatusQuery());

        Assert.Equal(status, result);
    }

    [Fact]
    public async Task WhenGettingRecentActivityThenReturnsRecordedActivity()
    {
        var entry = new ZigbeeActivityEntryModel("some_type", DateTimeOffset.UtcNow, new { });
        _store.Publish(_store.Current.RecordActivity(entry));

        var result = await _hausBus.ExecuteQueryAsync(new GetRecentZigbeeActivityQuery());

        Assert.Contains(entry, result.Items);
    }

    [Fact]
    public async Task WhenGettingKnownDevicesThenReturnsKnownDevices()
    {
        _store.Publish(_store.Current.RecordDeviceJoined("ieee-1", 42, DateTimeOffset.UtcNow));

        var result = await _hausBus.ExecuteQueryAsync(new GetKnownZigbeeDevicesQuery());

        Assert.Contains(result.Items, d => d.IeeeAddress == "ieee-1");
    }
}
