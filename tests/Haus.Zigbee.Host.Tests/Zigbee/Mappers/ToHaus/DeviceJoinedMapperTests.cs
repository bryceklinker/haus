using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus;
using Haus.Zigbee.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus;

public class DeviceJoinedMapperTests
{
    private readonly DeviceJoinedMapper _mapper;

    public DeviceJoinedMapperTests()
    {
        var provider = ServiceProviderFactory.Create();
        _mapper = provider.GetRequiredService<DeviceJoinedMapper>();
    }

    [Fact]
    public void Map_UsesExternalIdConverterForId()
    {
        var address = new IeeeAddress(0x00124b0012345678);
        var joined = new ZigbeeDeviceJoined(address, 0x1234, [], "vendor", "model");

        var result = _mapper.Map(joined);

        Assert.Equal(ExternalIdConverter.ToExternalId(address), result.Id);
    }

    [Fact]
    public void Map_ResolvesDeviceTypeFromManufacturerAndModel()
    {
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [], "Philips", "929002335001");

        var result = _mapper.Map(joined);

        Assert.Equal(DeviceType.Light, result.DeviceType);
    }

    [Fact]
    public void Map_MetadataIncludesVendorAndModel()
    {
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Contains(new MetadataModel("vendor", "acme"), result.Metadata);
        Assert.Contains(new MetadataModel("model", "widget-1"), result.Metadata);
    }

    [Fact]
    public void Map_SetsNetworkAddress()
    {
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x9abc, [], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((ushort)0x9abc, result.NetworkAddress);
    }

    [Fact]
    public void Map_NoEndpoints_EndpointIdIsNull()
    {
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Null(result.EndpointId);
    }

    [Fact]
    public void Map_SingleEndpointWithNoLightingClusters_UsesThatEndpoint()
    {
        var endpoint = new ZigbeeEndpoint(0x0b, 0x0104, 0x0000, [0x0000], []);
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [endpoint], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((byte)0x0b, result.EndpointId);
    }

    [Fact]
    public void Map_MultipleEndpoints_PrefersEndpointWithOnOffCluster()
    {
        var other = new ZigbeeEndpoint(0x01, 0x0104, 0x0000, [0x0000], []);
        var onOff = new ZigbeeEndpoint(0x0b, 0x0104, 0x0100, [0x0006], []);
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [other, onOff], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((byte)0x0b, result.EndpointId);
    }

    [Fact]
    public void Map_MultipleEndpoints_NoOnOffFallsBackToLevelCluster()
    {
        var other = new ZigbeeEndpoint(0x01, 0x0104, 0x0000, [0x0000], []);
        var level = new ZigbeeEndpoint(0x0b, 0x0104, 0x0100, [0x0008], []);
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [other, level], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((byte)0x0b, result.EndpointId);
    }

    [Fact]
    public void Map_MultipleEndpoints_NoOnOffOrLevelFallsBackToColorControlCluster()
    {
        var other = new ZigbeeEndpoint(0x01, 0x0104, 0x0000, [0x0000], []);
        var color = new ZigbeeEndpoint(0x0b, 0x0104, 0x0100, [0x0300], []);
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [other, color], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((byte)0x0b, result.EndpointId);
    }

    [Fact]
    public void Map_MultipleEndpoints_NoneWithLightingClusters_FallsBackToFirstEndpoint()
    {
        var first = new ZigbeeEndpoint(0x01, 0x0104, 0x0000, [0x0000], []);
        var second = new ZigbeeEndpoint(0x0b, 0x0104, 0x0000, [0x0001], []);
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, [first, second], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((byte)0x01, result.EndpointId);
    }
}
