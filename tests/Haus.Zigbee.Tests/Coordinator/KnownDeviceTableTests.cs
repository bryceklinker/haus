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
}
