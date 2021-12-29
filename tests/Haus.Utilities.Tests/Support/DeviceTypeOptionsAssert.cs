using System.Collections.Generic;
using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;
using Xunit;

namespace Haus.Utilities.Tests.Support;

public static class DeviceTypeOptionsAssert
{
    public static void AssertContains(string vendor, string model, IEnumerable<DeviceTypeOptions> options)
    {
        Assert.Contains(options, d => d.Matches(vendor, model));
    }

    public static void AssertContains(string vendor, string model, DeviceType deviceType,
        IEnumerable<DeviceTypeOptions> options)
    {
        Assert.Contains(options, d => d.Matches(vendor, model) && d.DeviceType == deviceType);
    }
}