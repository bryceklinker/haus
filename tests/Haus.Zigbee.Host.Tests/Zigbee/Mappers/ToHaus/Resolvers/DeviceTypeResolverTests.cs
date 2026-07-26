using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee.Mappers.ToHaus.Resolvers;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee.Mappers.ToHaus.Resolvers;

public class DeviceTypeResolverTests
{
    private readonly DeviceTypeResolver _resolver;

    public DeviceTypeResolverTests()
    {
        var options = OptionsFactory.CreateHausOptions();
        options.Value.DeviceTypeOptions = [new DeviceTypeOptions("Old", "Klinker", DeviceType.Light)];
        _resolver = new DeviceTypeResolver(options);
    }

    [Fact]
    public void WhenVendorAndModelDoNotMatchAnythingThenReturnsUnknownDeviceType()
    {
        Assert.Equal(DeviceType.Unknown, _resolver.Resolve("nope", "nope"));
    }

    [Fact]
    public void WhenVendorAndModelMatchThenReturnsDeviceTypeFromDefaults()
    {
        Assert.Equal(DeviceType.Light, _resolver.Resolve("Philips", "929002335001"));
    }

    [Fact]
    public void WhenVendorAndModelAreMultiFunctionDeviceThenReturnsDeviceTypeWithEachValue()
    {
        var deviceType = _resolver.Resolve("Philips", "9290012607");

        Assert.True(deviceType.HasFlag(DeviceType.LightSensor));
        Assert.True(deviceType.HasFlag(DeviceType.MotionSensor));
        Assert.True(deviceType.HasFlag(DeviceType.TemperatureSensor));
    }

    [Fact]
    public void WhenVendorAndModelAreInOptionsThenReturnsDeviceTypeFromOptions()
    {
        var deviceType = _resolver.Resolve("Old", "Klinker");

        Assert.Equal(DeviceType.Light, deviceType);
    }
}
