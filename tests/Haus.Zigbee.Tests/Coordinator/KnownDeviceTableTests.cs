using System;
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
        var device = DeviceWith(0x00124b0001234567, 0x1234);

        table.AddOrUpdate(device);

        Assert.Equal(new[] { device }, table.GetDevices());
    }

    [Fact]
    public void WhenTwoDevicesWithDifferentAddressesAreAddedThenBothAppearInGetDevices()
    {
        var table = new KnownDeviceTable();
        var first = DeviceWith(0x00124b0001234567, 0x1234);
        var second = DeviceWith(0x00124b0007654321, 0x5678);

        table.AddOrUpdate(first);
        table.AddOrUpdate(second);

        Assert.Equal(new[] { first, second }, table.GetDevices().OrderBy(device => device.NetworkAddress));
    }

    [Fact]
    public void WhenADeviceIsAddedWithAnAlreadyKnownAddressThenItReplacesThePriorEntry()
    {
        var table = new KnownDeviceTable();
        var original = DeviceWith(0x00124b0001234567, 0x1234);
        var updated = DeviceWith(0x00124b0001234567, 0x9999);

        table.AddOrUpdate(original);
        table.AddOrUpdate(updated);

        Assert.Equal(new[] { updated }, table.GetDevices());
    }

    private static ZigbeeDevice DeviceWith(ulong ieeeAddress, ushort networkAddress)
    {
        return new ZigbeeDevice(new IeeeAddress(ieeeAddress), networkAddress, Array.Empty<ZigbeeEndpoint>());
    }
}
