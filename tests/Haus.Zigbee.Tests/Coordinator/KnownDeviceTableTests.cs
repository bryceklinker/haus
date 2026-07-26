using System.Linq;
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

    [Fact]
    public void WhenTwoDevicesWithDifferentAddressesAreAddedThenBothAppearInGetDevices()
    {
        var table = new KnownDeviceTable();
        var first = new ZigbeeDevice(new IeeeAddress(0x00124b0001234567), 0x1234, new ZigbeeEndpoint[0]);
        var second = new ZigbeeDevice(new IeeeAddress(0x00124b0007654321), 0x5678, new ZigbeeEndpoint[0]);

        table.AddOrUpdate(first);
        table.AddOrUpdate(second);

        Assert.Equal(new[] { first, second }, table.GetDevices().OrderBy(device => device.NetworkAddress));
    }

    [Fact]
    public void WhenADeviceIsAddedWithAnAlreadyKnownAddressThenItReplacesThePriorEntry()
    {
        var table = new KnownDeviceTable();
        var address = new IeeeAddress(0x00124b0001234567);
        var original = new ZigbeeDevice(address, 0x1234, new ZigbeeEndpoint[0]);
        var updated = new ZigbeeDevice(address, 0x9999, new ZigbeeEndpoint[0]);

        table.AddOrUpdate(original);
        table.AddOrUpdate(updated);

        Assert.Equal(new[] { updated }, table.GetDevices());
    }
}
