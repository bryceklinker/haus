using System.Linq;
using FluentAssertions;
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

        options.Should().HaveCount(1937);
    }

    [Fact]
    public void WhenGettingDeviceTypeOptionsThenPopulatesDeviceTypeOptionsFromMarkdown()
    {
        var markdown = SupportedDevicesMarkdown.Sample;

        var options = _parser.Parse(markdown).ToArray();

        options[0].Model.Should().Be("RS 227 T");
        options[0].Vendor.Should().Be("Innr");
        options[0].DeviceType.Should().Be(DeviceType.Light);
    }
}
