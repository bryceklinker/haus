using System.Linq;
using Haus.Utilities.Tests.Support;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Haus.Utilities.Tests.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions
{
    public class DeviceTypeOptionsHtmlParserTests
    {
        private readonly DeviceTypeOptionsHtmlParser _parser;

        public DeviceTypeOptionsHtmlParserTests()
        {
            _parser = new DeviceTypeOptionsHtmlParser(new NullLogger<DeviceTypeOptionsHtmlParser>());
        }
        
        [Fact]
        public void WhenHtmlContainsOneVendorWithOneDeviceThenReturnsOneDeviceTypeOption()
        {
            var html = new SupportedDevicesPageHtmlBuilder()
                .WithVendor(vendor =>
                    vendor.WithName("Phillips")
                        .WithDevice(device =>
                            device.WithModel("SomeModel")
                        )
                )
                .Build();

            var options = _parser.Parse(html).ToArray();

            Assert.Single(options);
            DeviceTypeOptionsAssert.AssertContains("Phillips", "SomeModel", options);
        }

        [Fact]
        public void WhenHtmlContainsOneVendorWithTwoDevicesThenReturnsTwoDeviceTypeOptions()
        {
            var html = new SupportedDevicesPageHtmlBuilder()
                .WithVendor(vendor => 
                    vendor.WithName("Jackson")
                        .WithDevice(device =>
                            device.WithModel("One")
                        )
                        .WithDevice(device =>
                            device.WithModel("Two")
                        )
                )
                .Build();
            
            var options = _parser.Parse(html).ToArray();

            Assert.Equal(2, options.Length);
            DeviceTypeOptionsAssert.AssertContains("Jackson", "One", options);
            DeviceTypeOptionsAssert.AssertContains("Jackson", "Two", options);
        }

        [Fact]
        public void WhenHtmlContainsTwoVendorsWithOneDeviceEachThenReturnsTwoDeviceTypeOptions()
        {
            var html = new SupportedDevicesPageHtmlBuilder()
                .WithVendor(vendor =>
                    vendor.WithName("IDK")
                        .WithDevice(device =>
                            device.WithModel("some")
                        )
                )
                .WithVendor(vendor =>
                    vendor.WithName("Other")
                        .WithDevice(device =>
                            device.WithModel("other")
                        )
                )
                .Build();

            var options = _parser.Parse(html).ToArray();
            
            Assert.Equal(2, options.Length);
            DeviceTypeOptionsAssert.AssertContains("IDK", "some", options);
            DeviceTypeOptionsAssert.AssertContains("Other", "other", options);
        }
    }
}