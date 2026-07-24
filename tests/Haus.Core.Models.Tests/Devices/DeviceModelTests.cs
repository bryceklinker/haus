using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Xunit;

namespace Haus.Core.Models.Tests.Devices;

public class DeviceModelTests
{
    [Fact]
    public void WhenTwoInstancesShareAnIdThenTheyAreEqualRegardlessOfMetadataArrayIdentity()
    {
        var first = new DeviceModel(Id: 1, ExternalId: "device-1", Metadata: [new MetadataModel("key", "value")]);
        var second = new DeviceModel(Id: 1, ExternalId: "device-1", Metadata: [new MetadataModel("key", "value")]);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void WhenTwoInstancesHaveDifferentIdsThenTheyAreNotEqual()
    {
        var first = new DeviceModel(Id: 1, ExternalId: "device-1");
        var second = new DeviceModel(Id: 2, ExternalId: "device-1");

        Assert.NotEqual(first, second);
    }
}
