using System.Linq;
using Haus.Core.Models.Devices;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Xunit;

namespace Haus.Utilities.Tests.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public class DeviceTypeOptionsParserTests
{
    private readonly DeviceTypeOptionsParser _parser = new();

    [Fact]
    public void WhenGettingDeviceTypeOptionsThenReturnsAllDeviceTypesFromScriptTag()
    {
        var markdown = SupportedDevicesMarkdown.Sample;

        var options = _parser.Parse(markdown);

        Assert.Equal(1937, options.Count());
    }

    [Fact]
    public void WhenGettingDeviceTypeOptionsThenPopulatesDeviceTypeOptionsFromMarkdown()
    {
        var markdown = SupportedDevicesMarkdown.Sample;

        var options = _parser.Parse(markdown).ToArray();

        Assert.Equal("RS 227 T", options[0].Model);
        Assert.Equal("Innr", options[0].Vendor);
        Assert.Equal(DeviceType.Light, options[0].DeviceType);
    }
}
