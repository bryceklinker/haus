using System.Linq;
using System.Text;
using FluentAssertions;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Core.Models.Discovery;
using Haus.Core.Models.Lighting;
using Haus.Testing.Support;
using Haus.Zigbee.Host.Tests.Support;
using Haus.Zigbee.Host.Zigbee2Mqtt.Mappers.ToZigbee;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Haus.Zigbee.Host.Tests.Zigbee2Mqtt.Mappers.ToZigbee
{
    public class HausToZigbeeMapperTests
    {
        private const string Zigbee2MqttBaseTopic = "something";
        private readonly HausToZigbeeMapper _mapper;

        public HausToZigbeeMapperTests()
        {
            var config = ConfigurationFactory.CreateConfig(Zigbee2MqttBaseTopic);
            var mappers = ServiceProviderFactory.Create(config)
                .GetServices<IToZigbeeMapper>();
            
            _mapper = new HausToZigbeeMapper(mappers);
        }
        
        [Fact]
        public void WhenStartDiscoveryCommandReceivedThenReturnsPermitJoinTrueMessage()
        {
            var original = new StartDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            result.Topic.Should().Be($"{Zigbee2MqttBaseTopic}/bridge/config/permit_join");
            result.Payload.Should().BeEncodedString("true");
        }

        [Fact]
        public void WhenStopDiscoveryCommandReceivedThenReturnsPermitJoinFalseMessage()
        {
            var original = new StopDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            result.Topic.Should().Be($"{Zigbee2MqttBaseTopic}/bridge/config/permit_join");
            result.Payload.Should().BeEncodedString("false");
        }

        [Fact]
        public void WhenSyncDevicesCommandReceivedThenReturnsZigbeeGetDevices()
        {
            var original = new SyncDiscoveryModel()
                .AsHausCommand()
                .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            result.Topic.Should().Be($"{Zigbee2MqttBaseTopic}/bridge/config/devices/get");
            result.Payload.Should().BeEmpty();
        }

        [Fact]
        public void WhenDeviceLightingCommandReceivedThenReturnsSetDeviceMessage()
        {
            var original =
                new DeviceLightingChangedEvent(new DeviceModel {ExternalId = "my-ext-id"}, new LightingModel())
                    .AsHausCommand()
                    .ToMqttMessage("haus/commands");

            var result = _mapper.Map(original).Single();

            var payload = JObject.Parse(Encoding.UTF8.GetString(result.Payload));
            result.Topic.Should().Be($"{Zigbee2MqttBaseTopic}/my-ext-id/set");
            payload.Value<string>("state").Should().Be("OFF");
        }
    }
}