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

    // Regression coverage for devices 3 ('Underdesk Lighting Strip') and 4 ('Middle Basement
    // Light') stuck at DeviceType.Unknown in production -- the catalog has always classified these
    // Gledopto models as Light, the bug was that reclassification never ran for them, not that the
    // catalog was missing entries.
    [Theory]
    [InlineData("Gledopto", "GL-MC-001")]
    [InlineData("Gledopto", "GL-B-007P")]
    public void WhenVendorAndModelMatchAPreviouslyStuckProductionDeviceThenResolvesToLight(string vendor, string model)
    {
        var resolver = new DeviceTypeResolver();

        Assert.Equal(DeviceType.Light, resolver.Resolve(vendor, model));
    }
}
