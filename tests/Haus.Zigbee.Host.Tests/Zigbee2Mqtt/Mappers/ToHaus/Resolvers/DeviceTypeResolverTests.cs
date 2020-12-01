using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Configuration;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToHaus.Resolvers;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToHaus.Resolvers
{
    public class DeviceTypeResolverTests
    {
        private readonly OptionsMonitorFake<HausOptions> _optionsMonitor;
        private readonly DeviceTypeResolver _resolver;

        public DeviceTypeResolverTests()
        {
            _optionsMonitor = new OptionsMonitorFake<HausOptions>(new HausOptions());
            _resolver = new DeviceTypeResolver(_optionsMonitor);
        }

        [Fact]
        public void WhenMetadataDoesNotMatchAnythingThenReturnsUnknownDeviceType()
        {
            var meta = new Zigbee2MqttMetaBuilder()
                .BuildMeta();
            Assert.Equal(DeviceType.Unknown, _resolver.Resolve(meta));
        }

        [Fact]
        public void WhenMetadataModelMatchesThenReturnsDeviceTypeFromDefaults()
        {
            var meta = new Zigbee2MqttMetaBuilder()
                .WithModel("929002335001")
                .WithVendor("Philips")
                .BuildMeta();

            Assert.Equal(DeviceType.Light, _resolver.Resolve(meta));
        }

        [Fact]
        public void WhenMetadataIsMultiFunctionDeviceThenReturnsDeviceTypeWithEachValue()
        {
            var meta = new Zigbee2MqttMetaBuilder()
                .WithModel("9290012607")
                .WithVendor("Philips")
                .BuildMeta();

            var deviceType = _resolver.Resolve(meta);
            Assert.True(deviceType.HasFlag(DeviceType.LightSensor));
            Assert.True(deviceType.HasFlag(DeviceType.MotionSensor));
            Assert.True(deviceType.HasFlag(DeviceType.TemperatureSensor));
        }

        [Fact]
        public void WhenMetadataIsInOptionsThenReturnsDeviceTypeFromOptions()
        {
            _optionsMonitor.TriggerChange(new HausOptions
            {
                DeviceTypeOptions = new []
                {
                    new DeviceTypeOptions(vendor: "Old", model: "Klinker", deviceType: DeviceType.Light) 
                }
            });
            
            var meta = new Zigbee2MqttMetaBuilder()
                .WithModel("Klinker")
                .WithVendor("Old")
                .BuildMeta();

            var deviceType = _resolver.Resolve(meta);
            Assert.Equal(DeviceType.Light, deviceType);
        }
    }
}