using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Resolvers;
using Xunit;

namespace Haus.Core.Models.Tests.Devices.Resolvers;

public class DeviceTypeResolverTests
{
    [Fact]
    public void WhenVendorAndModelDoNotMatchAnythingThenReturnsUnknownDeviceType()
    {
        var resolver = new DeviceTypeResolver();

        Assert.Equal(DeviceType.Unknown, resolver.Resolve("nope", "nope"));
    }

    [Fact]
    public void WhenVendorAndModelMatchThenReturnsDeviceTypeFromDefaults()
    {
        var resolver = new DeviceTypeResolver();

        Assert.Equal(DeviceType.Light, resolver.Resolve("Philips", "929002335001"));
    }

    [Fact]
    public void WhenVendorAndModelAreMultiFunctionDeviceThenReturnsDeviceTypeWithEachValue()
    {
        var resolver = new DeviceTypeResolver();

        var deviceType = resolver.Resolve("Philips", "9290012607");

        Assert.True(deviceType.HasFlag(DeviceType.LightSensor));
        Assert.True(deviceType.HasFlag(DeviceType.MotionSensor));
        Assert.True(deviceType.HasFlag(DeviceType.TemperatureSensor));
    }

    [Fact]
    public void WhenVendorAndModelAreInCustomOptionsThenReturnsDeviceTypeFromCustomOptions()
    {
        var resolver = new DeviceTypeResolver([new DeviceTypeOptions("Old", "Klinker", DeviceType.Light)]);

        var deviceType = resolver.Resolve("Old", "Klinker");

        Assert.Equal(DeviceType.Light, deviceType);
    }

    [Theory]
    [InlineData("Gledopto", "GL-MC-001")]
    [InlineData("Gledopto", "GL-B-007P")]
    public void WhenVendorAndModelMatchAPreviouslyStuckProductionDeviceThenResolvesToLight(string vendor, string model)
    {
        var resolver = new DeviceTypeResolver();

        Assert.Equal(DeviceType.Light, resolver.Resolve(vendor, model));
    }

    [Theory]
    [InlineData("SML001")]
    [InlineData("SML002")]
    [InlineData("SML003")]
    [InlineData("SML004")]
    public void WhenVendorAndModelMatchAPhilipsHueMotionSensorThenResolvesToMultiFunctionSensor(string model)
    {
        var resolver = new DeviceTypeResolver();

        var deviceType = resolver.Resolve("Philips", model);

        Assert.True(deviceType.HasFlag(DeviceType.LightSensor));
        Assert.True(deviceType.HasFlag(DeviceType.MotionSensor));
        Assert.True(deviceType.HasFlag(DeviceType.TemperatureSensor));
    }
}
