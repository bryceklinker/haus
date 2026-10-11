using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Core.Models.Devices;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Xunit;

namespace Haus.Utilities.Tests.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public class DeviceTypeOptionsParserTests
{
    private const string SupportedDevicesPageUrl =
        "https://raw.githubusercontent.com/Koenkk/zigbee2mqtt.io/master/docs/supported-devices/README.md";

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

    [Fact]
    public void WhenADeviceDescriptionContainsASemicolonThenStillFindsTheEndOfTheArray()
    {
        var markdown =
            "ZIGBEE2MQTT_SUPPORTED_DEVICES = "
            + "[{\"model\":\"RS 227 T\",\"vendor\":\"Innr\",\"description\":\"desc; with a semicolon\",\"exposes\":[\"light\"]}];";

        var options = _parser.Parse(markdown).ToArray();

        Assert.Single(options);
        Assert.Equal("RS 227 T", options[0].Model);
    }

    [Fact]
    public async Task WhenParsingLiveZigbee2MqttSupportedDevicesPageThenFindsNonEmptyDeviceTypeOptions()
    {
        using var httpClient = new HttpClient();
        var markdown = await httpClient.GetStringAsync(SupportedDevicesPageUrl);

        var options = _parser.Parse(markdown).ToArray();

        Assert.NotEmpty(options);
        Assert.All(options, option => Assert.False(string.IsNullOrWhiteSpace(option.Vendor)));
        Assert.All(options, option => Assert.False(string.IsNullOrWhiteSpace(option.Model)));
    }
}
