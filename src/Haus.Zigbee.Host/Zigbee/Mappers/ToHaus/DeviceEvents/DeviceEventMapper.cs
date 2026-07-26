using Microsoft.Extensions.Logging;

namespace Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.DeviceEvents;

public class DeviceEventMapper(DeviceAddressRegistry addressRegistry, ILogger<DeviceEventMapper> logger)
{
    private const ushort PowerConfigCluster = 0x0001;
    private const ushort IlluminanceMeasurementCluster = 0x0400;
    private const ushort TemperatureMeasurementCluster = 0x0402;
    private const ushort OccupancySensingCluster = 0x0406;

    private const ushort BatteryPercentageAttribute = 0x0021;
    private const ushort MeasuredValueAttribute = 0x0000;
    private const ushort OccupancyAttribute = 0x0000;

    private readonly BatteryChangedMapper _batteryChangedMapper = new();
    private readonly IlluminanceChangedMapper _illuminanceChangedMapper = new();
    private readonly OccupancyChangedMapper _occupancyChangedMapper = new();
    private readonly TemperatureChangedMapper _temperatureChangedMapper = new();

    public object? Map(Haus.Zigbee.ZigbeeAttributeReport report)
    {
        if (!addressRegistry.TryGetExternalId(report.SourceNwkAddress, out var deviceId))
        {
            logger.LogWarning("No known device for network address {@Address}", report.SourceNwkAddress);
            return null;
        }

        return (report.ClusterId, report.AttributeId) switch
        {
            (PowerConfigCluster, BatteryPercentageAttribute) => _batteryChangedMapper.Map(deviceId, report.Value),
            (IlluminanceMeasurementCluster, MeasuredValueAttribute) => _illuminanceChangedMapper.Map(
                deviceId,
                report.Value
            ),
            (TemperatureMeasurementCluster, MeasuredValueAttribute) => _temperatureChangedMapper.Map(
                deviceId,
                report.Value
            ),
            (OccupancySensingCluster, OccupancyAttribute) => _occupancyChangedMapper.Map(deviceId, report.Value),
            _ => null,
        };
    }
}
