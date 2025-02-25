using FluentAssertions;
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

        deviceOptions.DeviceType.Should().Be(deviceType);
    }

    [Fact]
    public void WhenSupportedDeviceExposesMultipleThenDeviceTypeHasEachFlag()
    {
        var supportedDevice = new SupportedDevice("", "", ["light", "temperature", "occupancy"]);

        var deviceOptions = supportedDevice.ToDeviceTypeOption();

        deviceOptions.DeviceType.Should().HaveFlag(DeviceType.Light);
        deviceOptions.DeviceType.Should().HaveFlag(DeviceType.TemperatureSensor);
        deviceOptions.DeviceType.Should().HaveFlag(DeviceType.MotionSensor);
    }
}
