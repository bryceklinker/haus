using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Zigbee;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;
using Haus.Zigbee.Zcl;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.DeviceEvents;

public class DeviceEventMapperTests
{
    private const ushort PowerConfigCluster = 0x0001;
    private const ushort IlluminanceMeasurementCluster = 0x0400;
    private const ushort TemperatureMeasurementCluster = 0x0402;
    private const ushort OccupancySensingCluster = 0x0406;
    private const ushort BatteryPercentageAttribute = 0x0021;
    private const ushort MeasuredValueAttribute = 0x0000;
    private const ushort OccupancyAttribute = 0x0000;
    private const ushort OtherAttribute = 0x00ff;
    private const ushort KnownNetworkAddress = 0x1234;
    private const string KnownExternalId = "the-device-id";

    private readonly DeviceEventMapper _mapper;

    public DeviceEventMapperTests()
    {
        var provider = ServiceProviderFactory.Create();
        provider.GetRequiredService<DeviceAddressRegistry>().Register(KnownNetworkAddress, KnownExternalId);
        _mapper = provider.GetRequiredService<DeviceEventMapper>();
    }

    private static ZigbeeAttributeReport CreateReport(ushort clusterId, ushort attributeId, ZclAttributeValue value)
    {
        return new ZigbeeAttributeReport(KnownNetworkAddress, 1, clusterId, attributeId, value);
    }

    [Fact]
    public void Map_BatteryReport_ReturnsBatteryChangedEvent()
    {
        var report = CreateReport(
            PowerConfigCluster,
            BatteryPercentageAttribute,
            new ZclAttributeValue(ZclDataType.Uint8, 86)
        );

        var result = _mapper.Map(report);

        Assert.Equal(BatteryChangedModel.Type, result?.Type);
        var model = Assert.IsType<BatteryChangedModel>(result?.Payload);
        Assert.Equal(KnownExternalId, model.DeviceId);
    }

    [Fact]
    public void Map_IlluminanceReport_ReturnsIlluminanceChangedEvent()
    {
        var report = CreateReport(
            IlluminanceMeasurementCluster,
            MeasuredValueAttribute,
            new ZclAttributeValue(ZclDataType.Uint16, 10001)
        );

        var result = _mapper.Map(report);

        Assert.Equal(IlluminanceChangedModel.Type, result?.Type);
        var model = Assert.IsType<IlluminanceChangedModel>(result?.Payload);
        Assert.Equal(KnownExternalId, model.DeviceId);
    }

    [Fact]
    public void Map_TemperatureReport_ReturnsTemperatureChangedEvent()
    {
        var report = CreateReport(
            TemperatureMeasurementCluster,
            MeasuredValueAttribute,
            new ZclAttributeValue(ZclDataType.Int16, 6500)
        );

        var result = _mapper.Map(report);

        Assert.Equal(TemperatureChangedModel.Type, result?.Type);
        var model = Assert.IsType<TemperatureChangedModel>(result?.Payload);
        Assert.Equal(KnownExternalId, model.DeviceId);
    }

    [Fact]
    public void Map_OccupancyReport_ReturnsOccupancyChangedEvent()
    {
        var report = CreateReport(
            OccupancySensingCluster,
            OccupancyAttribute,
            new ZclAttributeValue(ZclDataType.Bitmap8, 1)
        );

        var result = _mapper.Map(report);

        Assert.Equal(OccupancyChangedModel.Type, result?.Type);
        var model = Assert.IsType<OccupancyChangedModel>(result?.Payload);
        Assert.Equal(KnownExternalId, model.DeviceId);
    }

    [Fact]
    public void Map_UnrecognizedClusterOrAttribute_ReturnsNull()
    {
        var report = CreateReport(PowerConfigCluster, OtherAttribute, new ZclAttributeValue(ZclDataType.Uint8, 1));

        var result = _mapper.Map(report);

        Assert.Null(result);
    }

    [Fact]
    public void Map_UnknownNetworkAddress_ReturnsNull()
    {
        var report = new ZigbeeAttributeReport(
            0x9999,
            1,
            PowerConfigCluster,
            BatteryPercentageAttribute,
            new ZclAttributeValue(ZclDataType.Uint8, 86)
        );

        var result = _mapper.Map(report);

        Assert.Null(result);
    }
}
