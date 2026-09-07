using System.Linq;
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
    public void Map_MapsTheFullEndpointListNotJustOneWinner()
    {
        var endpoints = new[]
        {
            new ZigbeeEndpoint(1, 0x0104, 0x0100, [0x0000, 0x0006], [0x0019]),
            new ZigbeeEndpoint(2, 0x0104, 0x0102, [0x0300], []),
        };
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x1234, endpoints, "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal(2, result.Endpoints.Length);
        Assert.Contains(
            result.Endpoints,
            e => e.EndpointId == 1 && e.InClusters.SequenceEqual(new ushort[] { 0x0000, 0x0006 })
        );
        Assert.Contains(
            result.Endpoints,
            e => e.EndpointId == 2 && e.InClusters.SequenceEqual(new ushort[] { 0x0300 })
        );
    }

    [Fact]
    public void Map_SetsNetworkAddress()
    {
        var joined = new ZigbeeDeviceJoined(new IeeeAddress(1), 0x9abc, [], "acme", "widget-1");

        var result = _mapper.Map(joined);

        Assert.Equal((ushort)0x9abc, result.NetworkAddress);
    }
}
