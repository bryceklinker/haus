using System;
using System.Linq;
using Haus.Core.Models.Devices;
using Haus.Utilities.Tests.Support;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Utilities.Tests.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;

public class DeviceTypeOptionsMergerTests
{
    private readonly DeviceTypeOptionsMerger _merger;

    public DeviceTypeOptionsMergerTests()
    {
        _merger = new DeviceTypeOptionsMerger(new NullLogger<DeviceTypeOptionsMerger>());
    }

    [Fact]
    public void WhenExistingOptionsIsEmptyThenReturnsLatestOptions()
    {
        var merged = _merger.Merge(Array.Empty<DeviceTypeOptions>(), new[]
        {
            new DeviceTypeOptions("vendor", "model")
        }).ToArray();

        Assert.Single(merged);
        DeviceTypeOptionsAssert.AssertContains("vendor", "model", merged);
    }

    [Fact]
    public void WhenExistingOptionsContainsDeviceTypeOptionThenReturnsExistingInMerged()
    {
        var merged = _merger.Merge(new[]
        {
            new DeviceTypeOptions("vendor", "model", DeviceType.LightSensor)
        }, new[]
        {
            new DeviceTypeOptions("vendor", "model")
        }).ToArray();

        Assert.Single(merged);
        DeviceTypeOptionsAssert.AssertContains("vendor", "model", DeviceType.LightSensor, merged);
    }

    [Fact]
    public void WhenExistingAndLatestAreCompletelyDifferentThenBothAreInMerged()
    {
        var merged = _merger.Merge(new[]
        {
            new DeviceTypeOptions("here", "one")
        }, new[]
        {
            new DeviceTypeOptions("other", "two")
        }).ToArray();

        Assert.Equal(2, merged.Length);
        DeviceTypeOptionsAssert.AssertContains("here", "one", merged);
        DeviceTypeOptionsAssert.AssertContains("other", "two", merged);
    }

    [Fact]
    public void WhenMergedThenReturnsOptionsSortedByVendor()
    {
        var merged = _merger.Merge(new[]
        {
            new DeviceTypeOptions("Philips", "idk")
        }, new[]
        {
            new DeviceTypeOptions("ABC", "three")
        }).ToArray();

        Assert.Equal("ABC", merged[0].Vendor);
        Assert.Equal("Philips", merged[1].Vendor);
    }
}