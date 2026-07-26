using Haus.Zigbee.Coordinator;
using Xunit;

namespace Haus.Zigbee.Tests.Coordinator;

public class KnownDeviceTableTests
{
    [Fact]
    public void WhenNoDevicesHaveBeenAddedThenGetDevicesReturnsEmptyList()
    {
        var table = new KnownDeviceTable();

        Assert.Empty(table.GetDevices());
    }

    [Fact]
    public void WhenADeviceIsAddedThenItAppearsInGetDevices()
    {
        var table = new KnownDeviceTable();
        var device = new ZigbeeDevice(new IeeeAddress(0x00124b0001234567), 0x1234, new ZigbeeEndpoint[0]);

        table.AddOrUpdate(device);

        Assert.Equal(new[] { device }, table.GetDevices());
    }
}
