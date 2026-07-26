using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Zigbee;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus;

public class DevicesMapperTests
{
    private readonly DevicesMapper _mapper = new();

    [Fact]
    public void Map_UsesExternalIdMapForId()
    {
        var address = new IeeeAddress(0x00124b0012345678);
        var device = new ZigbeeDevice(address, 0x1234, []);

        var result = _mapper.Map([device]).Single();

        Assert.Equal(ExternalIdMap.ToExternalId(address), result.Id);
    }

    [Fact]
    public void Map_DeviceTypeIsUnknown()
    {
        var device = new ZigbeeDevice(new IeeeAddress(1), 0x1234, []);

        var result = _mapper.Map([device]).Single();

        Assert.Equal(DeviceType.Unknown, result.DeviceType);
    }

    [Fact]
    public void Map_MetadataIncludesNetworkAddress()
    {
        var device = new ZigbeeDevice(new IeeeAddress(1), 0x9abc, []);

        var result = _mapper.Map([device]).Single();

        Assert.Contains(new MetadataModel("network_address", "39612"), result.Metadata);
    }

    [Fact]
    public void Map_MultipleDevices_ReturnsOneEventPerDevice()
    {
        var devices = new List<ZigbeeDevice>
        {
            new(new IeeeAddress(1), 1, []),
            new(new IeeeAddress(2), 2, []),
            new(new IeeeAddress(3), 3, []),
        };

        var result = _mapper.Map(devices);

        Assert.Equal(3, result.Count());
    }
}
