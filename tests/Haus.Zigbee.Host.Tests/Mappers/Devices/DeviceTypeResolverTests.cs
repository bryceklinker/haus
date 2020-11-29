using Haus.Core.Models.Devices;
using Haus.Zigbee.Host.Mappers;
using Haus.Zigbee.Host.Tests.Support;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Mappers.Devices
{
    public class DeviceTypeResolverTests
    {
        private readonly DeviceTypeResolver _resolver;

        public DeviceTypeResolverTests()
        {
            _resolver = new DeviceTypeResolver();
        }
        
        [Fact]
        public void WhenModelIsMissingFromMappingsThenReturnsUnknown()
        {
            var message = new Zigbee2MqttMessageBuilder()
                .WithTopicPath("bridge/log")
                .WithMessage("interview_successful")
                .WithType("pairing")
                .WithMeta(meta => meta.WithModel("unknown")
                    .WithFriendlyName("idk")
                ).BuildZigbee2MqttMessage();

            Assert.Equal(DeviceType.Unknown, _resolver.Resolve(message));
        }
    }
}