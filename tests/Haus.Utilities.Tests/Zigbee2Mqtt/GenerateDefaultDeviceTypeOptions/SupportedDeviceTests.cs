using Haus.Core.Models.Devices;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Xunit;

namespace Haus.Utilities.Tests.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public class SupportedDeviceTests
{
    [Theory]
    [InlineData("light", DeviceType.Light)]
    [InlineData("switch", DeviceType.Switch)]
    [InlineData("temperature", DeviceType.TemperatureSensor)]
    [InlineData("occupancy", DeviceType.MotionSensor)]
    [InlineData("illuminance", DeviceType.LightSensor)]
    public void WhenConvertedToDeviceTypeOptionThenConvertsExposesToDeviceType(string exposes, DeviceType deviceType)
    {
        var supportedDevice = new SupportedDevice("", "", [exposes]);

        var deviceOptions = supportedDevice.ToDeviceTypeOption();

        Assert.Equal(deviceType, deviceOptions.DeviceType);
    }

    [Fact]
    public void WhenSupportedDeviceExposesMultipleThenDeviceTypeHasEachFlag()
    {
        var supportedDevice = new SupportedDevice("", "", ["light", "temperature", "occupancy"]);

        var deviceOptions = supportedDevice.ToDeviceTypeOption();

        Assert.True(deviceOptions.DeviceType.HasFlag(DeviceType.Light));
        Assert.True(deviceOptions.DeviceType.HasFlag(DeviceType.TemperatureSensor));
        Assert.True(deviceOptions.DeviceType.HasFlag(DeviceType.MotionSensor));
    }
}
