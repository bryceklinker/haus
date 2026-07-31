using System;
using Haus.Core.Models.Devices.Sensors.Battery;
using Haus.Core.Models.Devices.Sensors.Light;
using Haus.Core.Models.Devices.Sensors.Motion;
using Haus.Core.Models.Devices.Sensors.Temperature;
using Haus.Core.Models.ExternalMessages;
using Haus.Zigbee.Models;
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

    public HausEvent<object>? Map(ZigbeeAttributeReport report)
    {
        if (!addressRegistry.TryGetExternalId(report.SourceNwkAddress, out var deviceId))
        {
            logger.LogWarning("No known device for network address {@Address}", report.SourceNwkAddress);
            return null;
        }

        object? payload = (report.ClusterId, report.AttributeId) switch
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

        return payload == null ? null : new HausEvent<object>(GetHausEventType(payload), payload);
    }

    private static string GetHausEventType(object payload)
    {
        return payload switch
        {
            BatteryChangedModel => BatteryChangedModel.Type,
            IlluminanceChangedModel => IlluminanceChangedModel.Type,
            TemperatureChangedModel => TemperatureChangedModel.Type,
            OccupancyChangedModel => OccupancyChangedModel.Type,
            _ => throw new InvalidOperationException($"Unexpected sensor payload type {payload.GetType()}"),
        };
    }
}
