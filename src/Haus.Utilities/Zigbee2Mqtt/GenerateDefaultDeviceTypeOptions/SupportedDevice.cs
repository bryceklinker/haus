using System;
using System.Linq;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;

namespace Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public record SupportedDevice(string Model, string Vendor, string[] Exposes)
{
    private const string Light = "light";
    private const string Temperature = "temperature";
    private const string Switch = "switch";
    private const string Occupancy = "occupancy";
    private const string Illuminance = "illuminance";
    
    public DeviceTypeOptions ToDeviceTypeOption()
    {
        return new DeviceTypeOptions(Vendor, Model, ConvertExposesToDeviceType());
    }

    private DeviceType ConvertExposesToDeviceType()
    {
        var deviceType = DeviceType.Unknown;
        if (IsInExposes(Light)) deviceType |= DeviceType.Light;
        if (IsInExposes(Switch)) deviceType |= DeviceType.Switch;
        if (IsInExposes(Temperature)) deviceType |= DeviceType.TemperatureSensor;
        if (IsInExposes(Occupancy)) deviceType |= DeviceType.MotionSensor;
        if (IsInExposes(Illuminance)) deviceType |= DeviceType.LightSensor;
        return deviceType;
    }

    private bool IsInExposes(string value)
    {
        return Exposes.Any(e => string.Compare(e, value, StringComparison.OrdinalIgnoreCase) ==  0);
    }
}