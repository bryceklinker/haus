using System;
using System.Collections.Generic;
using System.Linq;
using Haus.Core.Models;
using Haus.Zigbee.Host.Configuration;

namespace Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public interface IDeviceTypeOptionsParser
{
    IEnumerable<DeviceTypeOptions> Parse(string html);
}

public class DeviceTypeOptionsParser : IDeviceTypeOptionsParser
{
    private const string SUPPORTED_DEVICES_VARIABLE_NAME = "ZIGBEE2MQTT_SUPPORTED_DEVICES";

    public IEnumerable<DeviceTypeOptions> Parse(string markdown)
    {
        var jsonArray = ExtractSupportedDevicesJson(markdown);
        var supportedDevices = HausJsonSerializer.Deserialize<SupportedDevice[]>(jsonArray);
        return supportedDevices.Select(d => d.ToDeviceTypeOption()).ToArray();
    }

    private static string ExtractSupportedDevicesJson(string markdown)
    {
        var variableNameIndex = markdown.IndexOf(SUPPORTED_DEVICES_VARIABLE_NAME, StringComparison.Ordinal);
        var arrayStartIndex = variableNameIndex + SUPPORTED_DEVICES_VARIABLE_NAME.Length + 3;
        var arrayEndIndex = markdown.IndexOf(";", StringComparison.Ordinal);
        return markdown.Substring(arrayStartIndex, arrayEndIndex - arrayStartIndex);
    }
}
